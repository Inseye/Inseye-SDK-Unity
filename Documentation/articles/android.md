# Implementation details in Android
*created by [Mateusz Chojnowski](mailto:mateusz.chojnowski@inseye.com)*

On Android, Inseye SDK communicates with the eye tracker trough another application, called service, working as a middleman between electronics and user application installed on OS.
The service application must be installed on the headset and can be downloaded from the [App Center](https://install.appcenter.ms/orgs/remmed/apps/inseye-service-1/distribution_groups/public).

The SDK binds to the Android service on demand and automatically disconnects when the eye tracking features are no longer needed.
Communication is established with an android Java library that is called from Unity and distributed with SDK. Java library source code is available in [Inseye Unity SDK Android](https://github.com/Inseye/Inseye-Unity-SDK-Android) repository. The code is under the same license as SDK.

Communication between the service and the android library in Unity is done via the common AIDL interface wrapped in Java classes.
Definitions of public API interfaces are available in [Inseye-SDK-Android-API](https://github.com/Inseye/Inseye-SDK-Android-API) repository.

Communication flow flowchart.
```mermaid
flowchart LR
   1[Physical Device Firmware] <--> 2[Service] <--> 3[Public API] <--AIDL\nUDP--> 4[SDK Java Library] <--Unity Java\nInterop --> 5[C# Unity SDK]
```