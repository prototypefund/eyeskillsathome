using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using EyeSkills;
using System.IO;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;

namespace EyeSkills {

    /// <summary>
    /// A list of atoms which make up the day's experience.
    /// TODO: Enforce some convension in the naming?
    /// </summary>
    public class DayAtoms : MonoBehaviour
    {
        public string DayID;


        public ContentNode contentNode = new ContentNode();

//#if UNITY_EDITOR
        [ReorderableList]
//#endif
        public List<ESTrainingAtom> atoms;

        private bool started =false;

        void OnLevelWasLoaded()
        {
            if (!started) Begin();
            started = true;
        }
        private void Start()
        {
            if (!started) Begin();
            started = true;
        }

        void Begin(){
            contentNode.InitialiseAtoms(atoms);
            contentNode.Play();
        }

//#if UNITY_EDITOR
        [Button("Enable all atoms")]
//#endif
        private void EnableAllAtoms()
        {
            foreach (ESTrainingAtom atom in atoms)
            {
                atom.disableAtom = false;
            }
        }

//#if UNITY_EDITOR
        [Button("Disable all atoms")]
//#endif
        private void DisableAllAtoms()
        {
            foreach (ESTrainingAtom atom in atoms)
            {
                atom.disableAtom = true;
            }
        }

        static void WriteToFile(string content, string path)
        {

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(content);
            writer.Close();

            //Re-import the file to update the reference in the editor
            //AssetDatabase.ImportAsset(path);
            //TextAsset asset = Resources.Load("test");
        }

        public string ReadFromFile(string path)
        {
            StreamReader reader = new StreamReader(path, false);
            string output = reader.ReadToEnd();
            reader.Close();
            return output;
        }

        public void ApplyGrapViz(string dotFile, string imageFile)
        {
            string arguments = dotFile + "  -o " + imageFile + "  -T png";

            ProcessStartInfo processInfo = new ProcessStartInfo("/usr/local/bin/dot", arguments);
            processInfo.UseShellExecute = false;

            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }

        public void OpenInPreview(string imageFile)
        {
            string arguments = "  -a Preview " + imageFile;

            ProcessStartInfo processInfo = new ProcessStartInfo("open", arguments);
            processInfo.UseShellExecute = false;

            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }

