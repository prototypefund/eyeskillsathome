using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{

    public class PopulateDayGrid : MonoBehaviour
    {
        public GameObject pastPrefab,currentPrefab,futurePrefab;

        public SceneSelector sceneSelector;

        public int numberToCreate;

        // Start is called before the first frame update
        void Start()
        {
            Populate();
        }


        void Populate()
        {
            GameObject parentObj;

            //Now we want to reference our SceneSelector and get a list of past, present, future days, and unavailable days.
            //We can replay past days (within limits), choose to play the present day, and future days result in crowd funding?
            //Or should each day have extra information with it? In a future version, yes - a sliding panel explaining why it isn't available, what you can do, and what it does/would do. For now, basic.

            //We operate for now by getting the full list of scenes, and the id for the next scene. All prior days belong to the past, and all others, the future
            //Pick the appropriate prefab
            /*
            for (int i = 0; i < numberToCreate; i++){
                newObj = (GameObject) Instantiate(prefab, transform);
                newObj.GetComponent<Image>().color = Random.ColorHSV();
            }
            */
            sceneSelector.Load();

            string nextSceneName = sceneSelector.GetNextSceneName();
            int nextSceneIndex = sceneSelector.trialDaySceneNames.IndexOf(nextSceneName);

            for (int i = 0; i < sceneSelector.trialDaySceneNames.Count; i++)
            {
                GameObject p;
                if (i < nextSceneIndex)
                {
                    p = pastPrefab;
                }
                else if (i == nextSceneIndex)
                {
                    p = currentPrefab;
                }
                else
                {
                    p = futurePrefab;
                }
                //The Prefabs need to be buttons.
                int id = i;
                parentObj = (GameObject)Instantiate(p, transform);
                //Set the day ID on the element using i. Set the string id on another field.
                parentObj.GetComponentInChildren<Text>().text = "Day " + (id + 1);
                //Add a listener to respond to the button, calling back to the scene manager.
                parentObj.GetComponent<Button>().onClick.AddListener(delegate(){ sceneSelector.LoadScene("Day" + (id + 1)); });

                //TODO : Why not try and add a little more contextual information to the day scheduler.

            }
        }
    }

}