using System;
using System.Collections;
using System.Collections.Generic;
using MuProc;
using UnityEditor;
using UnityEngine;

public class MuProcInterface : MonoBehaviour
{
    private MuProcMain main;
    
    [Header("The main seed used for generation")]
    // Main seed which generates the seeds used to generate the notes.
    [SerializeField] private int musicSeed = 420;
    [Space (5)]
    
    [Header("Music Control")]
    [Space (-5)]
    [Header("Control the volume (keep low to reduce hearing damage):")]
    // Volume. Keep low to prevent hearing damage!
    [SerializeField,Range(0f,0.2f)] private float gain = 0.02f;
    [Space (-5)]
    [Header("Control the bpm or speed of the music:")]
    // Sets the beats per minute. Controls the speed.
    [SerializeField,Range(50,150)] private float bpm=100f;
    [Space (5)]
    
    [Header("Other Generation Control")]
    [Space (-5)]
    [Header("Set the amount of inner seeds. Every 8 note, a new inner seed from the amount of inner seeds is chosen.")]
    // Amount of seeds that generate the notes internally, to reduce melodic repetition. Used inner seed changes every 8 notes.
    [SerializeField,Range(1,9999)] private int amountOfInnerSeeds = 4;
    [Space (-5)]
    [Header("Whether the maximum allowed repetition amount of one note should be random or set.")]
    // Check if amount of allowed repetitions per note should be randomly generated between 1 and the following int variable.
    [SerializeField] private bool randomizeRepetitions = true;
    [Space (-5)]
    [Header("Set the highest value that can be the result for the maximum allowed repetition amount.")]
    [Space (-10)]
    [Header("(Only works when randomizeRepetitions is true)")]
    [SerializeField,Range(1,100)] private int maxValueForRandomizedRepetitions = 4;
    [Space (-5)]
    [Header("Set the maximum allowed repetition amount.")]
    [Space (-10)]
    [Header("(Only works when randomizeRepetitions is false)")]
    // Amount of set repetitions if they should not be randomly set.
    [SerializeField,Range(1,100)] private int maxRepetitions = 4;
    [Space (5)]
    [Header("Set this to a json file similar to the ones from the Resources to change the note chances.")]
    // JSON file with the chances for each of the 12 notes.
    [SerializeField] private TextAsset noteChances;

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
    public bool RandomizeRepetitions
    {
        get => randomizeRepetitions;
        set {
            randomizeRepetitions = value;
            main.SetRandomizeRepetitions(randomizeRepetitions);
        }
    }

    public int MaxValueForRandomizedRepetitions
    {
        get => maxValueForRandomizedRepetitions;
        set {
            maxValueForRandomizedRepetitions = Math.Clamp(value,1,100);
            main.SetMaxValueForRepetitionsWhenRandomized(maxValueForRandomizedRepetitions);
        }
    }

    public int MaxRepetitions
    {
        get => maxRepetitions;
        set {
            maxRepetitions = Math.Clamp(value,1,100);
            main.SetMaxRepetitions(maxRepetitions);
        }
    }

    public TextAsset NoteChances
    {
        get => noteChances;
        set {
            if (value == null) return;
            noteChances = value;
            var chances = JsonReader.GetNoteChances(noteChances);
            if (chances == null) return;
            main.SetNoteChances(chances);
        }
    }
    public bool GetMusicPlaying()
    {
        return main.enabled;
    }

    public string GetDebugValues()
    {
        return main.GetDebugValues();
    }
    
    

    private void OnValidate()
    {
        if (main == null) return;
        MusicSeed = musicSeed;
        Gain = gain;
        Bpm = bpm;
        AmountOfInnerSeeds = amountOfInnerSeeds;
        RandomizeRepetitions = randomizeRepetitions;
        MaxValueForRandomizedRepetitions = maxValueForRandomizedRepetitions;
        MaxRepetitions = maxRepetitions;
        NoteChances = noteChances;
    }

    private void Awake()
    {
        main = GetComponent<MuProcMain>();
        if (noteChances == null) return;
        var chances = JsonReader.GetNoteChances(noteChances);
        if (chances == null) return;
        main.SetNoteChances(chances);
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
    
    

#if UNITY_EDITOR
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
#endif
}
