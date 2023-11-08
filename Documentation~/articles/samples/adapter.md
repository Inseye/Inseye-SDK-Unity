# Adapter
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@remmed.vision)*

The `InseyeGazeInputAdapter` sample contains single mono behavior that injects the Inseye eye tracker data into the [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html).

## Adapter behavior
When enabled adapter monitors the eye tracker [availability](../../api/Inseye.InseyeEyeTrackerAvailability.yml) and when the eye tracker is available it creates new [input device](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputDevice.html). In update event of the adapter behavior data from the eye tracker is passed into created input device.

The created device is compatible with input actions declared in the [XR Interaction Toolkit Starter Assets Sample](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.3/manual/samples.html#starter-assets).

## OpenXR plugin
This sample works with and without OpenXR plugin. If the OpenXR plugin is included in the project then adapter registers as OpenXR compatible input device.