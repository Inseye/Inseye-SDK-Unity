# Calibration
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@remmed.vision)*

Calibration sample contains an implemented (recommended) calibration procedure as a separate scene and code that can be used in a new calibration implementation.

## Structure
Calibration sample directory contains:
- [InseyeCalibration](../../api/Inseye.Samples.Calibration.InseyeCalibration.yml) - code that defines in-scene behavior during calibration, `InseyeCalibration.PerformCalibration` is an entry point for calibration in this sample,
- `CalibrationScene.unity` - scene loaded during the calibration,
- `AnimationHelpers` - directory containing scripts used in animating the fixation point during the calibration.

## Usage
1. Import the Calibration sample from [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) window.

[<image src="../../images/samples/calibration_import.png" width="450"/>](../../images/samples/calibration_import.png)

2. Add calibration scene to the project build.

[<image src="../../images/samples/calibration_build.png" width="750"/>](../../images/samples/calibration_build.png)

3. Invoke the calibration `InseyeCalibration.PerformCalibration` in the runtime to calibrate application user.
- a proper callback must be provided to return to te previous scene after a successful or failed calibration. The code bellow shows how to start the calibration procedure and change back the scene to the original one.
```cs
var calibrationProcedure = Inseye.InseyeSDK.StartCalibration();
var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
var activeSceneBuildIndex = activeScene.buildIndex;
System.Action callback = () => UnityEngine.SceneManagement.SceneManager.LoadScene(activeSceneBuildIndex);
Inseye.Samples.Calibration.InseyeCalibration.PerformCalibration(calibrationProcedure, callback, callback);
```
Alternatively, one can use `InseyeCalibration.PerformCalibrationAndReturn` to get the same behavior.