using UnityEngine;

namespace EyeSkills
{
    public class UnitySubTitlesVisualiser : MonoBehaviour, ISubTitlesVisualiser
    {

        // singleton like access pattern
        public static UnitySubTitlesVisualiser instance = null;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        public void Visualise(string text, AudioSource length)
        {
            //throw new System.NotImplementedException();
        }

        public void Hide(){
            this.Hide();
        }

        public void Show(){
            this.Show();
        }

        public void Start()
        {

        }
    }
}