using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

namespace EyeSkills
{

    /// <summary>
    /// The 3D suppression test. 
    /// </summary>
    public class ESAtomPhysics3DSuppressionTest1 : ESAtomPhysicsDefaultVR
    {

        //Where we store our data
        public SuppressionTestsData data;
        public string selectedPerceptionImage;
        public float timeToWaitForHeadTiltCheck = 6f; //Wait maximum of 6 seconds for participant to pass tile checks
        public int maxNumberOfTiltAttempts = 2;

        protected float blinkerExperienceInSeconds = 6f;
        protected ConflictZoneModel conflictController;

        private bool blinkersActive = false;
        /// <summary>
        /// The Blinker Pause. Pause between toggling blinkers on/off for this many seconds
        /// </summary>
        private float blinkerPause = 5f;
        private bool blinkersOn = false;

        private List<Coroutine> coroutinesToShutdown = new List<Coroutine>(); 

        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {
            data = new SuppressionTestsData();
            data.Load();

            base.Initialise(_atom, _initCallback);

        }

        /// <summary>
        /// Coordinating what the participant experiences on the basis of the atom's supplied physicsState.
        /// Doing things this way should actually simplify things like certifying the app is in known states.
        /// </summary>
        /// <param name="_completionCallback">Completion callback.</param>
        public override void Start(Action<String> _completionCallback)
        {
            base.Start(_completionCallback);

            Debug.Log("Making the camera look forward by default, without following headset rotation.");
            cameraRig.ImmediatelyLookToOrigin();
            cameraRig.SetLeftEyePositionOnly();
            cameraRig.SetRightEyePositionOnly();

            Debug.Log("UIMolecule disabled by default.");
            DeActivateUIMolecule();

            completionCallback = _completionCallback;


            if (atom.physicsState == 1) //Let the participant know what is coming.
            {
                ActivateUIMolecule();
                audioManager.Say(atom.audioFile, delegate ()
                                {
                                    AtomExpiry(0, false, "Physics state 1 - After audio expires");
                                });
            }
            else if (atom.physicsState == 2)  //Present them with a conflict image
            {
                audioManager.Say(atom.audioFile, StopConflictEnvironment, 5);
                StartConflictEnvironment();
            }
            else if (atom.physicsState == 3)  //How did they perceive the image?  The gallery.
            {
                //Allow the user to look around
                cameraRig.SetLeftEyeRotationAndPosition();
                cameraRig.SetRightEyeRotationAndPosition();

                audioManager.Say(atom.audioFile);
                StartPerceptionGallery(SelectedPerceptionImage);
            }
            else if (atom.physicsState == 5) //Now find out if blinkers can remove conflict for them
            {
                StartConflictEnvironment();
                conflictController.OutOfConflict();

                blinkerExperienceInSeconds = 6; //Allow some time after the audio has finished to keep observing
                audioManager.Say(atom.audioFile, delegate ()
                {
                    coRoutineHelper.BeginCoroutine(TimeBlinkerExperience());
                });
            }
            else if (atom.physicsState == 6) //Did the blinkers remove conflict for them?
            {
                ReviewBlinkerExperience();
            }
            else if (atom.physicsState == 7) //TODO: Day1 - OBSOLETE?
            {
                UIMolecule m = ActivateUIMolecule();
                SetDefaultVRImage(m, atom.image);
                ActivateContinueButton(false);
                ExplainBlinkerExperience();
            }
            else if (atom.physicsState == 8)  //Day2 - Present a conflict environment for ten seconds after audio ends : Similar to state 2
            {
                audioManager.Say(atom.audioFile, StopConflictEnvironment, 10);
                StartConflictEnvironment();
            }
            else if (atom.physicsState == 9)  //Day2 - Show the static Blinker
            {
                Debug.Log("Explaining about Blinkers, after audio ends, we start blinking...");
                //Manually start the conflict environment.
                StartConflictEnvironment();
                conflictController.OutOfConflict();

                blinkerExperienceInSeconds = 10;
                audioManager.Say(atom.audioFile, delegate ()
                {
                    coRoutineHelper.BeginCoroutine(TimeBlinkerExperience());
                });
            }
            else if (atom.physicsState == 10) //Now show the alternating Blinker toggling
            {
                blinkerExperienceInSeconds = 30;
                StartBlinkerEnvironment();
                audioManager.Say(atom.audioFile);
            }
            else if (atom.physicsState == 11)  //Present them with a conflict image
            {
                audioManager.Say(atom.audioFile, StopConflictEnvironment);
                StartConflictEnvironment();
            }
            else if (atom.physicsState == 12)
            { //Test they can tilt their heads upwards
                //Get hold of the HeadsetVRInput

                DemoSuppression();

                if (data.numberOfTiltAttempts >= maxNumberOfTiltAttempts)
                {
                    data.numberOfTiltAttempts = 0; //A hack for testing. Shouldn't affect production
                    data.Save();
                }

                audioManager.Say(atom.audioFile, delegate(){
                    DetectVerticalHeadTilt(EndUpwardTiltCheck, timeToWaitForHeadTiltCheck);//We pass "0" to the EndUpwardTiltCheck to signify we ran out of time.
                });

            }
            else if (atom.physicsState == 13)
            {
                DemoSuppression();

                if (data.numberOfTiltAttempts >= maxNumberOfTiltAttempts)
                {
                    data.numberOfTiltAttempts = 0; //A hack for testing. Shouldn't affect production
                    data.Save();
                }

                //Test they can tilt their heads downwards
                audioManager.Say(atom.audioFile, delegate () {
                    DetectVerticalHeadTilt(EndDownwardTiltCheck, timeToWaitForHeadTiltCheck);
                });

            }
            else if (atom.physicsState == 14) //Decide whether to repeat the tilt tests or not
            {
                if ((data.couldTiltUpward==false) || (data.couldTiltDownward==false)){
                    //Debug.Log("One of the tilts failed " + data.couldTiltUpward + data.couldTiltDownward);
                    if (data.numberOfTiltAttempts == (maxNumberOfTiltAttempts - 1)){ //We've already tried enough times and still don't have adequate tilt
                        //Debug.Log("Too many attempts");
                        data.numberOfTiltAttempts = maxNumberOfTiltAttempts;
                        AtomExpiry(0, true, "Failed to tilt head adequately");
                    } else 
                    { //Let's have another go
                        AtomExpiry(1, true, "Trying another round of head tilt tests");
                    }                   
                } else { //They must have succeeded
                    AtomExpiry(2, true, "Head tilt tests succeeded");
                }
            }
            else if (atom.physicsState == 15) //Time to detect the suppression ratio
            {
                //Enable head rotation
                cameraRig.SetLeftEyeRotationAndPosition();
                cameraRig.SetRightEyeRotationAndPosition();

                //Get hold of the HeadsetVRInput
                EyeSkillsVRHeadsetInput ratioController = GameObject.Find("Head").GetComponent<EyeSkillsVRHeadsetInput>();

                //Get hold of the conflict objects
                StartConflictEnvironment(); //Sets conflictZoneModel

                SelectionIndicatorScaler indicator = cameraRig.GetComponentInChildren<SelectionIndicatorScaler>();


                //Get hold of our Stillness Selector - adding it to the Head object
                EyeSkillsVRHeadsetSelectByStillness stillness = EyeSkillsVRHeadsetSelectByStillness.instance;

                audioManager.Say(atom.audioFile, delegate ()
                {
                    //Start a couroutine which manages the brightness control and calls a finishing function with a suppression ratio
                    Coroutine c = coRoutineHelper.BeginCoroutine(IdentifyBinocularSuppressionRatio(
                        ratioController,
                        conflictController,
                        cameraRig,
                        stillness,
                        indicator,
                        delegate (float suppressionRatio)
                        {
                            data.binocularSuppressionRatio = suppressionRatio;
                            data.Save();
                            AtomExpiry(0, false, "SuppressionRatio captured");
                        },
                        6f
                    ));
                    coroutinesToShutdown.Add(c);
                });
            }
            else if (atom.physicsState == 16)
            {
                ActivateUIMolecule();
                //Allow the user to look around
                cameraRig.SetLeftEyeRotationAndPosition();
                cameraRig.SetRightEyeRotationAndPosition();

                audioManager.Say(atom.audioFile);
                StartPerceptionGallery(SelectedTolerancePerceptionImage); 
            }
        }


