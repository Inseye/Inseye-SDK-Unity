# SDK Structure
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@remmed.vision)*

Under the hood, the Inseye software stack is dependent on the runtime platform.
Unity SDK abstracts from it and provides a unified API that is, in turn, independent.

Nonetheless, if you are interested in a platform specific implementation details checkout:

[Android](./android.md)

# InseyeSDK
Most of the SDK features are available through the [InseyeSDK](../api/Inseye.InseyeSDK.yml) static class. Inseye SDK is not thread-safe and must be called from the main Unity thread.

All public methods from the [InseyeSDK](../api/Inseye.InseyeSDK.yml) may throw one of the exceptions defined in [Exception](../api/Inseye.Exceptions.yml). It's user responsibility to handle them properly.

## SKD and eye tracker lifecycle
All communication with the eye tracking device is done on demand in the background. SDK asks for resources and releases them automatically in response to the developer's requests, e.g. eye tracker starts streaming data and enters the [Initialized](../api/Inseye.InseyeSDKState.yml) and [MostRecentGazePointAvailable](../api/Inseye.InseyeSDKState.yml) states when the developer asks for data by creating [GetGazeProvider](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_GetGazeProvider). 

**Developer can (and should) control the SDK state indirectly by properly disposing objects returned by the SDK when they are no longer needed.**

## Tracking physical connection
Application may need information about the physical connection with the eye tracker to e.g. display a popup message instructing to plug in the device, perform calibration etc.

To obtain information about the current connection call [GetEyeTrackerAvailability](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_GetEyetrackerAvailability), to receive updates about availability changes subscribe to the [EyeTrackerAvailabilityChanged](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_EyeTrackerAvailabilityChanged) event.


## Obtaining gaze data
To obtain gaze data developer must create an object implementing [IGazeProvider](../api/Inseye.Interfaces.IGazeProvider.yml) by calling the appropriate methods from the [InseyeSDK](../api/Inseye.InseyeSDK.yml) - [GetGazeProvider](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_GetGazeProvider).
Gaze data can be pooled from from [GetGazeProvider](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_GetGazeProvider) by calling [GetMostRecentGazeData](../api/Inseye.Interfaces.IGazeProvider.yml#Inseye_Interfaces_IGazeProvider_GetMostRecentGazeData_Inseye_InseyeGazeData__) each frame.

Please remember to dispose of the gaze data sources when they are no longer needed.

## Calibration procedure
Calibration is a procedure during which the eye tracker device creates a map from the signal, based on the application user's gaze, to the gaze position in the application.
During calibration, a fixation point is displayed at multiple locations and the application user is asked to focus their gaze (fixate) on the visible point.

To start the calibration procedure the developer must call [StartCalibration](../api/Inseye.InseyeSDK.yml#Inseye_InseyeSDK_StartCalibration), returned calibration procedure is a position source for point/object used as fixation point. 
Then if valid object was returned developer must call [ReportReadyToDisplayPoints](../api/Inseye.Interfaces.ICalibrationProcedure.yml#Inseye_Interfaces_ICalibrationProcedure_ReportReadyToDisplayPoints) method.
Please check reference implementation of a class that consumes the calibration position at [InseyeCalibration](../api/Inseye.Samples.Calibration.InseyeCalibration.yml)

