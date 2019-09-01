using System;
using UnityEngine;

namespace EyeSkills
{
    public class ESAtomPhysicsDefaultVR : ESAtomPhysicsInterface
    {


        public AudioManager audioManager;
        public CoRoutineHelper coRoutineHelper;
        public MultiCameraController multiCameraController;
        public GameObject camera3D;
        public EyeSkillsCameraRig cameraRig;
        public Action<String> completionCallback;
        public AssetSwitcher assetSwitcher;
        public Action<bool> initCallback;


        public ESTrainingAtom atom;

        public ESTrainingAtom Atom
        {
            get
            {
                return atom;
            }

            set
            {
                atom = value;
            }
        }

        public UIMolecule molecule;

        /// <summary>
        /// Helper to sets the default image in the UIMolecule DefaultVR 
        /// </summary>
        /// <param name="i">The index.</param>
        public void SetDefaultVRImage(UIMolecule m, string image)
        {
            //MeshRenderer i = GetMolecularChild(m, "Image").GetComponent<MeshRenderer>();
            //Material material = i.materials[0];
            SpriteRenderer i = GetMolecularChild(m, "Image").GetComponent<SpriteRenderer>();
            if (image != "")
            {

                i.enabled = true;
                Sprite sprite = Resources.Load<Sprite>(image);
                if (sprite != null)
                {
                    i.sprite = sprite; //Load the sprite - this should work for each file.
                    //material.mainTexture = sprite.texture;
                }
                else
                {
                    Debug.Log("SetDefaultVRImage could not load sprite " + image);
                }

            }
            else
            {
                i.enabled = false;
                i.sprite = null;
            }

        }


        public GameObject GetMolecularChild(UIMolecule molecule, string id)
        {
            try
            {
                return (GameObject)molecule.children.Find(i => i.id == id).gameObject;
            }
            catch
            {
                Debug.Log("Could not load Molecular Child " + id);
                return null;
            }
        }

        private UIMolecule FindUIMolecule(string _molecule)
        {
            return GameObject.Find("UIMolecule-" + _molecule).GetComponent<UIMolecule>();
        }
        protected UIMolecule ActivateUIMolecule()
        {
            //Find the molecule and activate it - this only shows us the Sphere
            molecule = FindUIMolecule(atom.molecule);
            if (molecule != null)
            {
                molecule.container.SetActive(true);
                return molecule;
            }
            Debug.Log("ActivateUIMolecule could not find molecule.");
            return null;
        }

        protected void DeActivateUIMolecule()
        {
            molecule = FindUIMolecule(atom.molecule);
            if (molecule != null)
            {
                molecule.container.SetActive(false);
            }
            else
            {
                Debug.Log("DeActivateUIMolecule could not find molecule. Was it already deactivated?");
            }


        }

        public virtual void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {
            atom = _atom;

            initCallback = _initCallback;

            //Fix our screen orientation
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            //Get the audio manager
            audioManager = AudioManager.instance;

            //multiCameraController = GetMolecularChild(molecule, "MultiCamera").GetComponent<MultiCameraController>(); 
            multiCameraController = GameObject.Find("MultiCamera").GetComponent<MultiCameraController>();

            //Disable the 2D camera and enable the 3D camera
            multiCameraController.SwitchTo3D(InitPart2);
        }

        /// <summary>
        /// The second phase of initialisation. Driving to this approach because embedding a closure in SwitchTo3D causes 
        /// AndroidPlayer(ADB@127.0.0.1:34999) [EGL] Failed to create window surface: EGL_BAD_ALLOC: EGL failed to allocate resources for the requested operation.
        /// </summary>
        /// <param name="sucess">If set to <c>true</c> sucess.</param>
        public virtual void InitPart2(bool sucess)
        {
            //Get an explicit reference to the EyeSkillCameraRig
            cameraRig = multiCameraController.GetVRCameraRig();

            assetSwitcher = cameraRig.GetComponent<AssetSwitcher>();

            initCallback(true); //We know that the 3D switch has completed, and we are ready to continue to "Start"
        }

        public virtual void Start(Action<string> _completionCallback)
        {

            completionCallback = _completionCallback;
            //This allows us to set a mock for testing during the initialisation phase.
            if (coRoutineHelper == null)
            {
                coRoutineHelper = CoRoutineHelper.instance;
            }

            while (!cameraRig.isActiveAndEnabled)
            {
                Debug.Log("Waiting for camera rig to become active");
            }

        }
    }

}