        void DemoSuppression(){
            //This is a new trick to shut down coroutines in AtomExpiry - so we can have multiple monitoring async coroutines working
            EyeSkillsVRHeadsetInput ratioController = GameObject.Find("Head").GetComponent<EyeSkillsVRHeadsetInput>();

            StartConflictEnvironment();

            coroutinesToShutdown.Add(coRoutineHelper.BeginCoroutine(DemonstrateBinocularSuppressionRatio(ratioController, conflictController, cameraRig)));

        }

        IEnumerator DemonstrateBinocularSuppressionRatio(EyeSkillsVRHeadsetInput ratioController,
                                                      ConflictZoneModel model,
                                                      EyeSkillsCameraRig cameraRig)
        {
            float brightnessRatio;

            while (true)
            {
                //Update the luminance ratio for each eye
                brightnessRatio = Mathf.Clamp(ratioController.getVerticalDirection(), -1, 1);

                cameraRig.SetBinocularSuppressionRatio(brightnessRatio);

                yield return null;

            }
        }

        /// <summary>
        /// Identifies the binocular suppression ratio.
        /// </summary>
        /// <returns>The binocular suppression ratio.</returns>
        /// <param name="ratioController">Ratio controller. Where we get our signal from to alter the ratio - e.g. tilt angle of head</param>
        /// <param name="model">Model. The model that provides us the conflict images.</param>
        /// <param name="cameraRig">Camera rig. The cameras whose relative brightness we wish to alter.</param>
        /// <param name="stillness">Stillness. Where we get information from about whether or not the headset is still. Might have been better to pass this headset specific mechanism to the VRHeadsetInput, which could have implemented a generic interface for reporting "stillness"</param>
        /// <param name="indicator">Indicator. The element which informs the user about the progress of the stillness based selection</param>
        /// <param name="NextStep">Next step.</param>
        /// <param name="secondsOfStillnessForSelect">Seconds of stillness for select.</param>
        IEnumerator IdentifyBinocularSuppressionRatio(EyeSkillsVRHeadsetInput ratioController,
                                                      ConflictZoneModel model,
                                                      EyeSkillsCameraRig cameraRig,
                                                      EyeSkillsVRHeadsetSelectByStillness stillness,
                                                      SelectionIndicatorScaler indicator,
                                                      Action<float> NextStep, 
                                                      float secondsOfStillnessForSelect)
        {
            float suppressionRatio = 0;
            float still = 0;
            float brightnessRatio;

            stillness.StartTracking();

            //TODO : We probably need a maximum timeout for if the headset never appears to settle!
            still = stillness.getTimeStill();
            while (still < secondsOfStillnessForSelect)
            {
                //Update the luminance ratio for each eye
                brightnessRatio = Mathf.Clamp(ratioController.getVerticalDirection(), -1, 1);

                cameraRig.SetBinocularSuppressionRatio(brightnessRatio);

                //redraw the indicator and play a rising tone?!?
                float percentage = (still / secondsOfStillnessForSelect);
                //Debug.Log("Percentage still " + percentage);
                indicator.SetIndicatorPercentage(percentage);
                yield return null;
                still = stillness.getTimeStill();

                //Debug.Log("ratio : "+ brightnessRatio + " %:" + percentage);
            }

            stillness.StopTracking();
            indicator.Reset();
            StopConflictEnvironment();
            NextStep(suppressionRatio);
        }


