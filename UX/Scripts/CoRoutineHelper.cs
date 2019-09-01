using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills{

    public class CoRoutineHelper : MonoBehaviour, ICoRoutineHelper
    {
        public static CoRoutineHelper instance;

        private List<Coroutine> coroutines = new List<Coroutine>();

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        public Coroutine BeginCoroutine(IEnumerator coroutine)
        {
            Coroutine c = StartCoroutine(coroutine);
            coroutines.Add(c);
            return c;
        }

        public void EndCoroutine(Coroutine coroutine)
        {
            //Another Unity/Mono bug.  Without this check, the coroutine will be continually "destroyed" until there is a stack overflow.
            if (coroutines.Contains(coroutine)){
                Debug.Log("Stopping coroutine");
                coroutines.Remove(coroutine);
                EndCoroutine(coroutine);
            } else {
                Debug.Log("Could not find coroutine");
            }


        }
    }


}
