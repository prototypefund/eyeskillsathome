using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    [System.Serializable]
    public class ESAtomPhysicsRemovePhoneFromHeadset : ESAtomPhysicsDefaultVR
    {
        public IScreenOrientationManager screenOrientationManager;

        public ESAtomPhysicsRemovePhoneFromHeadset()
        {
            screenOrientationManager = GameObject.Find("EyeSkillsDayInit").GetComponent<IScreenOrientationManager>();
        }

        public void WaitForPhoneRemoval(){ //Need to use override so the BaseClass passes this version of AtomExpiry into the AudioManager for callback

            //Watch for the phone to be in landscape mode. When it is, we continue to the callback.
            //To do this we need to use a MonoBehaviour and pass it our completionCallback - PITA
            //We are forced to do this as we can't unit test MonoBehaviours in Unity.

            Screen.orientation = ScreenOrientation.AutoRotation;
            screenOrientationManager.CallbackWhenInPortrait(InPortrait);

        }

        public override void Initialise(ESTrainingAtom atom, Action<bool> _initCallback)
        {
            base.Initialise(atom, _initCallback);
        }

        public void InPortrait(){
            Screen.orientation = ScreenOrientation.Portrait;
            audioManager.StopAudioOnly();
            MultiCameraController.instance.SwitchTo2D(In2D);
        }

        public void In2D(bool success){
            molecule.container.SetActive(false);
            completionCallback("");
        }
    
        public override void Start(Action<string> _completionCallback)
        {

            ActivateUIMolecule();
            if (atom.image !=null ){
                SetDefaultVRImage(molecule, atom.image);
            }

            if (atom.audioFile !=null) {
                audioManager.Say(atom.audioFile);
            }

            base.Start(_completionCallback);
            WaitForPhoneRemoval();
        }

    }
}