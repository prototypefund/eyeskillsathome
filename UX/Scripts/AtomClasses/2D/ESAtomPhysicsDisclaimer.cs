using System;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    public class ESAtomPhysicsDisclaimer : ESTrainingDefaultPhysics
    {

        private string leaveURL;

        public AudioSource audioSource;

        public new void AtomExpiry()
        {
            //data = new SignUpData();
            //data.isSeeingDouble = GetMolecularChild(molecule, "DoubleVisionSlider").GetComponent<Slider>().value;
            //data.hasHeadache = GetMolecularChild(molecule, "HeadacheSlider").GetComponent<Slider>().value;
            //data.experienceValue = GetMolecularChild(molecule, "ExperienceSlider").GetComponent<Slider>().value;
            //data.feedback = GetMolecularChild(molecule, "Feedback").GetComponent<Text>().text;
            //data.Save();
            DeActivateUIMolecule();
            audioSource.Stop(); //IMPROVE: Make this fade out gracefully?
            completionCallback("");

        }

        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {

            atom = _atom;

            audioManager = AudioManager.instance;

            if (atom.molecule == "")
            {
                atom.molecule = "default";
            }
            //Fetch our default display element and it's parts
            molecule = GetMolecule(atom.molecule);

            molecule.container.SetActive(true);

            _initCallback(true);

        }

        public void QuitApp(){
            audioSource.Stop();
            audioManager.Say("ByeBye",delegate (){
                Application.Quit();
            }); 
        }

        public void CheckAgree(){
            Toggle toggle = GetMolecularChild(molecule, "Over18").GetComponent<Toggle>();
            if (toggle.isOn){
                AtomExpiry();
            } else {
                GetMolecularChild(molecule, "ToggleHighlight").GetComponent<Text>().color = new Color(1.0f, 0f, 0f, 0.5f);
                //ColorBlock cb = toggle.colors;
                //cb.normalColor = new Color(1f, 0f, 0f,0f);

            }

        }

        public override void Start(Action<string> _completionCallback)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            completionCallback = _completionCallback;

            audioSource = audioManager.Say(atom.audioFile);
            GameObject agree = GetMolecularChild(molecule, "Agree");
            agree.GetComponent<Button>().onClick.AddListener(CheckAgree);
            GameObject notAgree = GetMolecularChild(molecule, "NotAgree");
            notAgree.GetComponent<Button>().onClick.AddListener(QuitApp);

        }
    }
}