        private List<string> OutputLink(List<string> links, ESTrainingAtom atom, string nextStep, bool inferred)
        {
            string linkStr = "";
            if (atoms.Exists(x => x.id == nextStep))
            {
                linkStr = "\"" + atom.id + "\" -> " + "\"" + nextStep + "\"";
            }
            else
            {
                linkStr = "\"" + atom.id + "\" -> ERROR";
            }
            if (inferred)
            {
                linkStr += "[style=dashed]";
            }
            links.Add(linkStr);
            return links;
        }

//#if UNITY_EDITOR
        [Button("Render connectivity as graph")]
//#endif
        private void AtomConnectivity()
        {
            List<string> nodes = new List<string>(), links = new List<string>();

            nodes.Add("\"ERROR\" [label=\"target node missing!\" color=\"red\"]"); //OUR ERROR NODE

            string nodeColour = "black";

            foreach (ESTrainingAtom atom in atoms)
            {
                //atom.disableAtom = true;

                if (atom.why == "") atom.why = "";

                //If it references an Audio or Image resource, let's check that resource is really there.
                bool imageFail = true;
                bool audioFail = true;
                bool moleculeMissing = false;
                bool physicsMissing = true;

                if ((atom.image != null) && (atom.image != ""))
                {
                    Sprite image = Resources.Load<Sprite>(atom.image);
                    //Sprite image = EditorGUIUtility.Load("hishy") as Sprite;
                    //UnityEngine.Debug.Log("atom.imageFile " + atom.image + " : "+image);
                    if (image != null)
                    {
                        imageFail = false;
                    }
                }
                else
                {
                    imageFail = false;
                }
                if ((atom.audioFile != null) && (atom.audioFile != ""))
                {
                    AudioClip ac = Resources.Load<AudioClip>("Audio/en/" + atom.audioFile);
                    //UnityEngine.Debug.Log("atom.audioFile "+atom.audioFile+" : "+ac);
                    //AudioClip ac = EditorGUIUtility.Load(atom.audioFile+".mp3") as AudioClip;
                    if (ac != null)
                    {
                        audioFail = false;
                    }
                }
                else
                {
                    audioFail = false;
                }

                /* How can I check for GameObjects in a scene within the Editor?
                if ((atom.molecule != null) && (atom.molecule != ""))
                {
                    Type t = Type.GetType("UIMolecule-" + atom.molecule, false);
                    if (t != null) moleculeMissing = false;
                } else {
                    moleculeMissing = false;
                }
                */

                if ((atom.preferredPhysics != null) && (atom.preferredPhysics != ""))
                {
                    Type t = Type.GetType("EyeSkills.ESAtomPhysics" + atom.preferredPhysics, false);
                    if (t != null) physicsMissing = false;
                }
                else
                {
                    physicsMissing = false;
                }

                //The idea here was to make the association between xlabel and it's node clearer. It doesn't work well at all because the visual order of node placement isn't "sequential" so you end up with blocks of single colour. Might want to extend with multiple colours?. 
                //Leaving the idea here, but it needs to be improved.
                //nodeColour = (nodeColour == "grey") ? "black" : "grey";

                nodes.Add("\"" + atom.id
                          + "\" [label=\"id: "
                          + atom.id
                          + "\\n physics: "
                          + atom.preferredPhysics
                          + "\\n state: ("
                          + atom.physicsState
                          + ") \\n "
                          + ((imageFail == true) ? "(image missing)" : "")
                          + ((audioFail == true) ? "(audio missing)" : "")
                          + ((moleculeMissing == true) ? "(molecule missing)" : "")
                          + ((physicsMissing == true) ? "(physics missing)" : "")
                          + "\", xlabel=\""
                          + atom.why
                          + "\", "
                          + ((imageFail || audioFail || moleculeMissing || physicsMissing) ? " color=\"red\" shape=\"doubleoctagon\" " : "color =\"black\"")
                          + " fontcolor=\""
                          + nodeColour
                          + "\""
                          + " color=\""
                          + nodeColour
                          + "\""
                          + "]");

                if (atom.nextStep.Count == 0)
                {
                    //We assume a transition to the next node in the list - if it exists
                    int index = atoms.IndexOf(atom);
                    if (index + 1 < atoms.Count)
                    {
                        links = OutputLink(links, atom, atoms[index + 1].id, true);
                    }

                }
                else
                {
                    foreach (string nextStep in atom.nextStep)
                    {

                        //We need to check if the referenced nextStep exists as an id, otherwise we link to the error state
                        links = OutputLink(links, atom, nextStep, false);

                    }
                }

            }

            string nodeOutput = string.Join(";\n", nodes);
            string linkOutput = string.Join(";\n", links);
            string dotOutput = "digraph g { nodesep=1.5; ranksep = 1; " + nodeOutput + "\n" + linkOutput + "}";

            //Now we need to use dot and imagemagick (OSX only) to render out output graph and open it in preview
            string outputFile = "/tmp/eyeskillsOutput.dot";
            string imageFile = "/tmp/eyeskillsOutput.png";
            WriteToFile(dotOutput, outputFile);
            ApplyGrapViz(outputFile, imageFile);
            OpenInPreview(imageFile);
        }


//#if UNITY_EDITOR
        [Button("Export atoms to JSON in ../DayJSONBackups")]
//#endif
        private void ExportAsJson()
        {
            var json = JsonConvert.SerializeObject(atoms);
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            WriteToFile(json, "../DayJSONBackups/day-" + DayID + "-" + cur_time.ToString() + ".json");
        }

//#if UNITY_EDITOR
        [Button("Temp export to import.json")]
//#endif
        private void TempExportAsJson()
        {
            var json = JsonConvert.SerializeObject(atoms);
            WriteToFile(json, "../DayJSONBackups/import.json");
        }

//#if UNITY_EDITOR
        [Button("Import from ../DayJSONBackups/import.json")]
//#endif
        private void ImportFromJson()
        {
            string json = ReadFromFile("../DayJSONBackups/import.json");
            List<ESTrainingAtom> newAtoms = JsonConvert.DeserializeObject<IEnumerable<ESTrainingAtom>>(json).ToList();
            //Now replace the atoms
            if ((newAtoms!=null) && (newAtoms.Count>0)){
                atoms = newAtoms;
            } else {
                UnityEngine.Debug.Log("Could not load new atoms.");
            }


        }
    }

}

