using System;
using System.Collections.Generic;
using NaughtyAttributes;

namespace EyeSkills {

    /// <summary>
    /// ESTraining atom. We make this quite a rigid structure, to encourage the app interface to not devolve into total wierdness.
    /// </summary>
    [Serializable]
    public class ESTrainingAtom
    {
        public string id;
        /// <summary>
        /// The preferred physics. If blank, it will use default physics class
        /// </summary>
        public bool disableAtom;

        public string preferredPhysics; //The physics for manipulating this atom...
        public int physicsState;
        public string molecule; //...in the space of this molecule
        //public string subtitleText;
        //public bool visualiseVoice; No longer needed
        public string image;
        public string titleText;
        public string why; //Explain why this node exists. Used when rendering transition diagrams. 
        public string audioFile;
        public bool restartable; //Can we restart from this point- or are we in the middle of an experience. Iterate backwards to find a natural restart point, marked as such.
        //public bool decayWhenFinished;
        //public string actionButtonURL;
        //public string actionButtonText;
        //public bool showActionButton;

        //Up to three choices per atom for the next atom to proceed to (one-way linked list).
        public List<string> nextStep;

    }

}