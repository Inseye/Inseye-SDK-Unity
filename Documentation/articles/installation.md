# The SDK installation in a Unity project
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@inseye.com)*

1. Download the SDK from the [Github repository](https://github.com/Inseye/Inseye-Unity-SDK)
2. Add the SDK to project:
- Open Unity package manager by opening `Window` menu from the top bar and selecting `Package Manager` option

[<img src="../images/install_package_unity.png" width="350"/>](../images/install_package_unity.png)

- Add package from disk.

[<image src="../images/install_package_manager.png" width="450"/>](../images/install_package_manager.png)

# Configuration
The SDK doesn't require any additional configuration, all public API is ready to go.

# What next?
- [The SDK structure](./sdk_structure.md) provides insight on the general SDK structure and contains information about implementation details.

# Samples
Three samples are delivered with the SDK:
- [Calibration](./samples/calibration.md) – contains sample eye tracker calibration scene and all the necessary code.
- [Mocks](./samples/mock.md) – contains eye tracker mock implementation that can be used during development.
- [InseyeGazeInputAdapter](./samples/adapter.md) - contains adapter that makes working with [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html) and [Unity XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html) simpler.