using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    [System.Serializable]
    public class ESAtomPhysicsPlacePhoneInHeadset : ESTrainingDefaultPhysics
    {
        public IScreenOrientationManager screenOrientationManager;

        public ESAtomPhysicsPlacePhoneInHeadset()
        {
            screenOrientationManager = GameObject.Find("EyeSkillsDayInit").GetComponent<IScreenOrientationManager>();
        }

        public override void AtomExpiry(){ //Need to use override so the BaseClass passes this version of AtomExpiry into the AudioManager for callback

            //Watch for the phone to be in landscape mode. When it is, we continue to the callback.
            //To do this we need to use a MonoBehaviour and pass it our completionCallback - PITA
            //We are forced to do this as we can't unit test MonoBehaviours in Unity.
            Screen.orientation = ScreenOrientation.AutoRotation;
            screenOrientationManager.CallbackWhenInLandscapeLeft(OurCallback);

        }

        public void OurCallback(){
            molecule.container.SetActive(false);
            completionCallback("");
        }
    }
}