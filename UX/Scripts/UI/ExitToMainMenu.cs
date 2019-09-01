using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EyeSkills {

    public class ExitToMainMenu : MonoBehaviour
    {

        public DayAtoms dayAtoms;

        // Update is called once per frame
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.instance.StopAudioOnly();
                dayAtoms.contentNode.QuitToSceneSelector();
            }
        }
    }
}