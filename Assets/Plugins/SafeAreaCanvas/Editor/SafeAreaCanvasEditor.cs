using UnityEngine;
using UnityEditor;

namespace SafeAreaCanvas
{
    [CustomEditor(typeof(SafeAreaCanvas))]
    public class SafeAreaCanvasEditor : Editor
    {
        public SafeAreaCanvas.Orientation orientation;

        public bool isFold;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var safeCanvas = target as SafeAreaCanvas;

            if (safeCanvas.coverUnsafeArea)
            {
                safeCanvas.coverColor = EditorGUILayout.ColorField("    Cover Color", safeCanvas.coverColor);
            }

            isFold = EditorGUILayout.Foldout(isFold, "Notch Simulation");

            if (isFold)
            {
                orientation = (SafeAreaCanvas.Orientation)EditorGUILayout.EnumPopup("    Orientation", orientation);

                if (!safeCanvas.IsNotchShowing)
                {
                    if (GUILayout.Button("Show Notch"))
                        safeCanvas.ShowNotch(orientation);
                }
                else
                {
                    if (GUILayout.Button("Hide Notch"))
                        safeCanvas.HideNotch();
                }
            }
        }
    }
}
