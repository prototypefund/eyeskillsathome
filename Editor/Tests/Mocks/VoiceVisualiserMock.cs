using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EyeSkills;

public class VoiceVisualiserMock : IVoiceVisualiser {

    public bool called = false;


    public void StartVisualisation(){
        called = true;
    }

    public void StopVisualisation(){

    }

}
