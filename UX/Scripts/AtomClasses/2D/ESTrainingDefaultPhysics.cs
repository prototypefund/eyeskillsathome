using System;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    //TODO: There should be an abstract class with many of the helper methods implemented and waiting, only waiting on an override of init or start.
    public class ESTrainingDefaultPhysics : ESAtomPhysicsInterface
    {
        //public AudioManager audioManager = new AudioManager();

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

        public IVoiceVisualiser voiceVisualiser;

        public ISubTitlesVisualiser subsVisualiser;

        public Action<String> completionCallback;

        public IAudioManager audioManager;

        public AudioSource source;

        public UIMolecule molecule;

        public GameObject image, titleText, hitKeyToContinue;

        public UIMolecule GetMolecule(string id)
        {
            GameObject go = GameObject.Find("UIMolecule-" + id);
            UIMolecule molecule = go.GetComponentInChildren<UIMolecule>();
            return molecule;
        }

        public GameObject GetMolecularChild(UIMolecule molecule, string id){
            try{
                return (GameObject) molecule.children.Find(i => i.id == id).gameObject;
            } catch {
                Debug.Log("Could not load Molecular Child " + id);
                return null;
            }
        }

        protected UIMolecule FindUIMolecule(string _molecule)
        {
            return GameObject.Find("UIMolecule-" + _molecule).GetComponent<UIMolecule>();
        }

        protected UIMolecule ActivateUIMolecule()
        {
            //Find the molecule and activate it - this only shows us the Sphere
            molecule = FindUIMolecule(atom.molecule);
            molecule.container.SetActive(true);
            return molecule;
        }

        protected void DeActivateUIMolecule()
        {
            molecule = FindUIMolecule(atom.molecule);
            molecule.container.SetActive(false);
        }

        /// <summary>
        /// Initialise this instance.  This allows us to overwrite default initialisation for unit testing with mocks.
        /// </summary>
        public virtual void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback){

            atom = _atom;

            audioManager = AudioManager.instance;

            if (atom.molecule==""){
                atom.molecule = "default";
            } 
            //Fetch our default display element and it's parts
            molecule = GetMolecule(atom.molecule);

            molecule.container.SetActive(true);

            image = GetMolecularChild(molecule, "Image");
            titleText = GetMolecularChild(molecule, "TitleText");
            hitKeyToContinue = GetMolecularChild(molecule, "HitKeyToContinue");


            //Fetch our visualisers - Singletons so we don't keep creating destroying unless necessary.
            //voiceVisualiser = UnityVoiceVisualiser.instance;
            //subsVisualiser = UnitySubTitlesVisualiser.instance;

            //Naiive but for now, ok.
            _initCallback(true);

        }

        public virtual void StartAudio(){
            if ((atom.audioFile!=null) && (atom.audioFile!="")) {
                source = audioManager.Say(atom.audioFile, AtomExpiry);
            }
        }

        public virtual void StartText(){
            //Add the text
            if ((titleText != null) && (titleText.GetComponent<Text>() != null))
            {
                titleText.GetComponent<Text>().text = atom.titleText;
            }
        }

        public virtual void StartVisuals(){
            if (image != null)
            {
                Sprite mySprite = Resources.Load<Sprite>(atom.image);

                Image component = image.GetComponent<Image>();

                component.sprite = mySprite;
            }
        }

        public virtual void Start(Action<String> _completionCallback)
        {


            Screen.orientation = ScreenOrientation.Portrait;

            completionCallback = _completionCallback;
            //Trigger the physics -> pass it what it needs to do it's job

            StartVisuals();

            StartText();

            StartAudio();

            //Visualise the text as subtitles - can be mocked for testing
            //subsVisualiser.Visualise(atom.subtitleText, source);


        }

        /// <summary>
        /// By default called when the audio clip has actually finished playing. This is only a default behaviour
        /// </summary>
        public virtual void AtomExpiry()
        {
            //By default, when the audio is over, lets just relinquish control again.
            if ((molecule) && (molecule.container))
            {
                molecule.container.SetActive(false);
            }
            completionCallback("");
        }


        public void setVoiceVisualiser(IVoiceVisualiser _voiceVisualiser)
        {
            voiceVisualiser = _voiceVisualiser;
        }

        public void setSubTitlesVisualiser(ISubTitlesVisualiser _subTitlesVisualiser)
        {
            subsVisualiser = _subTitlesVisualiser;
        }

    }
}