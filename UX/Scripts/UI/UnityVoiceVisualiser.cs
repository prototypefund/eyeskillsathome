using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EyeSkills
{
    /// <summary>
    /// Unity voice visualiser. This should work in both 2D and VR mode with the EyeSkills Camera Rig thanks to layers.
    /// It activates two regular game objects (on different layers works best for VR) and alters their horizontal position (fake depth)!
    /// If provided with cameras, it will also alter the scene background colour in time with the audio.. 
    /// Everything gets reset when we stop the visualisation.
    /// </summary>
    public class UnityVoiceVisualiser : MonoBehaviour, IVoiceVisualiser
    {
        private float[] audioSpectrum;
        public float movementSuppression = 2f; //divides movement extreme by this amount.
        public int movementWeighting = 2; //Slows down the movement
        public bool showVisualisationObjects;

        public GameObject leftObj, rightObj;
        public List<Camera> cameras;
        private List<Color> cameraBackgroundColours;
        private Coroutine coroutine;

        public static float spectrumValue { get; private set; }
        private AudioSource source;
        /// <summary>
        /// Two objects (one for each eye) which alter their position based on the audio source
        /// </summary>

        private float lastVal=0f,currentVal,t=0f,nextVal,modifiedVal;

        // singleton like access pattern
        public static UnityVoiceVisualiser instance = null;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }


        private void Hide()
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameraBackgroundColours != null)
                {
                    cameras[i].backgroundColor = cameraBackgroundColours[i];
                }
            }
            leftObj.SetActive(false);
            rightObj.SetActive(false);
            StopCoroutine(coroutine);
        }

        private void Show()
        {
            cameraBackgroundColours = new List<Color>();
            foreach (Camera cam in cameras)
            {
                cameraBackgroundColours.Add(cam.backgroundColor);
            }
            if (showVisualisationObjects)
            {
                leftObj.SetActive(true);
                rightObj.SetActive(true);
            }
        }

        public void StopVisualisation(){
            Hide();
        }

        public void StartVisualisation()
        {
            Debug.Log("Started visualiser");
            //This may be a nasty dependency.
            source = AudioManager.instance.audioSource;
            Show();
            coroutine = StartCoroutine("DisplayElements");
        }

        private void AlterCameraBackgroundColour(List<Camera> cams, float colour){
            Color nextColour;
            foreach (Camera cam in cams){
                nextColour = cam.backgroundColor;
                float modifiedColour = (colour / 10 * 3)+0.6f;//normalise it in a 0.6-0.9f range
                nextColour.b = Mathf.Clamp(modifiedColour,0.6f,0.9f);
                cam.backgroundColor = nextColour; //We alter blue
                //Debug.Log("Modified background colour for " + cam + " to " + nextColour);
            }
        }

        /// <summary>
        /// Lets take a sample every five frames, and then smoothly move towards it, to reduce jerkiness?
        /// </summary>
        /// <returns>The elements.</returns>
        IEnumerator DisplayElements(){
            while (true)
            {
                if (source != null)
                { //TODO : Cache a check for source, canvas, and left and right objects?
                    audioSpectrum = new float[128];
                    source.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);
                    if ((audioSpectrum != null) && (audioSpectrum.Length > 0))
                    {
                        spectrumValue = audioSpectrum[0];

                        //Spectrum Value seems to be between 0 and 1
                        //Debug.Log("Spectrum value "+spectrumValue);

                        //currentVal = (lastVal + spectrumValue) / 2;
                        currentVal = spectrumValue;

                        //if (Mathf.Abs(currentVal - lastVal) > 0.2f)
                        //{

                        //  lastVal = currentVal;

                        //}

                    }
                }

                for (int i = 0; i < movementWeighting; i++)
                {

                    nextVal = Mathf.Lerp(lastVal, currentVal, t + 1/movementWeighting); //Linked as a % to the number of yields

                    AlterCameraBackgroundColour(cameras,nextVal*25);

                    if (showVisualisationObjects)
                    {
                        modifiedVal = nextVal * 50 / movementSuppression; //Enhance the visual aspect

                        leftObj.transform.localPosition = new Vector3(modifiedVal, 0, 0);
                        rightObj.transform.localPosition = new Vector3(-modifiedVal, 0, 0);
                    }

                    lastVal = currentVal;

                    t = t + movementWeighting;
                    yield return null;
                }
                t = 0;
            }
        }

    }
}