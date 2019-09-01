using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills
{
    public class EyeSkillsInit : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

    }

}