using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeSkills
{

    public class MouthControl : MonoBehaviour
    {

        //The top right of the texure is rendered.  You need to move the sheet to place the expression you want there.

        private float xOffset = 4.28f, yOffset = -2.26f; //The correct offsets for our needs. These map the local positions of the mouth bone onto the mouth texture which goes from 0,0 in top right corner, to -0.75,-0.9 in bottom left corner.
        public GameObject modelArmature;
        private GameObject mouthBone;
        public Material mouthShapes;
        public string iExist;
        public string mouthTargetName = "Face_start";

        private Vector3 faceTransformLocal, newTransform, lastTransform;

        private Vector3 scaleFactor;
        private string debug = "";

        public void Start()
        {
            //Debug.Log(">>>>>>>>>>>> MouthControl - I have started! " + iExist);

            if (modelArmature != null)
            {

                foreach (Transform t in modelArmature.GetComponentsInChildren<Transform>())
                {
                    if (t.name == mouthTargetName)
                    {
                        mouthBone = t.gameObject;
                        Debug.Log("Found face with name " + mouthBone.name);
                    }
                }

                scaleFactor = modelArmature.transform.localScale;

            }
            else
            {
                Debug.Log("Model armature was null for " + iExist);
            }
        }

        public void Update()
        {
            if (mouthBone != null)
            {

                faceTransformLocal = mouthBone.transform.localPosition;
                newTransform = new Vector2(-(faceTransformLocal.x * scaleFactor.x + xOffset), faceTransformLocal.z * scaleFactor.y + yOffset);

                if (newTransform.Equals(lastTransform))
                {
                    mouthShapes.mainTextureOffset = newTransform;
                    //Debug.Log("newTransform : " + newTransform);
                }
                else
                {
                    //Debug.Log("No Match! :" + newTransform + lastTransform);
                }

                lastTransform = newTransform;
            }
        }
    }
}