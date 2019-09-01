using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Molecule. Because we can't use Unity's FIND to find inactive game objects we have an active container with a reference to the inactive container containing the parts we want to show/hide
/// </summary>
public class UIMolecule : MonoBehaviour {

    public GameObject container;

    public List<UIMoleculeChildList> children = new List<UIMoleculeChildList>();

    public List<UIMoleculeChildList> animatedChildren = new List<UIMoleculeChildList>();

}
