using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

namespace EyeSkills
{
    public class ESAtomPhysicsVoiceVisualisationVR : ESAtomPhysicsDefaultVR
    {


        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {
         
            base.Initialise(_atom, _initCallback);

        }

        public override void Start(Action<String> _completionCallback)
        {
            base.Start(_completionCallback);

            ActivateUIMolecule(); // The particle field is implicit. We don't actually guide it by voice analysis for the moment.

            Debug.Log("Voice Visualiser = Making the camera look forward by default");
            cameraRig.ImmediatelyLookToOrigin();

            completionCallback = _completionCallback;

            //Say the audio file and when finished, stop the visualisation
            audioManager.Say(atom.audioFile,delegate() { AtomExpiry(); });
        }

        public void AtomExpiry()
        {
            //Stop the visualisation
            DeActivateUIMolecule();
            completionCallback("");

        }
    }

}