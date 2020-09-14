![logo](https://user-images.githubusercontent.com/18159012/42125819-e0f2d9e2-7cb8-11e8-9833-52c911b3e8af.png)

# SafeAreaCanvas

![unity_version](https://img.shields.io/badge/Unity-2020.1-blue.svg?style=flat-square) ![license](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)

Unity plugin to handle safe area super easily :D

## Getting Start

SafeAreaCanvas component require UGUI's Canvas component. So it is recommended that you first set the UGUI's Canvas component and then add SafeAreaCanvas.

### Options

- Safe Horizontal : Adjust canvas elements with safe area's horizontal value.
- Safe Vertical : Adjust canvas elements with safe area's vertical value.
- Cover Unsafe Area : Cover unsafe area with static color : Cover Color.

## Running the simulation

To simulate safe area canvas, just set example orientation and push the 'Show Notch' button. That's all!

### Caution

SafeAreaCanvas generate 'Safe Root' when simulate notch. This is used as root to adjust size of canvas in SafeAreaCanvas. So if you create new UI element out of this root, that element doesn't support safe area.
