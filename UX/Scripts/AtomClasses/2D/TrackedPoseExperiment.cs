using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SpatialTracking;

public class TrackedPoseExperiment : MonoBehaviour
{
    public TrackedPoseDriver leftPose;
    public TrackedPoseDriver rightPose;
    public GameObject leftCamera;
    public GameObject rightCamera;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Experiment");
        //StartCoroutine(SwitchTo2DCoroutineExp1());

        //TrackedPoseDrivers set to Position ONLY. This experiment disables the TrackedPoseDriver proactively to re-enable it again after the 2D->3D mode switch
        //StartCoroutine(SwitchTo2DCoroutineExp2());
        //FAILED.  After 2D->3D both rotating

        //TrackedPoseDrivers set to Position ONLY. This experiment removes the component before 3D->2D and then, after 2D->3D, and recreates it.
        StartCoroutine(SwitchTo2DCoroutineExp3());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SwitchTo2DCoroutineExp1()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("Deactivating 3D Camera and activated 2D Camera");

        Debug.Log("Left Pose " + leftPose.trackingType);
        Debug.Log("Right Pose " + rightPose.trackingType);

        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName("");

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        while (!XRSettings.loadedDeviceName.Equals(""))
        {
            Debug.LogWarning("2D Waiting extra frame… loadedDevice is "+XRSettings.loadedDeviceName);
            yield return null;
        }

        // Not needed, since loading the None (`""`) device takes care of this.
        XRSettings.enabled = false;

        StartCoroutine(SwitchTo3DCoroutineExp1());

    }

    IEnumerator SwitchTo2DCoroutineExp2()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("Deactivating 3D Camera and activated 2D Camera");

        Debug.Log("Left Pose " + leftPose.trackingType);
        Debug.Log("Right Pose " + rightPose.trackingType);


        leftPose.enabled = false;
        rightPose.enabled = false;
        yield return null;
        yield return null;

        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName("");

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        while (!XRSettings.loadedDeviceName.Equals(""))
        {
            Debug.LogWarning("2D Waiting extra frame… loadedDevice is "+XRSettings.loadedDeviceName);
            yield return null;
        }

        // Not needed, since loading the None (`""`) device takes care of this.
        XRSettings.enabled = false;

        StartCoroutine(SwitchTo3DCoroutineExp2());

    }

    IEnumerator SwitchTo2DCoroutineExp3()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("Deactivating 3D Camera and activated 2D Camera");


        //Destroy(leftPose);
        //Destroy(rightPose);
        //yield return null;
        //yield return null;

        //Debug.Log("Destroyed TrackedPoseDriver components");

        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName("");

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        while (!XRSettings.loadedDeviceName.Equals(""))
        {
            Debug.LogWarning("2D Waiting extra frame… loadedDevice is " + XRSettings.loadedDeviceName);
            yield return null;
        }

        // Not needed, since loading the None (`""`) device takes care of this.
        XRSettings.enabled = false;

        StartCoroutine(SwitchTo3DCoroutineExp3());

    }


    /// <summary>
    /// This doesn't work if the Cardboard device isn't loaded before NONE
    /// </summary>
    /// <returns>The to3 DC oroutine.</returns>
    IEnumerator SwitchTo3DCoroutineExp1()
    {
        Debug.Log("Switching to 3D");
        yield return new WaitForSeconds(3);

        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        if (String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {

            XRSettings.LoadDeviceByName(desiredDevice);
            Debug.Log("Loaded device " + desiredDevice);

            //https://github.com/googlevr/gvr-unity-sdk/issues/492#issuecomment-285752385
            yield return null; // try also: yield return WaitForEndOfFrame();
            Debug.Log("Waited one cycle");

            while (!XRSettings.loadedDeviceName.Equals(desiredDevice))
            {
                Debug.LogWarning("3D Waiting extra frame… loadedDevice is " + XRSettings.loadedDeviceName);
                yield return null;
            }

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            //yield return null;

            // Now it's ok to enable VR mode.
            XRSettings.enabled = true;
            Debug.Log("Enabled XR");

            Debug.Log("Left Pose " + leftPose.trackingType);
            Debug.Log("Right Pose " + rightPose.trackingType);
            // Another desperate attempt
            //yield return null;
        }

    }

    IEnumerator SwitchTo3DCoroutineExp2()
    {
        Debug.Log("Switching to 3D");
        yield return new WaitForSeconds(3);

        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        if (String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {

            XRSettings.LoadDeviceByName(desiredDevice);
            Debug.Log("Loaded device " + desiredDevice);

            //https://github.com/googlevr/gvr-unity-sdk/issues/492#issuecomment-285752385
            yield return null; // try also: yield return WaitForEndOfFrame();
            Debug.Log("Waited one cycle");

            while (!XRSettings.loadedDeviceName.Equals(desiredDevice))
            {
                Debug.LogWarning("3D Waiting extra frame… loadedDevice is " + XRSettings.loadedDeviceName);
                yield return null;
            }

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            //yield return null;

            // Now it's ok to enable VR mode.
            XRSettings.enabled = true;

            // Now lets wait for a frame or two

            yield return null;
            yield return null;

            //and try to re-enable the TrackedPoseDriver
            Debug.Log("Enabled XR");
            leftPose.enabled = true;
            rightPose.enabled = true;

            yield return null;
            yield return null;

            Debug.Log("Left Pose " + leftPose.trackingType);
            Debug.Log("Right Pose " + rightPose.trackingType);
            // Another desperate attempt
            //yield return null;
        }

    }

    IEnumerator SwitchTo3DCoroutineExp3()
    {
        Debug.Log("Switching to 3D");
        yield return new WaitForSeconds(3);

        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        if (String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {

            XRSettings.LoadDeviceByName(desiredDevice);
            Debug.Log("Loaded device " + desiredDevice);

            //https://github.com/googlevr/gvr-unity-sdk/issues/492#issuecomment-285752385
            yield return null; // try also: yield return WaitForEndOfFrame();
            Debug.Log("Waited one cycle");

            while (!XRSettings.loadedDeviceName.Equals(desiredDevice))
            {
                Debug.LogWarning("3D Waiting extra frame… loadedDevice is " + XRSettings.loadedDeviceName);
                yield return null;
            }

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            //yield return null;

            // Now it's ok to enable VR mode.
            XRSettings.enabled = true;

            // Now lets wait for a frame or two

            yield return null;
            yield return null;

            //and try to re-create the TrackedPoseDriver
            Debug.Log("Enabled XR");
            leftPose = leftCamera.AddComponent<TrackedPoseDriver>();
            rightPose = rightCamera.AddComponent<TrackedPoseDriver>();

            Debug.Log("After creation...");
            Debug.Log("Left Pose " + leftPose.trackingType);
            Debug.Log("Right Pose " + rightPose.trackingType);

            leftPose.trackingType = TrackedPoseDriver.TrackingType.PositionOnly;
            rightPose.trackingType = TrackedPoseDriver.TrackingType.PositionOnly;
            leftCamera.transform.LookAt(Vector3.forward);
            rightCamera.transform.LookAt(Vector3.forward);

            yield return null;
            yield return null;

            Debug.Log("After reconfiguration...");
            Debug.Log("Left Pose " + leftPose.trackingType);
            Debug.Log("Right Pose " + rightPose.trackingType);
            // Another desperate attempt
            //yield return null;
        }

    }
}
