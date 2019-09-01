using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace EyeSkills
{
    public class MultiCameraController : MonoBehaviour
    {

        public GameObject camera2D;
        public GameObject camera3D;

        private Action<bool> callbackSwitch3DComplete,callbackSwitch2DComplete;

        public static MultiCameraController instance;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        public void SwitchTo3D(Action<bool> _callbackSwitch3DComplete)
        {
            callbackSwitch3DComplete = _callbackSwitch3DComplete;
            StartCoroutine("SwitchTo3DCoroutine");           
        }


        public void SwitchTo2D(Action<bool> _callbackSwitch2DComplete)
        {
            callbackSwitch2DComplete = _callbackSwitch2DComplete;
            StartCoroutine("SwitchTo2DCoroutine");
        }

        public EyeSkillsCameraRig GetVRCameraRig(){
            return camera3D.GetComponent<EyeSkillsCameraRig>();
        }

        IEnumerator SwitchTo3DCoroutine()
        {

            // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
            string desiredDevice = "cardboard"; // Or "cardboard".

            // Some VR Devices do not support reloading when already active, see
            // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
            if (String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
            {
                Debug.Log("Deactivating 2D Camera Rig and activated 3D Camera Rig");

                //Switching order
                camera3D.SetActive(true);
                //camera2D.GetComponent<Camera>().enabled = false;
                camera2D.SetActive(false);

                //Will this help to provide a frame for the activation to settle?
                yield return null;

                //First we need to remove the TrackedPoseDriver before making the switch, otherwise we have hell to pay as Unity break the TPD.
                Debug.Log("Removing TrackedPoseDrivers");
                GetVRCameraRig().RemoveTrackedPoseDrivers();


                Debug.Log("Changing Display Devices");

                XRSettings.LoadDeviceByName(desiredDevice);
                Debug.Log("Loaded device " + desiredDevice);

                //https://github.com/googlevr/gvr-unity-sdk/issues/492#issuecomment-285752385
                yield return null; // try also: yield return WaitForEndOfFrame();
                Debug.Log("Waited one cycle");
//#if !UNITY_EDITOR

                //while (!XRSettings.loadedDeviceName.Equals(desiredDevice))
                //{
                //    Debug.LogWarning("Waiting extra frame…");

                //    yield return null;
                //}
//#endif

                // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
                //yield return null;

                // Now it's ok to enable VR mode.
                XRSettings.enabled = true;
                Debug.Log("Enabled XR");

                //Now we can add the tracked pose drivers back in....
                GetVRCameraRig().RestoreTrackedPoseDrivers();

            }
            callbackSwitch3DComplete(true); 
        }

        IEnumerator SwitchTo2DCoroutine()
        {
            Debug.Log("Deactivated 3D Camera and activated 2D Camera");
            camera3D.SetActive(false); //May need to reset the 3D camera position? This may also cause hassles with config management.

            camera2D.SetActive(true);
            camera2D.GetComponent<Camera>().enabled = true;
            // Empty string loads the "None" device.
            XRSettings.LoadDeviceByName("");

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            yield return null;

            while (!XRSettings.loadedDeviceName.Equals(""))
            {
                Debug.LogWarning("Waiting extra frame…");
                yield return null;
            }

            // Not needed, since loading the None (`""`) device takes care of this.
            XRSettings.enabled = false;
            callbackSwitch2DComplete(true);
        }
    }
}