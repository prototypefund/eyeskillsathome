using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills
{
    public class SuppressionTestsData : InfoBase
    {
        //conflict
        public string chosenPerception;

        //if we could use blinkers to remove conflict
        public bool blinkersRemovedConflict;

        //if we could break through suppression
        public bool couldTiltUpward;
        public bool couldTiltDownward;
        public int numberOfTiltAttempts; //Are people struggling with managing the tilt?
        public bool binocularTolerance; //Just because they were still to end the suppressionRatio scene doesn't mean they broke supression.  We check what they saw with the conflict gallery and store the conclusion here.
        public string chosenTolerancePerception;
        public float binocularSuppressionRatio;
    }
}