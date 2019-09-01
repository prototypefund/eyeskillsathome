using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public GameObject cube;
    public float x = 0;

    // Use this for initialization
    void Start () {
        Debug.Log("I HAVE STARTED");
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("I AM UPDATING");
        cube.transform.localPosition = new Vector3(Mathf.PingPong(Time.time, 3),0,0);
    }
}
