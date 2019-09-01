using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

namespace EyeSkills
{


    /// <summary>
    /// Based on the User experiences, we now choose the path they will take.
    /// </summary>
    public class ESAtomPhysicsDay2SummaryController : ESAtomPhysicsDefaultVR
    {

        //The user data we base our decisions on
        public SuppressionTestsData data;

        public override void Initialise(ESTrainingAtom _atom, Action<bool> _initCallback)
        {
            data = new SuppressionTestsData();
            atom = _atom;
            //base.Initialise(_atom, _initCallback);
            _initCallback(true);

        }


        public override void Start(Action<String> _completionCallback)
        {
            //base.Start(_completionCallback);

            data.Load();

            if (data.couldTiltUpward && 
                data.couldTiltDownward &&
                data.binocularTolerance){
                _completionCallback(atom.nextStep[0]);
            } else { //They didn't make it.
                _completionCallback(atom.nextStep[1]);
            }

        }
    }
}
