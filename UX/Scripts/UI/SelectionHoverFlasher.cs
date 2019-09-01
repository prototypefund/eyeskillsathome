using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills
{
    public class SelectionHoverFlasher : MonoBehaviour
    {
        private VRInteractiveItem interactiveItem;
        public bool reset = false;
        private IEnumerator coroutine;
        Color lerpedColor = Color.white;

        public void Start()
        {
            interactiveItem = this.GetComponent<VRInteractiveItem>();
            if (interactiveItem){
                interactiveItem.OnOver += StartHoverSelection;
                interactiveItem.OnOut += ResetHoverSelection;
            }
        }

        IEnumerator AlterColour()
        {
            while (!reset)
            {
                lerpedColor = Color.Lerp(Color.white, Color.green, Mathf.PingPong(Time.time, 0.5f));
                this.GetComponent<MeshRenderer>().material.SetColor("_Color", lerpedColor);
                yield return null;
            }
        }

        void StartHoverSelection(string id)
        {
            coroutine = AlterColour();
            StartCoroutine(coroutine);
        }

        void ResetHoverSelection(string id)
        {
            StopCoroutine(coroutine);
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
    }
}