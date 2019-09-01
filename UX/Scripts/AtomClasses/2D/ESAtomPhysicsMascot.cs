using System;
using UnityEngine;
using UnityEngine.UI;

namespace EyeSkills
{
    //TODO: There should be an abstract class with many of the helper methods implemented and waiting, only waiting on an override of init or start.
    public class ESAtomPhysicsMascot : ESTrainingDefaultPhysics
    {
        /*
        public GameObject domi, supti;
        private Animator domiAnim,suptiAnim;
        */

        /// <summary>
        /// We deal with animated mascots rather than static images, 
        /// so we override StartVisuals
        /// </summary>
        public override void StartVisuals()
        {
            molecule = ActivateUIMolecule();

            //Transfer the physics state to all connected animators
            foreach (UIMoleculeChildList c in molecule.animatedChildren){
                Animator a = c.gameObject.GetComponent<Animator>();
                if (a!=null){
                    a.SetInteger("physicsState", atom.physicsState);
                }
            };



            //Look in the animation controller (Animator pane) for how the physicsState paramater on Domi and Supti control how they behave
            /*
            domi = GetMolecularChild(molecule, "Domi");
            domiAnim = domi.GetComponent<Animator>();

            supti = GetMolecularChild(molecule, "Supti");
            suptiAnim = supti.GetComponent<Animator>();

            domiAnim.SetInteger("physicsState", atom.physicsState);
            suptiAnim.SetInteger("physicsState", atom.physicsState);
            */

        }

        public void AtomExpirySoft(string next=""){
            //Doesn't stop the animation
            completionCallback(next);
        }

        public override void AtomExpiry()
        {
            if (atom.physicsState >= 666) DeActivateUIMolecule();

            if ((atom.nextStep != null) && (atom.nextStep.Count>0))
            {
                AtomExpirySoft(atom.nextStep[0]);
            } else {
                AtomExpirySoft();
            }
        }

    }
}