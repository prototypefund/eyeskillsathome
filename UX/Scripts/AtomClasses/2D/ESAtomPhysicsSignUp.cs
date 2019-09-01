using System;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    public class ESAtomPhysicsSignUp : ESTrainingDefaultPhysics
    {

        public class SignUpData : ConfigBase
        {
            public bool interestedInCrowdfunding;
            public bool interestedInDonating;
        }

        public SignUpData data;
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
            audioSource.Stop(); //IMPROVE: Make this fade out gracefully?
            completionCallback(atom.nextStep[0]);

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

        public void SayBye(string url)
        {
            audioSource.Stop();
            leaveURL = url;
            audioManager.Say("LeaveApp",LeaveApp);
        }

        public void LeaveApp(){
            Application.OpenURL(leaveURL);
        }

        public void LeaveAppCrowdfund(){

            data.interestedInCrowdfunding = true;
            data.Save();
            SayBye("https://eyeskills.com/crowdfund");
        }

        public void LeaveAppDonate()
        {
            data.interestedInDonating = true;
            data.Save();
            SayBye("https://eyeskills.org/donate");
        }

        public void ClearHighlighting(){
            GetMolecularChild(molecule, "Crowdfund").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            GetMolecularChild(molecule, "Donate").GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }

        public override void Start(Action<string> _completionCallback)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            data = new SignUpData();
            completionCallback = _completionCallback;

            if (atom.physicsState==0){
                ClearHighlighting();
                GetMolecularChild(molecule, "Crowdfund").GetComponent<Image>().color = new Color(1f,1f,0f);
                audioSource = audioManager.Say(atom.audioFile,AtomExpiry);
            } else if (atom.physicsState==1){
                ClearHighlighting();
                GetMolecularChild(molecule, "Donate").GetComponent<Image>().color = new Color(1f, 1f, 0f);
                audioSource = audioManager.Say(atom.audioFile,ClearHighlighting);
            }


            GetMolecularChild(molecule, "Quit").GetComponent<Button>().onClick.AddListener(QuitApp);
            GetMolecularChild(molecule, "Crowdfund").GetComponent<Button>().onClick.AddListener(LeaveAppCrowdfund);
            GetMolecularChild(molecule, "Donate").GetComponent<Button>().onClick.AddListener(LeaveAppDonate);
        }
    }
}