        IEnumerator TimeTiltExperience(float waitTime, Action<int> NextStep)
        {
            yield return new WaitForSeconds(waitTime);
            NextStep(0);
        }

        private void EndUpwardTiltCheck(int tilt){
            //For the timeout process we therefore need to extend the TiltWatcher in EyeSkillsVRHeadsetInput to remember it's coroutine, so we can call a cancellation function if we timeout- otherwise we have a memory leak
            if (tilt<=0){ //The participant didn't tilt their head far enough (or the gyro on the phone is screwed? - launch other diagnostics?), or looked in the wrong direction
                data.couldTiltUpward = false; //We need to record the number of tile attempts later once we've tried both up and down
            } else if (tilt>0) //We found an upward tilt
            {
                data.couldTiltUpward = true;
            }

            if (tilt==0){ //We came here because of a timeout
                CancelVerticalHeadTiltDetection();
            }
            data.Save();
            AtomExpiry(0,false,""); //Now continue
        }

        private void EndDownwardTiltCheck(int tilt)
        {
            if (tilt >= 0)
            { 
                data.couldTiltDownward = false; 
            }
            else if (tilt < 0) 
            {
                data.couldTiltDownward = true;
            }

            if (tilt == 0)
            { //We came here because of a timeout
                CancelVerticalHeadTiltDetection();
            }
            data.Save();
            AtomExpiry(0, false, ""); 
        }

