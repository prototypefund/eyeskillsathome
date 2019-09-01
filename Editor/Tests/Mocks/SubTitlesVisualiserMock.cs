using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyeSkills;

public class SubTitlesVisualiserMock : ISubTitlesVisualiser
{
    public bool called = false;

    public void Visualise(string text, AudioSource source)
    {
        called = true;
    }
}
