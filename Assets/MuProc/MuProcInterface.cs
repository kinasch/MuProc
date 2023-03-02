using System;
using System.Collections;
using System.Collections.Generic;
using MuProc;
using UnityEditor;
using UnityEngine;

public class MuProcInterface : MonoBehaviour
{
    private MuProcMain main;
    
    // Main seed which generates the seeds used to generate the notes.
    [SerializeField] private int musicSeed = 420;
    // Volume. Keep low to prevent hearing damage!
    [SerializeField,Range(0f,0.2f)] private float gain = 0.02f;
    // Sets the beats per minute. Controls the speed.
    [SerializeField,Range(50,150)] private float bpm=100f;
    // Amount of seeds that generate the notes internally, to reduce melodic repetition. Used inner seed changes every 8 notes.
    [SerializeField,Range(1,9999)] private int amountOfInnerSeeds = 4;

    public int MusicSeed
    {
        get => musicSeed;
        set {
            musicSeed = value;
            main.SetSeed(musicSeed);
        }
    }
    public float Gain
    {
        get => gain;
        set {
            gain = Mathf.Clamp(value,0,0.2f);
            main.SetGain(gain);
        }
    }
    public float Bpm
    {
        get => bpm;
        set {
            bpm = Mathf.Clamp(value,50,150);
            main.SetBpm(bpm);
        }
    }
    public int AmountOfInnerSeeds
    {
        get => amountOfInnerSeeds;
        set {
            amountOfInnerSeeds = Math.Clamp(value,1,9999);
            main.SetAmountOfInnerSeeds(amountOfInnerSeeds);
        }
    }

    private void OnValidate()
    {
        if (main == null) return;
        MusicSeed = musicSeed;
        Gain = gain;
        Bpm = bpm;
        AmountOfInnerSeeds = amountOfInnerSeeds;
    }

    private void Awake()
    {
        main = GetComponent<MuProcMain>();
    }

    public void StopMusic()
    {
        main.enabled = false;
    }
    public void StartMusic()
    {
        Gain = gain;
        main.enabled = true;
    }

    public void SetSeedFromString(string input)
    {
        if (input == null) return;
        if (int.TryParse(input, out var seed))
        {
            MusicSeed = seed;
        }
    }
    
    [MenuItem("GameObject/MuProc/MuProc Instance", false, 10)]
    static void CreateMuProcInstance(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = Instantiate((GameObject)Resources.Load("MuProc", typeof(GameObject)));
        go.name = "MuProc Instance";
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
