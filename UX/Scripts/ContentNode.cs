using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using EyeSkills;

namespace EyeSkills {

    /// <summary>
    /// Content node. This manages which atoms to load when, and finds and prepares their physics (the context in which they are interpreted).
    /// </summary>
    public class ContentNode 
    {

        public Canvas targetCanvas;

        /// <summary>
        /// Quits to menu, looking up the last restartable atom ID in the current content list
        /// </summary>
        public void QuitToSceneSelector()
        {
            string restartableID = "";
            //Lookup the last restore point working back from the currentNodeID
            for (int i = FindAtomIndex(currentAtom,atoms); i >= 0; i--){
                if (atoms[i].restartable){
                    restartableID = atoms[i].id;
                }
            }
            if ((SceneSelector.instance!=null))
            SceneSelector.instance.ExitToMenu(SceneManager.GetActiveScene().name, restartableID);
        }

        /// <summary>
        /// The current node identifier.  Ideally this would store a list of all played nodes so we could drop back to a sensible point in the case of a missing ID, but this doesn't play will with Serialisation (JSON needed etc.). For speed, KISS.
        /// </summary>
        public string currentNodeID;
        public bool currentNodeCompleted;
        public List<ESTrainingAtom> atoms = new List<ESTrainingAtom>();

        public ESTrainingAtom currentAtom;

        ESAtomPhysicsInterface physics;

        private int FindAtomIndex(ESTrainingAtom match, List<ESTrainingAtom> searchInAtoms)
        {
            int i = 0;
            foreach (ESTrainingAtom atom in searchInAtoms)
            {
                if (atom == match) return i;
                i++;
            }
            return -1;
        }

        private ESTrainingAtom FindAtom(string id, List<ESTrainingAtom> searchInAtoms)
        {

            foreach (ESTrainingAtom atom in searchInAtoms)
            {
                if (atom.id == id) return atom;
            }

            return null;
        }

        public ESTrainingAtom FindNextAtom(ESTrainingAtom searchFromAtom, List<ESTrainingAtom> searchInAtoms)
        {
            int i = -1;
            if (searchFromAtom != null)
            {
                i = FindAtomIndex(searchFromAtom, searchInAtoms);
            }
            i++;
            while (i <= atoms.Count - 1)
            {
                if (atoms[i].disableAtom != true)
                { //ignore disabled atoms
                    return atoms[i];
                }
                else
                {
                    i++;
                }
            }
            return null;
        }

        public ESTrainingAtom FindNextAtom()
        {
            return FindNextAtom(currentAtom, atoms);
        }

        /// <summary>
        /// Begin from the first atom.
        /// </summary>
        /// <param name="_atoms">Atoms.</param>
        private void InitaliseFreshStart(List<ESTrainingAtom> _atoms)
        {

            currentAtom = FindNextAtom(null, _atoms);
            currentNodeID = currentAtom.id;
            currentNodeCompleted = false;
        }

        /// <summary>
        /// Initialises the atoms. Where were we last time? Do we want to start from the beginning?
        /// </summary>
        public void InitialiseAtoms(List<ESTrainingAtom> _atoms)
        {

            atoms = _atoms; //TODO : May want to explicitly set this e.g. from JSON before calling this method

            if (PlayerPrefs.HasKey("currentNodeID"))
            { //Are there stored settings already?

                currentNodeID = PlayerPrefs.GetString("currentNodeID");
                currentNodeCompleted = (PlayerPrefs.GetInt("currentNodeCompleted") != 0);
                currentAtom = FindAtom(currentNodeID, _atoms);
                if (currentAtom == null)
                {
                    InitaliseFreshStart(_atoms);
                }
                //TODO: Now we need to check that this currentNodeID exists, and if not, rollback the currentNodeID to the 
            }
            else
            { //Lets initialise as this must be a first run-through
                InitaliseFreshStart(_atoms);
            }

        }

        /// <summary>
        /// Look for any custom physics matching the paragraph - we might want a physics type rather than id matching
        /// </summary>
        /// <returns>The physics.</returns>
        /// <param name="atom">Atom.</param>
        public ESAtomPhysicsInterface LocatePhysics(ESTrainingAtom atom)
        {

            Type t;
            try
            {
                //Look for custom physics... 
                string classname = "EyeSkills.ESAtomPhysics" + atom.preferredPhysics;
                t = Type.GetType(classname, true);
                physics = (ESAtomPhysicsInterface)Activator.CreateInstance(t);

            }
            catch (TypeLoadException)
            {
                //This is expected behaviour
                if (atom.preferredPhysics != "")
                {
                    Debug.Log("Could not load physics [" + atom.preferredPhysics + "]");
                }
                Debug.Log("Loading default physics");
                physics = new ESTrainingDefaultPhysics();
            }

            return physics;


        }

        /// <summary>
        /// Spins the atom. Asks the user whether to carry on if they wish, or start from the first atom of this day.
        /// 
        /// </summary>
        /// <param name="atom">Atom.</param>
        public void Play(ESTrainingAtom atom)
        {

            //TODO: If the atom isn't the very first, we should ask if the user wishes to start from the first atom, or continue from where they are
            physics = LocatePhysics(atom);
            //We start the atom, passing it our callback hook so we know when to load the next one

            physics.Initialise(atom, PostInit);

        }

        public void PostInit(bool success){
            Debug.Log("Initialisation reports all ok?: " + success.ToString());

            //Hack to try to stop screen darkening. It doesn't seem to work in EyeSkillsInit script?!?
            Screen.sleepTimeout = (int)0f;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            //If voice visualisation requested, start it. We end it when the atom expires.
            //if (physics.Atom.visualiseVoice){
            //    UnityVoiceVisualiser.instance.StartVisualisation();
            //}

            physics.Start(Atomised);
        }

        public void Play()
        {
            Play(currentAtom);
        }


        /// <summary>
        /// When an atom has run to completion, we receive a callback here
        /// </summary>
        /// <param name="_id">Identifier.</param>
        public void Atomised()
        {
            //Now organise either moving on to the next atom, or wrapping up the day and quiting the app/returning to a higher level menu
            //The ending depends on the physics (e.g. the user finishing their exploration.
            Debug.Log("Atomised callback in ContentNode");

        }

        /// <summary>
        /// When an atom has run to completion, we receive a callback here
        /// if that atoms specifies a specific atom to transition to.
        /// </summary>
        /// <param name="_id">Identifier.</param>
        public void Atomised(string nextAtomID)
        {
            Debug.Log("Atomised callback in ContentNode - next atom specified as (" + nextAtomID + ")");

            //Shut down visualisation if it exists.
            //if (physics.Atom.visualiseVoice)
            //{
            //    UnityVoiceVisualiser.instance.StopVisualisation();
            //}

            if (nextAtomID != "")
            {
                ESTrainingAtom nextAtom = FindAtom(nextAtomID, atoms);
                if (nextAtom != null)
                {
                    currentAtom = nextAtom;
                    Play(currentAtom);
                }
                else
                {
                    Debug.Log("Could not find specified atom!" + nextAtomID);
                }
            } else {
                ESTrainingAtom nextAtom = FindNextAtom();
                if (nextAtom != null)
                {
                    currentAtom = nextAtom;
                    Play(currentAtom);
                }
                else
                {
                    Debug.Log("Ran out of Atoms");
                    Debug.Log("This is where we call back to our maker");
                    currentAtom = atoms[0];//Reset the atom to the original atom so that it becomes the restart point
                    QuitToSceneSelector();
                }
            }
        }
    }
}