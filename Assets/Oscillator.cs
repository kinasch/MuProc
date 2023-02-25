using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Oscillator : MonoBehaviour
{
    private double increment;
    private double phase,bassPhase;
    private const double samplingFrequency = 48000.0;

    [SerializeField] private int musicSeed = 420;
    private int seed;
    private int[] innerSeeds;

    [SerializeField] private float gain;
    private float currentGain;
    [Range(75,125)] public float bpm=100f;
    [SerializeField] private int amountOfInnerSeeds = 4;
    
    private double time=0.0;
    private double duration = 0;
    private Note oldNote, currNote;
    private float randomValue;
    private bool changeNote = false;

    private readonly Dictionary<string, double[]> chances = new Dictionary<string, double[]>()
    {
        {"C#",new double[]{0.055,0.0519,0.0744,0.0706,0.2895,0.0641,0.0631,0.0765,0.0374,0.1063,0.0165,0.0947}},
        { "G#",new double[]{0.0441, 0.07, 0.0729, 0.0904, 0.0602, 0.0384, 0.0677, 0.0503, 0.0397, 0.1072, 0.0436, 0.3156} },
        { "D#",new double[]{0.0237, 0.0763, 0.0478, 0.0945, 0.0531, 0.0752, 0.2642, 0.0599, 0.0757, 0.0756, 0.0626, 0.0915} },
        { "B",new double[]{0.0756, 0.0467, 0.2355, 0.1226, 0.042, 0.0877, 0.0258, 0.0942, 0.0169, 0.1534, 0.0477, 0.052} },
        { "F#",new double[]{0.0392, 0.0545, 0.1027, 0.0814, 0.0398, 0.0672, 0.0302, 0.0647, 0.0263, 0.3924, 0.0345, 0.0671} },
        { "C",new double[]{0.0478, 0.0591, 0.0923, 0.2844, 0.0279, 0.1, 0.0473, 0.0667, 0.0701, 0.0844, 0.0708, 0.0493} },
        { "G",new double[]{0.0916, 0.0574, 0.0486, 0.1107, 0.01, 0.1086, 0.0363, 0.0726, 0.0729, 0.052, 0.3032, 0.0359} },
        { "D",new double[]{0.0736, 0.0471, 0.062, 0.1084, 0.0313, 0.3049, 0.0382, 0.0897, 0.0564, 0.073, 0.0884, 0.0272} },
        { "A",new double[]{0.3213, 0.0469, 0.0797, 0.082, 0.0292, 0.1015, 0.0161, 0.0976, 0.0463, 0.0571, 0.0855, 0.0368} },
        { "E",new double[]{0.0947, 0.0286, 0.0877, 0.0847, 0.0355, 0.0929, 0.0334, 0.2788, 0.0528, 0.0984, 0.0733, 0.0392} },
        { "F",new double[]{0.0582, 0.0776, 0.0271, 0.1204, 0.0259, 0.0775, 0.0587, 0.073, 0.2864, 0.0471, 0.0959, 0.0522} },
        { "A#",new double[]{0.0585, 0.2688, 0.0621, 0.1114, 0.0407, 0.0718, 0.0614, 0.0323, 0.077, 0.0891, 0.0603, 0.0666} }
    };

    private readonly List<Note> notes = new List<Note>()
    {
        new("C#", 277.18),
        new( "G#", 392.00),
        new( "D#", 311.13),
        new( "B", 493.88),
        new( "F#", 369.99),
        new( "C", 261.63),
        new( "G", 415.30),
        new( "D", 293.66),
        new( "A", 440.00),
        new( "E", 329.63),
        new( "F", 349.23),
        new( "A#", 466.16)
    };
    
    private readonly List<Note> bass = new List<Note>()
    {
        new("C2", 65.41),
        new( "E2", 82.41),
        new( "G2", 98.00)
    };
    private int bassDirection = 1;
    private int currBaseIndex = 0;
    private Note bassNote;

    private int repetitionCounter = 0;
    private int currentSeedNumber = 0;
    private int sameNoteAmount = 0,sameNoteMax=2;

    private void Start()
    {
        
        Random.InitState(musicSeed);
        innerSeeds = CreateSeedList(musicSeed, 10000);

        seed = innerSeeds[currentSeedNumber];
        
        Random.InitState(seed);

        duration = Random.Range(0.125f,0.25f) * 4 * (60 / bpm);
        currentGain = gain;
        
        currNote = notes[Random.Range(0,notes.Count)];
        oldNote = new Note(currNote);

        bassNote = bass[Random.Range(0, bass.Count)];
        bassDirection = Random.value > 0.5 ? 1 : -1;
        currBaseIndex = Random.value > 0.5 ? 0 : bass.Count-1;

        randomValue = Random.value;

        sameNoteMax = Random.Range(2, 4);
        Debug.Log("sameNoteMax: "+sameNoteMax);
    }

    private void Update()
    {
        // Stops working once Time.timeScale = 0 -> Fix?
        time += Time.deltaTime;
        currentGain = Mathf.Lerp(gain, 0, (float)time);
        if (time >= duration)
        {
            changeNote = true;
            currentGain = gain;
            time = 0.0;
            duration = 0.125 * 4 * (60 / bpm);
            repetitionCounter++;
        }
        
        if (repetitionCounter > 8)
        {
            repetitionCounter %= 8;
            ChangeInnerSeed();
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (changeNote)
        {
            currNote = GetNewNote(oldNote);
            oldNote = new Note(currNote);
            
            currBaseIndex = (currBaseIndex + bassDirection) % bass.Count;
            bassNote = bass[Math.Abs(currBaseIndex)];
            
            Debug.Log("Note: "+currNote.Name+" | Bass: "+bassNote.Name+"\n"+seed);
            changeNote = false;
        }

        increment = ( currNote.Frequency * 2.0 * Mathf.PI / samplingFrequency);
        var bassInc = (bassNote.Frequency * 2.0 * Mathf.PI / samplingFrequency);
        
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            bassPhase += bassInc;

            var mainSin = Mathf.Sin((float)phase);
            var bassSin = Mathf.Sin((float)bassPhase);

            data[i] = (float)(currentGain * (mainSin+0.5*bassSin) /*+ (currentGain*0.9f) * Mathf.Sin((float)bassPhase) /* + new wave */);

            /*var flipper = data[i] >= 0 ? 1 : -1;
            data[i] = (flipper * (float)currentGain) * 0.6f;*/
            

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            phase %= (Mathf.PI * 2);
            bassPhase %= (Mathf.PI * 2);
        }
    }

    private Note GetNewNote(Note prevNote)
    {
        double[] notePercentages = SumEveryDoubleArrayValueWithItsPreviousValues(chances[prevNote.Name]);
        // Sorted alphabetically (for whatever reason)
        string[] correspondingNote = {"A", "A#","B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
        float tempRdmValue = repetitionCounter % 2 == 0 ? randomValue : 1 - randomValue;
        for (var i=notePercentages.Length-1; i>=0;i--)
        {
            if (tempRdmValue > notePercentages[i])
            {
                var newName = correspondingNote[i];

                // Prevent same note repetition.
                // Replace with something more elegant later on.
                if (newName == prevNote.Name)
                {
                    sameNoteAmount++;
                    /*var num = Array.IndexOf(correspondingNote, prevNote.Name) + (int)Math.Floor(randomValue*12);
                    var altNote = FindInNotes(correspondingNote[num%correspondingNote.Length]);
                    return altNote;*/
                }
                else
                {
                    sameNoteAmount = 0;
                }

                if (sameNoteAmount >= sameNoteMax)
                {
                    var num = Array.IndexOf(correspondingNote, prevNote.Name) + (int)Math.Floor(randomValue*12);
                    var altNote = FindInNotes(correspondingNote[num%correspondingNote.Length]);
                    return altNote;
                }

                var returnNote = FindInNotes(newName);
                return returnNote;
            }
        }
        return null;
    }

    /// <summary>
    /// That's resource hungry.
    /// Change!
    /// </summary>
    private double[] SumEveryDoubleArrayValueWithItsPreviousValues(double[] array)
    {
        var newArray = new double[array.Length];
        //array.CopyTo(newArray,0);
        newArray[0] = 0;
        for (int i = 1; i < array.Length; i++)
        {
            newArray[i] += (newArray[i - 1] + array[i-1]);
        }
        
        return newArray;
    }

    private Note FindInNotes(string nameToLookup)
    {
        foreach (var note in notes)
        {
            if (note.Name == nameToLookup) return note;
        }

        return null;
    }
    
    public void SetSeed(string inputSeed)
    {
        this.musicSeed = int.Parse(inputSeed);
        
        Random.InitState(musicSeed);
        
        innerSeeds = CreateSeedList(musicSeed, 10000);

        seed = innerSeeds[0];
        
        Random.InitState(seed);
        
        this.randomValue = Random.value;
    }
    
    private void ChangeInnerSeed()
    {
        currentSeedNumber = (currentSeedNumber + 1) % innerSeeds.Length;

        this.seed = innerSeeds[currentSeedNumber];
        
        Random.InitState(seed);
        randomValue = Random.value;
    }

    private int[] CreateSeedList(int initialSeed, int max)
    {
        Random.InitState(initialSeed);
        
        int[] newSeeds = new int[amountOfInnerSeeds];
        for (int i = 0; i < amountOfInnerSeeds; i++)
        {
            // Usually first multiplying and then dividing is better.
            int rangeMax = max / amountOfInnerSeeds;
            newSeeds[i] = Random.Range(i * rangeMax, (i + 1) * rangeMax);
        }

        return newSeeds;
    }
}