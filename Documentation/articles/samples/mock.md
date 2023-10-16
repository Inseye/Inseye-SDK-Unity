# Mock
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@inseye.com)*

Mock is a code-only sample that provides the implementation of an eye tracker mock, which uses Unity Input System as a source of data.

## Requirements
Mock sample requires basic knowledge of [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/QuickStartGuide.html) and Input System Assets in the project configured as an input data provider.

## Usage
1. Import sample from the Inseye Unity SDK in [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) window.

[<image src="../../images/samples/mocks_import.png" width="450"/>](../../images/samples/mocks_import.png)

2. Add [InseyeEyeTrackerInputSystemMock](../../api/Inseye.Samples.Mocks.InseyeEyeTrackerInputSystemMock.yml) to an object in the scene.

[<image src="../../images/samples/mocks_add_to_object.png" width="300"/>](../../images/samples/mocks_add_to_object.png)

3. Configure the mock by selecting data source and configuring input references:
- select `Middle Of The Screen` if you want the gaze to be simulated at the center of the screen space  

    [<image src="../../images/samples/middle_of_the_screen.png" width="400"/>](../../images/samples/middle_of_the_screen.png)
- select `Delta` if the data source returned values is to be a stream of minor changes such as \[WASD\] keys or controller thumb stick
  
    [<image src="../../images/samples/delta.png" width="400"/>](../../images/samples/delta.png)
- select `Screen Position` if data source values are to be an absolute position in the screenspace such as mouse position
  
    [<image src="../../images/samples/screen_position.png" width="400"/>](../../images/samples/screen_position.png)
- select `Controller Position And Direction` to use virtual ray from controller to plane at arbitrary position in front of camera as a source of gaze position.
 
    [<image src="../../images/samples/controller_pos_dir.png" width="400"/>](../../images/samples/controller_pos_dir.png) 

4. Use the mock like a standard SDK implementation via the [InseyeSDK](../../api/Inseye.InseyeSDK.yml) class.

`InseyeEyeTrackerInputSystemMock.StartCalibration` returns [MockCalibrationProcedure](../../api/Inseye.Samples.Mocks.MockCalibrationProcedure.yml) which sets a new calibration point every few seconds. 

## Simulating eye openness
The [InseyeEyeTrackerInputSystemMock](../../api/Inseye.Samples.Mocks.InseyeEyeTrackerInputSystemMock.yml) object allows developer to simulate eye openness either by changing `Opened Eyes` in the inspector or by setting [OpenedEyes property](../../api/Inseye.Samples.Mocks.InseyeEyeTrackerInputSystemMock.yml#Inseye_Samples_Mocks_InseyeEyeTrackerInputSystemMock_OpenedEyes).

## Simulating gaze availability
[InseyeEyeTrackerInputSystemMock](../../api/Inseye.Samples.Mocks.InseyeEyeTrackerInputSystemMock.yml) allows developer to change eye tracker availability. Either by changing `Eyetracker availablity` field in inspector or by calling [SetEyeTrackerAvailability method](../../api/Inseye.Samples.Mocks.InseyeMockSDKImplementation.yml#Inseye_Samples_Mocks_InseyeMockSDKImplementation_SetEyeTrackerAvailability_Inseye_InseyeEyeTrackerAvailability_).
[EyeTrackerAvailabilityChanged](../../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_EyeTrackerAvailabilityChanged) is invoked when availability is changed in inspector or with a method call.

## Simulating selection of most accurate eye
The [InseyeEyeTrackerInputSystemMock](../../api/Inseye.Samples.Mocks.InseyeEyeTrackerInputSystemMock.yml) instance allows developer to change eye that will be used as a source for gaze data. Either by changing `Most accurate eye` field in inspector or by calling the [SetMostAccurateEye method](../../api/Inseye.Samples.Mocks.InseyeMockSDKImplementation.yml#Inseye_Samples_Mocks_InseyeMockSDKImplementation_SetMostAccurateEye_Inseye_Eyes_) method.
The [MostAccurateEyeChanged](../../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_MostAccurateEyeChanged) delegate is invoked when most accurate eye is changed in inspector or with a method call.

## Custom mock implementation
To create a custom mock implementation inherit the [InseyeMockSDKImplementation](../../api/Inseye.Samples.Mocks.InseyeMockSDKImplementation.yml) overriding `GetMostRecentDataPoint` (mandatory), `InheritorAwake`, and `StartCalibration`.

