using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyeSkills;
using System;

namespace EyeSkills
{

    public class ESAtomPhysicsTESTINGPHYSICS : ESAtomPhysicsInterface
    {
        ESTrainingAtom atom;

        public ESTrainingAtom Atom
        {
            get
            {
                return atom;
            }

            set
            {
                atom = value;
            }
        }

        public void AtomExpiry()
        {
            throw new NotImplementedException();
        }

        public void Initialise(ESTrainingAtom atom, Action<bool> _initCallback)
        {
            _initCallback(true);
        }

        public void Start(Action<String> _completionCallback)
        {
            throw new NotImplementedException();
        }
    }
}