        /// <summary>
        /// Toggle the blinkers on and off
        /// </summary>
        protected void ToggleBlinkers()
        {
            if (blinkersOn)
            {
                conflictController.IntoConflict();
            }
            else
            {
                conflictController.OutOfConflict();
            }
            blinkersOn = !blinkersOn;
        }

        IEnumerator TimeBlinkerToggling()
        {
            while (blinkersActive)
            {
                yield return new WaitForSeconds(blinkerPause);
                ToggleBlinkers();
            }
        }

        /// <summary>
        /// We reuse the Conflict Environment, with a longer audio file, and after we set it running, we start a coroutine to alternate between blinkers and non-blinkers
        /// </summary>
        protected void StartBlinkerEnvironment()
        {
            Debug.Log("StartBlinkerEnvironment");

            StartConflictEnvironment();
            //Start the Toggling. Now we start the blinkers switching on and off while the audio track explains what's happening.
            Debug.Log("StartCoroutinesEnvironment");
            blinkersActive = true;

            coRoutineHelper.BeginCoroutine(TimeBlinkerToggling());
            //Start another co-routine so that after the toggling time we call StopConfilctEnvironment().
            coRoutineHelper.BeginCoroutine(TimeBlinkerExperience());

        }

        public void ExplainBlinkerExperience()
        {
            audioManager.Say(atom.audioFile, delegate () { AtomExpiry(0, false, "Expire of Explain Blinker Experience."); });
        }

        public void ReviewBlinkerExperience()
        {
            //Now we want to check what the person experienced
            UIMolecule m = ActivateUIMolecule();
            SetDefaultVRImage(m, atom.image);
            //audioManager.Say(atom.audioFile, DetectBlinkerSelection);
            audioManager.Say(atom.audioFile);
            DetectVerticalHeadTilt(EndBlinkerSelection,60f); //Wait 60 seconds before timing out.
        }

        public void DetectVerticalHeadTilt(Action<int> NextStep, float timeToWaitForHeadTiltCheck)
        { //Lets move on to the next step depending on the vertical tilt of the head.

            EyeSkillsVRHeadsetInput headInput = GameObject.Find("Head").GetComponent<EyeSkillsVRHeadsetInput>();

            headInput.TiltedVerticallyBeyond(15f, -15f, NextStep, timeToWaitForHeadTiltCheck);


        }

        public void CancelVerticalHeadTiltDetection(){
            EyeSkillsVRHeadsetInput headInput = GameObject.Find("Head").GetComponent<EyeSkillsVRHeadsetInput>();

            headInput.CancelVerticalHeadTiltDetection();
        }

        public void EndBlinkerSelection(int tilt)
        {
            //int index = -1;
            if (tilt > 0)
            {
                data.blinkersRemovedConflict = true;
                //index = 0;
            }
            else
            {
                data.blinkersRemovedConflict = false; //Will record false if a tilt of 0 is returned through a timeout.
                //index = 1;
            }
            data.Save();
            DeActivateUIMolecule();
            //completionCallback(atom.nextStep[index]); //We no longer branch here.
            completionCallback("");
        }

        protected void ActivateContinueButton(bool activate)
        {

            GameObject go = GameObject.Find("ContinueButtonFace");
            if (go != null) go.SetActive(activate);
        }

        protected ConflictZoneModel StartConflictEnvironment()
        {

            //Debug.Log("Start Conflict Environment");
            //Set luminance to defaults 

            cameraRig.EqualiseCameraBrightness();

            assetSwitcher.SwitchAssetsToNamed("asset-test"); // This should show our wavy lines

            //Setup the conflict zones - Activate them and asset swap basically.
            conflictController = cameraRig.gameObject.GetComponent<ConflictZoneModel>();

            conflictController.Show(true);

            return conflictController;

        }

        protected void StopConflictEnvironment()
        {
            //Debug.Log("Stopping conflict environment");

            conflictController = cameraRig.gameObject.GetComponent<ConflictZoneModel>();

            conflictController.IntoConflict();

            conflictController.Show(false);

            AtomExpiry(0, false, "-> Called from StopConflictEnvironment. Physics state 2. After audio expires.");
        }

