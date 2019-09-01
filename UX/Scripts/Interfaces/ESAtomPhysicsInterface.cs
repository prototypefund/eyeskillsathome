using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EyeSkills
{
    //We shall have a default base class which specific paragraphs can over-ride (always calling their parent however)
    public interface ESAtomPhysicsInterface
    {

        ESTrainingAtom Atom { get; set; }

        ///<summary>
        /// Initialise the specified atom.  The Initialisation and "Start" phase are split to ensure that all MonoBehaviour (asynchronous or coroutine) using
        /// parts can be run in Initialise.  This allows the rest of any conformant class to be unit tested (Unity does not support unit testing MonoBehaviour).
        /// Because we cannot anticipate when the Initialisation will complete, we must indicate we have completed by using the supplied callback.
        /// </summary>
        /// <param name="atom">Atom.</param>
        /// <param name="_completionCallback">Completion callback. Did we succeed or fail?</param>
        void Initialise(ESTrainingAtom atom, Action<bool> _initCallback); 

        /// <summary>
        /// Start the specified Physics.  When complete, use _completionCallback.
        /// </summary>
        /// <param name="_completionCallback">Completion callback.</param>
        void Start(Action<String> _completionCallback);

        //void AtomExpiry();
        //bool FinishHook();
    }
}
