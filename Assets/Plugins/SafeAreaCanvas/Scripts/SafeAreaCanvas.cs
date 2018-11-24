using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SafeAreaCanvas
{
    public enum UpdateTiming
    {
        OnAwake = 0,
        OnStart,
        OnUpdate,
        OnFixedUpdate
    }

    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaCanvas : MonoBehaviour
    {
        private const string ROOT_NAME = "Safe Root";
        private const string NOTCH_COVER_NAME = "Notch Cover";

        [SerializeField] private UpdateTiming updateTimming;

        [SerializeField] private bool safeHorizontal = true;
        [SerializeField] private bool safeVertical = true;

        public bool coverUnsafeArea;
        [HideInInspector] public Color coverColor = Color.black;

        private RectTransform safeRoot;

        private void Awake()
        {
            if (updateTimming != UpdateTiming.OnAwake)
                return;

            UpdateSafeArea();
        }

        private void Start()
        {
            if (updateTimming != UpdateTiming.OnStart)
                return;

            UpdateSafeArea();
        }

        private void Update()
        {
            if (updateTimming != UpdateTiming.OnUpdate)
                return;

            UpdateSafeArea();
        }

        private void FixedUpdate()
        {
            if (updateTimming != UpdateTiming.OnFixedUpdate)
                return;

            UpdateSafeArea();
        }

        public void UpdateSafeArea()
        {
            UpdateSafeArea(Screen.width, Screen.height, Screen.safeArea);
        }

        private void UpdateSafeArea(float width, float height, Rect safeArea)
        {
            UpdateRoot();

            if (coverUnsafeArea)
                CoverUnsafeArea(width, height, safeArea);

            safeRoot.anchorMin = new Vector2(safeHorizontal ? safeArea.x / width : 0,
                                             safeVertical ? safeArea.y / height : 0);
            safeRoot.anchorMax = new Vector2(safeHorizontal ? (safeArea.x + safeArea.width) / width : 1,
                                             safeVertical ? (safeArea.y + safeArea.height) / height : 1);
        }

        private void UpdateRoot()
        {
            var children = new List<Transform>();
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);

                if (ROOT_NAME.Equals(child.name))
                {
                    safeRoot = child as RectTransform;
                    return;
                }

                children.Add(child);
            }

            var go = new GameObject(ROOT_NAME, typeof(RectTransform));
            go.transform.SetParent(transform);

            safeRoot = go.transform as RectTransform;
            StretchRoot();

            children.ForEach(child => child.SetParent(safeRoot));
        }

        private void CoverUnsafeArea(float width, float height, Rect safeArea)
        {
            var prefab = Resources.Load(NOTCH_COVER_NAME) as GameObject;

            var goCover = Instantiate(prefab, transform);
            goCover.name = NOTCH_COVER_NAME;

            var coverRt = goCover.GetComponent<RectTransform>();
            coverRt.SetParent(transform);

            coverRt.localScale = Vector3.one;
            coverRt.offsetMin = Vector2.zero;
            coverRt.offsetMax = Vector2.zero;

            var childCount = coverRt.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = coverRt.GetChild(i) as RectTransform;

                if ("Left".Equals(child.name))
                {
                    child.anchorMin = Vector2.zero;
                    child.anchorMax = safeHorizontal ? new Vector2(safeArea.x / width, 1) : Vector2.zero;
                }
                else if ("Right".Equals(child.name))
                {
                    child.anchorMin = safeHorizontal ? new Vector2((safeArea.x + safeArea.width) / width, 0) : Vector2.one;
                    child.anchorMax = Vector2.one;
                }
                else if ("Top".Equals(child.name))
                {
                    child.anchorMin = safeVertical ? new Vector2(0, (safeArea.y + safeArea.height) / height) : Vector2.one;
                    child.anchorMax = Vector2.one;
                }
                else if ("Bottom".Equals(child.name))
                {
                    child.anchorMin = Vector2.zero;
                    child.anchorMax = safeVertical ? new Vector2(1, safeArea.y / height) : Vector2.zero;
                }

                var image = child.GetComponent<Image>();
                image.color = coverColor;
            }
        }

        private void StretchRoot()
        {
            safeRoot.localScale = Vector3.one;
            safeRoot.anchorMin = Vector2.zero;
            safeRoot.anchorMax = Vector2.one;
            safeRoot.sizeDelta = Vector2.zero;
            safeRoot.offsetMin = Vector2.zero;
            safeRoot.offsetMax = Vector2.zero;
        }
#if UNITY_EDITOR
		private const string NOTCH_NAME = "Notch";

		public enum Orientation
		{
			Portrait,
			Landscape
		}

		private bool isNotchShowing;
		
		public bool IsNotchShowing
		{
			get { return isNotchShowing; }
		}
		
		public void ShowNotch(Orientation orientation)
		{
			isNotchShowing = true;
			
			UpdateRoot();
			
			float notchSize;
			switch (orientation)
			{
				case Orientation.Portrait:
					notchSize = safeRoot.rect.height * 0.037f;
					InstantiateNotch(orientation, notchSize);
					UpdateSafeArea(safeRoot.rect.width, 
					               safeRoot.rect.height, 
					               new Rect(0, notchSize, safeRoot.rect.width, safeRoot.rect.height - 2 * notchSize));
					break;
				case Orientation.Landscape:
					notchSize = safeRoot.rect.width * 0.037f;
					InstantiateNotch(orientation, notchSize);
					UpdateSafeArea(safeRoot.rect.width, 
					               safeRoot.rect.height, 
					               new Rect(notchSize, 0, safeRoot.rect.width - 2 * notchSize, safeRoot.rect.height));
					break;
			}
		}
 
		public void HideNotch()
		{
			isNotchShowing = false;
			
			DestroyNotch();
			StretchRoot();
		}

		private void InstantiateNotch(Orientation orientation, float notchSize)
		{
			var prefab = Resources.Load(string.Format("{0} {1}", orientation, NOTCH_NAME)) as GameObject;
			
			var goNotch = Instantiate(prefab, transform);
			goNotch.name = NOTCH_NAME;

			var notchRt = goNotch.GetComponent<RectTransform>();
			notchRt.SetParent(transform);

			notchRt.localScale = Vector3.one;
			notchRt.offsetMin = Vector2.zero;
			notchRt.offsetMax = Vector2.zero;

			var childCount = notchRt.childCount;

			for (var i = 0; i < childCount; i++)
			{
				var child = notchRt.GetChild(i) as RectTransform;
				switch (orientation)
				{
					case Orientation.Portrait:
						if (child.name.Equals(NOTCH_NAME))
							child.sizeDelta = new Vector2(notchSize * 7.166f, notchSize);
						else
							child.sizeDelta = new Vector2(notchSize, notchSize);
						break;
					case Orientation.Landscape:
						if (child.name.Equals(NOTCH_NAME))
							child.sizeDelta = new Vector2(notchSize * 7.166f, notchSize);
						else
							child.sizeDelta = new Vector2(notchSize, notchSize);
						break;
				}
			}
		}

		private void DestroyNotch()
		{
			var notches = new List<GameObject>();

			foreach (Transform tr in transform)
			{
				if (NOTCH_NAME.Equals(tr.name) || NOTCH_COVER_NAME.Equals(tr.name))
					notches.Add(tr.gameObject);
			}

			notches.ForEach(notch =>
			{
				if (Application.isEditor)
					DestroyImmediate(notch);
				else
					Destroy(notch);
			});
		}
#endif
    }
}
