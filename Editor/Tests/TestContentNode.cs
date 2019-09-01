using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using EyeSkills;
using System;

public class TestContentNode
{

    private List<ESTrainingAtom> atoms = new List<ESTrainingAtom>();

    public string callbackMessageReceived = "";

    private void populateSimpleAtoms()
    {
        atoms = new List<ESTrainingAtom>();
        for (int i = 0; i < 10; i++)
        {
            ESTrainingAtom atom = new ESTrainingAtom();
            atom.id = "id_" + i.ToString();
            atom.preferredPhysics = "physics_" + i.ToString();
            //atom.subtitleText = "text_" + i.ToString();
            atom.audioFile = "audio_test.mp3";
            atoms.Add(atom);
        }
    }


    public bool callbackCompleted = false;

    private ESTrainingAtom buildDefaultAtom()
    {
        ESTrainingAtom atom = new ESTrainingAtom();
        atom.id = "id_notFound";
        atom.preferredPhysics = "physics_notFound";
        //atom.subtitleText = "text_someText";
        atom.audioFile = "audio_test.mp3";
        return atom;
    }

    private ESTrainingAtom buildCustomAtom()
    {
        ESTrainingAtom atom = new ESTrainingAtom();
        atom.id = "TESTINGID";
        atom.preferredPhysics = "TESTINGPHYSICS";
        //atom.subtitleText = "text_someText";
        atom.audioFile = "audio_test.mp3";
        return atom;
    }

    [Test]
    public void TestFreshInitialisationOfAtomsWithoutPlayerPrefs()
    {

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ContentNode contentNode = new ContentNode();
        populateSimpleAtoms();

        contentNode.InitialiseAtoms(atoms);

        Assert.AreEqual(contentNode.currentNodeID, "id_0");
        Assert.AreEqual(contentNode.currentNodeCompleted, false);
        Assert.AreSame(atoms[0], contentNode.currentAtom);

    }

    [Test]
    public void TestFreshInitialisationOfAtomsWithPlayerPrefs()
    {

        PlayerPrefs.DeleteAll();

        ContentNode contentNode = new ContentNode();
        populateSimpleAtoms();

        PlayerPrefs.SetString("currentNodeID", atoms[2].id);
        PlayerPrefs.SetInt("currentNodeCompleted", 1);
        PlayerPrefs.Save();

        contentNode.InitialiseAtoms(atoms);

        Assert.AreEqual(contentNode.currentNodeID, "id_2");
        Assert.AreEqual(contentNode.currentNodeCompleted, true);
        Assert.AreSame(atoms[2], contentNode.currentAtom);

    }

    [Test]
    public void TestFreshInitialisationOfAtomsWithPlayerPrefsAndMissingAtom()
    {

        PlayerPrefs.DeleteAll();

        ContentNode contentNode = new ContentNode();
        populateSimpleAtoms();

        PlayerPrefs.SetString("currentNodeID", "id_missingAtom");
        PlayerPrefs.SetInt("currentNodeCompleted", 1);
        PlayerPrefs.Save();

        contentNode.InitialiseAtoms(atoms);

        Assert.AreEqual(contentNode.currentNodeID, "id_0");
        Assert.AreEqual(contentNode.currentNodeCompleted, false);
        Assert.AreSame(atoms[0], contentNode.currentAtom);

    }

    [Test]
    public void TestGetNextAtomStep()
    {
        ContentNode contentNode = new ContentNode();
        populateSimpleAtoms();
        contentNode.InitialiseAtoms(atoms);
        contentNode.currentAtom = atoms[1];
        ESTrainingAtom a = contentNode.FindNextAtom();

        Assert.AreEqual(atoms[2], a);
    }

    [Test]
    public void TestGetNextAtomBounds()
    {
        ContentNode contentNode = new ContentNode();
        populateSimpleAtoms();
        contentNode.InitialiseAtoms(atoms);
        contentNode.currentAtom = atoms[9];
        ESTrainingAtom a = contentNode.FindNextAtom();

        Assert.AreEqual(null, a);

    }

    [Test]
    public void TestInitialiseWithoutCustomClassPresent()
    {
        ContentNode contentNode = new ContentNode();

        ESTrainingAtom atom = buildDefaultAtom();

        ESAtomPhysicsInterface physics = contentNode.LocatePhysics(atom);

        Assert.AreEqual("EyeSkills.ESTrainingDefaultPhysics", physics.GetType().ToString());

        //https://www.devjoy.com/blog/unit-testing-events-and-callbacks-in-csharp/

        ESTrainingDefaultPhysics physicsConcrete = physics as ESTrainingDefaultPhysics;

        //physics.Initialise(atom); //Now we overwrite the initialised settings with mocks
        physics.Atom = atom; //We skip initialisation because it has GameObject dependencies.

        //Specify a different audio manager so that the ESTrainingAtom can re-route audio to our mock
        AudioManagerMock audioMock = new AudioManagerMock();
        physicsConcrete.audioManager = audioMock;
        //SubTitlesVisualiserMock visMock = new SubTitlesVisualiserMock();
        //physicsConcrete.subsVisualiser = visMock;
        //VoiceVisualiserMock subsMock = new VoiceVisualiserMock();
        //physicsConcrete.voiceVisualiser = subsMock;

        callbackCompleted = false;
        physics.Start(TestCallback);

        Assert.True(callbackCompleted);
        Assert.AreEqual(audioMock.providedKey, atom.audioFile);
        //We don't actually initialise these anymore
        //Assert.True(visMock.called);
        //Assert.True(subsMock.called);
    }

    public void TestCallback(string message)
    {
        callbackMessageReceived = message;
        callbackCompleted = true;
    }

    [Test]
    public void TestInitialiseWithCustomClassPresent()
    {

        ContentNode contentNode = new ContentNode();

        ESTrainingAtom atom = buildCustomAtom();

        ESAtomPhysicsInterface physics = contentNode.LocatePhysics(atom);

        physics.Initialise(atom,delegate(bool success){
            Assert.True(success);
        }); //Now we overwrite the initialised settings with mocks

        Assert.NotNull(physics);
        Assert.AreEqual("EyeSkills.ESAtomPhysicsTESTINGPHYSICS", physics.GetType().ToString());
    }

    //Now test a callback which specifies a specific next atom
    [Test]
    public void TestCallbacksSpecifyingValidNext()
    {

    }

    [Test]
    public void TestCallbacksSpecifyingInvalidNext()
    {

    }
}
