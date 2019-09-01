using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace EyeSkills
{
    public class ESAtomPhysicsid2Test : ESTrainingDefaultPhysics, ESAtomPhysicsInterface
    {
        private GameObject go;

        //public AudioManager audioManager = new AudioManager();

        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {
            MultiCameraController.instance.SwitchTo3D(delegate(bool success){
                _initCallback(true);
            });

        }

        //This time we don't want the visualisation
        public override void Start(Action<String> _completionCallback)
        {


            //Maybe set a variable to disable voice visualisation? and then call parent start

            completionCallback = _completionCallback;
            //We still want to talk though
            source = audioManager.Say(atom.audioFile, AudioExpiry);

            //And now we want to place/find/expose a UI element on the screen
            //and listen for it being clicked... (call AtomExpiry());

            go = GameObject.Find("UIMolecule-id2");
            go.GetComponent<UIMolecule>().container.SetActive(true);
            Button b = go.GetComponentInChildren<Button>();
            b.onClick.AddListener(ButtonClick);
        }

        public void ButtonClick(){
            //go.SetActive(false);
            go.GetComponent<UIMolecule>().container.SetActive(false);
            //XRSettings.enabled = false;
            MultiCameraController.instance.SwitchTo2D(AtomExpiry);
        }


        public void AudioExpiry(){
            //We don't want to quit until the UI element has been touched.
            MultiCameraController.instance.SwitchTo2D(AtomExpiry);
        }

        public void AtomExpiry(bool success){
            AtomExpiry();
        }
    }
}