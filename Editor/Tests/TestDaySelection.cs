using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

namespace EyeSkills
{


    public class TestDaySelection
    {

        public SceneSelector daySelector;
        public DaySelectorData data;
        public string sceneToUnload;

        //Note: In all cases test that DaySelectorData has been updated.

        public void SetupDaySelector()
        {

            daySelector = new SceneSelector
            {
                tryAgainTomorrowSceneName = "TryAgainTomorrow",
                bannedSceneName = "UnsafeToContine",
                trialCompletedSceneName = "TrialCompleted",
                corruptedSceneName = "CorruptedSceneName",
                trialDaySceneNames = new List<string>() {
                    "Day1",
                    "Day2",
                    "Day3",
                    "Day4"
                }

            };
        }

        public void SetupInitialDayCase()
        {
            data = new DaySelectorData
            {
                currentTime = DateTime.UtcNow.ToLongTimeString(),
                currentTimeZone = TimeZone.CurrentTimeZone.ToString()
            };
            daySelector.data = data;
        }

        [UnitySetUp]
        public IEnumerator SetupScene(){
            sceneToUnload = "SceneSelector";
            AsyncOperation loadScene = SceneManager.LoadSceneAsync("SceneSelector");
            loadScene.allowSceneActivation = true;
            while (!loadScene.isDone)
            {
                yield return null;
            }
        }

        [UnityTearDown]
        public void TearDown(){
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        [Test]
        public void TestFirstUsage()
        {

            SetupDaySelector();
            SetupInitialDayCase();

            string nextSceneName = daySelector.GetNextSceneName();
            Assert.AreSame("Day1", nextSceneName, "On the initial run, we should be selecting the first day");
        }

        [Test]
        public void TestSceneNameCorrupted()
        {
            SetupDaySelector();
            SetupInitialDayCase();
            daySelector.data.lastCompletedSceneName = "corruptedSceneName";


            string nextSceneName = daySelector.GetNextSceneName();
            Assert.AreSame("CorruptedSceneName", nextSceneName, "On the initial run, we should be selecting the first day");
        }

        [Test]
        public void TestNoMoreScenesLeft()
        {
            //It should report the next scene name as TrialCompletedSceneName
            SetupDaySelector();
            SetupInitialDayCase();

            daySelector.data.lastCompletedSceneName = "Day4";

            string nextSceneName = daySelector.GetNextSceneName();
            Assert.AreSame(daySelector.trialCompletedSceneName, nextSceneName, "On the initial run, we should be selecting the first day");

        }

        [Test]
        public void TestTryingToPlayAgainTooSoon()
        {
            //It should report the next scene name as TrialCompletedSceneName
            SetupDaySelector();
            SetupInitialDayCase();

            daySelector.data.lastCompletedSceneName = "Day2";
            daySelector.data.lastCompletedSceneTime = DateTime.Now.AddHours(-1).ToUniversalTime().ToString();

            string nextSceneName = daySelector.GetNextSceneName();
            Assert.AreSame(daySelector.tryAgainTomorrowSceneName, nextSceneName, "On the initial run, we should be selecting the first day");

        }

        /*
        [UnityTest]
        public void TestTryingToPlayAfterBan()
        {
            //TODO: How and where (in what object) do we set the ban?
            //In a simple PlayerPrefs object? Probably not good.
            //We want it recorded. DaySelectorData sounds a bit wrong...
            //but we make it a persistent object and write to it!
            //This also allows us to update it directly when completed... see below
            // BUUUUUT  Can we mark a non monobehaviour to persist like this?
        }


        [UnityTest]
        public void TestValidNextScene()
        {
            //Both valid in time and in next choice of scene
        }

        [Test]
        public void TestSceneCompletion(){
            //TODO: By persisting the DaySelectionData object
            //We can actually have ContentNode transparently
            //set when the Day is finished by writing its scene name
            //it can also set automatically the newest play time (last time played)
            //AHA TODO: Extend with startedPlayTime, startedSceneName - because this way we might be able to track crashes too.
            //and a scene can also set the ban. 
            //Add this test to the TestContentNode and extend
            //The IDE structure with a trigger on each atom to mark when it counts as the level being complete (so people don't just kill the app and play again)
        }

    */
    }
}