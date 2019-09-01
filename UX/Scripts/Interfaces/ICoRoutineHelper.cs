using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills {

    public interface ICoRoutineHelper
    {
        Coroutine BeginCoroutine(IEnumerator coroutine);
        void EndCoroutine(Coroutine coroutine);
    }

}

