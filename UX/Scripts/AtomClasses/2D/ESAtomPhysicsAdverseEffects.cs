using System;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    public class ESAtomPhysicsAdverseEffects : ESTrainingDefaultPhysics
    {

        public class AdverseEffectsData : ConfigBase
        {
            public float isSeeingDouble;
            public float hasHeadache;
            public float experienceValue;
            public string feedback;
        }

        public AdverseEffectsData data;

        public AudioSource audioSource;

        public new void AtomExpiry()
        {
            data = new AdverseEffectsData();
            data.isSeeingDouble = GetMolecularChild(molecule, "DoubleVisionSlider").GetComponent<Slider>().value;
            data.hasHeadache = GetMolecularChild(molecule, "HeadacheSlider").GetComponent<Slider>().value;
            data.experienceValue = GetMolecularChild(molecule, "ExperienceSlider").GetComponent<Slider>().value;
            data.feedback = GetMolecularChild(molecule, "Feedback").GetComponent<Text>().text;
            data.Save();
            audioSource.Stop(); //IMPROVE: Make this fade out gracefully?
            DeActivateUIMolecule();
            completionCallback(atom.nextStep[0]);

        }

        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {

            atom = _atom;

            audioManager = AudioManager.instance;

            _initCallback(true);
        }

        public override void Start(Action<string> _completionCallback)
        {
            Debug.Log("Setting screen orientation");
            Screen.orientation = ScreenOrientation.Portrait;
            Debug.Log("Screen orientation is "+ Screen.orientation);
            if (atom.molecule == "")
            {
                atom.molecule = "default";
            }
            //Fetch our default display element and it's parts
            molecule = GetMolecule(atom.molecule);
            //molecule.container.SetActive(true);
            ActivateUIMolecule();

            completionCallback = _completionCallback;
            audioSource = audioManager.Say(atom.audioFile);
            Button b = GetMolecularChild(molecule, "Next").GetComponent<Button>();
            b.onClick.AddListener(AtomExpiry);
        }
    }
}