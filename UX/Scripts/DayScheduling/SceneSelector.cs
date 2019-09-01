using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EyeSkills
{

    public class DaySelectorData : ConfigBase
    {
        public string lastCompletedSceneName = "";
        public string currentTime = ""; 
        public string lastCompletedSceneTime = "";
        public string currentTimeZone = ""; 
    }

    /// <summary>
    /// This defines the scenes which make up the app, and manages which scenes and when the participant can play.
    /// It requires each individual scene to know how to use DaySelectorData which keeps track of the user's progress.
    /// If you want to test with a particular day, just start that scene or only include it in your mobile build.
    /// </summary>
    public class SceneSelector : MonoBehaviour
    { 
        //This is an InfoBase - it allows us to recreate what the user did, when and follow any complaints they may have.
        public string tryAgainTomorrowSceneName, bannedSceneName, trialCompletedSceneName, corruptedSceneName;
        public List<string> trialDaySceneNames;
        protected int currentTime, lastCompletedSceneTime; //We will work out if they are allowed to use the app again today.

        public DaySelectorData data;

        public static SceneSelector instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EyeSkills.DaySelector"/> class.
        /// </summary>
        public SceneSelector(){

        }

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

        }

        /// <summary>
        /// Exits to this menu.  Either a result of hitting the android back button, or a special Atom as the final content node entry.
        /// Should be called from the ContentNode as it can also pass any restore points.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public void ExitToMenu(string sceneName, string restorePointAtomId){
            //TODO: STORE THE DATA
            SceneManager.UnloadSceneAsync(sceneName);
            SceneManager.LoadScene("SceneSelector");
            //SceneManager.UnloadScene(sceneName);//.completed += LoadMenu;
        }


        public void Load()
        {
            data = new DaySelectorData();
            data.Load();
        }

        private string DefaultScene(){
            return trialDaySceneNames[0];
        }

        /// <summary>
        /// Gets the name of the next scene which would be loaded
        /// </summary>
        /// <returns>The next scene name.</returns>
        public string GetNextSceneName(){

            //First, lets see if this is an initial run
            if (data.lastCompletedSceneName==""){
                return DefaultScene();
            } else

            { //Let's see if we can find the last scene so we know where to continue from

                if (!trialDaySceneNames.Contains(data.lastCompletedSceneName))
                {
                    return corruptedSceneName;
                }
                else
                { //We found the scene name, so let's see if there's one afer it.

                    int i = trialDaySceneNames.IndexOf(data.lastCompletedSceneName);

                    if (i >= (trialDaySceneNames.Count - 1))
                    { //We have reached the last entry

                        //TODO: This is where we might look to load non-trial days in dynamically

                        return (trialCompletedSceneName);

                    } else 
                    { //Now we want to check if we are trying to play again too soon (min 12hrs wait time)

                        //Check the 12hr interval. The scene we load should explain when the next possible showing will be.
                        DateTime nextValidMoment = new DateTime();
                        DateTime.TryParse(data.lastCompletedSceneTime,out nextValidMoment);
                        nextValidMoment = nextValidMoment.AddHours(12);
                        //Are we too early or not?
                        int early = DateTime.Compare(nextValidMoment,DateTime.Now);

                        if (early > 0) {

                            return (tryAgainTomorrowSceneName);

                        } else {

                            return (trialDaySceneNames[trialDaySceneNames.IndexOf(data.lastCompletedSceneName) + 1]);
                        }

                    }
                }
                return "somethingRandom";
            }
            return null;
        }

        public void LoadScene(string id){
            //Scene s = SceneManager.GetSceneByName(id);
            SceneManager.LoadScene(id);
        }

        /// <summary>
        /// Loads the next scene. Throws an exception if it can't.
        /// </summary>
        public void LoadNextScene(){

        }

        public DaySelectorData GetCurrentDaySelectorData(){
            return null;
        }


        //Store the data
        //If first usage or corrupted scene name, start from beginning
        //If they haven't left enough time since completing the last scene and now, play the TryAgainTomorrowScene
        //If they have been banned, load that scene and let it explain why and encourage a crowd fund etc. (we need to generate that ban InfoBase object inside each Day as appropriate)
        //If they have left enough time and they haven't been banned, discover the index of the last scene they completed, and if there is still one left, move onto the next scene.
        //If we are at the end of our TrialDaySceneNames, then start the TrialCompletedSceneName.
    }
}