        protected void WireUpPerceptionGallery(GameObject container, Action<string> SelectionFunction)
        {
            //Iterate through the children of the container
            VRInteractiveItem vrit;
            foreach (Transform g in container.transform.GetComponentsInChildren<Transform>())
            {
                vrit = g.GetComponent<VRInteractiveItem>();
                if (vrit)
                {
                    vrit.OnHoverSelected += SelectionFunction;
                    vrit.OnOver += delegate (string str) { cameraRig.ActiveBeam(); };
                    vrit.OnOut += delegate (string str) { cameraRig.PassiveBeam(); };
                }
            }

            //Set them up to have a callback with their own GameObject name.
        }

        /// <summary>
        /// A callback from the selected perception gallery image
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void SelectedPerceptionImage(string id)
        {
            Debug.Log("Selected an image from the gallery called " + id);
            cameraRig.HideBeam();
            selectedPerceptionImage = id;
            data.chosenPerception = selectedPerceptionImage;
            data.Save();

            if ((id == "LeftSuppressed") || (id == "RightSuppressed"))
            {
                AtomExpiry(1, true, "Expiring as Classic Left/Right Suppressed selected.");
            }
            else if ((id == "AlternatingLeft") || (id == "AlternatingRight"))
            {
                AtomExpiry(1, true, "Expiring as Alternating Left/Right Suppressed selected.");
            }
            else
            {
                AtomExpiry(1, true, "Expiring as regular fusion detected.");
            }
        }

        /// <summary>
        /// A callback from the selected perception gallery image when setting binocular suppression ratio
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void SelectedTolerancePerceptionImage(string id)
        {
            Debug.Log("Selected an image from the gallery called " + id);
            cameraRig.HideBeam();
            selectedPerceptionImage = id;
            data.chosenTolerancePerception = selectedPerceptionImage;

            if ((id == "LeftSuppressed") || (id == "RightSuppressed"))
            {
                data.binocularTolerance = false;
            }
            else if ((id == "AlternatingLeft") || (id == "AlternatingRight"))
            {
                data.binocularTolerance = false;
            }
            else
            {
                data.binocularTolerance = true;
            }

            data.Save();

            //May want to make a navigation choice here- but for now we pass that on to the summary controller
            AtomExpiry(0, true, "SelectedTolerancePerceptionImage");

        }

        protected void StartPerceptionGallery(Action<string> SelectionFunction)
        {
            molecule = GameObject.Find("UIMolecule-" + atom.molecule).GetComponent<UIMolecule>();
            molecule.container.SetActive(true);

            cameraRig.ShowBeam();
            //Now we need to start the monitoring and UI for selection which eventually calls StopPerceptionGallery
            //At this point, we want the raycasted parts to display something when they're selected, and after a certain amount of time, those parts call back in here to indicate which has been selected.
            //Need to create and EyeSkillsVRInteractiveItem which is more simplified... public variables with Action to set with the functions to call back. This means we need to iterate through the parts in the gallery.
            //At that point we can make decisions about how to proceed to the next scene.  We're actually getting there.  There's not THAT much to do to get the first steps ready.
            WireUpPerceptionGallery(molecule.container, SelectionFunction);

        }

        //protected void StopPerceptionGallery()
        //{
        //    molecule = GameObject.Find("UIMolecule-" + atom.molecule).GetComponent<UIMolecule>();
        //    molecule.container.SetActive(false);
        //    IntroExpiry();
        //}

        IEnumerator TimeBlinkerExperience()
        {
            yield return new WaitForSeconds(blinkerExperienceInSeconds);
            EndBlinkerExperience();
        }

        public void EndBlinkerExperience()
        {
            blinkersActive = false; //Causes the TimeBlinkerToggling to stop
            StopConflictEnvironment();
        }

        public void AtomExpiry(int index, bool deactivateUI, string debugMessage)
        {
            Debug.Log("Atom Expiry(x) in 3D Suppression. Message : " + debugMessage);
            if (deactivateUI)
            {
                DeActivateUIMolecule();
            }

            if (coroutinesToShutdown.Count>0){
                Debug.Log("Shutting down excess coroutines");
                for (int i = 0; i < coroutinesToShutdown.Count; i++){
                    coRoutineHelper.EndCoroutine(coroutinesToShutdown[i]);
                }
            }

            Debug.Log("Atom Expiry - attempting to move to index " + index);

            if ((atom.nextStep != null) && (atom.nextStep.Count > index) && (atom.nextStep[index] != null))
            {
                completionCallback(atom.nextStep[index]);
            }
            else
            {
                completionCallback("");
            }
        }

    }
}
