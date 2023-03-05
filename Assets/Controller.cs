using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    // Example usage for the MuProc methods via script.
    // For every serialized variable, a null check is recommended. This was not done here to shorten the code.
    // Get a MuProc Instance
    [SerializeField] private MuProcInterface muProcInterface;
    // Textfield to display debug values.
    [SerializeField] private TMP_Text debugValues;

    private void Start()
    {
        SetDisplayValues();
    }

    // Restart music
    public void RestartMusic()
    {
        muProcInterface.StartMusic();
    }
    // Stop music
    public void StopMusic()
    {
        muProcInterface.StopMusic();
    }
    // Resume music generation
    public void ResumeGeneration()
    {
        muProcInterface.ResumeGeneration();
    }
    // Halt music generation
    public void HaltGeneration()
    {
        muProcInterface.HaltGeneration();
    }
    
    
    // Control the values with TextAreas, Sliders or Toggles
    public void SetSeedFromString(string inputSeed)
    {
        if (string.IsNullOrWhiteSpace(inputSeed)) return;
        muProcInterface.MusicSeed = int.Parse(inputSeed);
        SetDisplayValues();
    }
    public void SetGainFromSlider(float gain)
    {
        muProcInterface.Gain = gain;
        SetDisplayValues();
    }
    public void SetBpmFromSlider(float bpm)
    {
        muProcInterface.Bpm = bpm;
        SetDisplayValues();
    }
    public void SetNotesPerInnerSeedFromSlider(float amount)
    {
        muProcInterface.NotesPerInnerSeed = (int)amount;
        SetDisplayValues();
    }
    public void SetAmountOfInnerSeedsFromSlider(float amount)
    {
        muProcInterface.AmountOfInnerSeeds = (int)amount;
        SetDisplayValues();
    }
    public void SetRandomizeRepetitions(bool randomReps)
    {
        muProcInterface.RandomizeRepetitions = randomReps;
        SetDisplayValues();
    }
    public void SetMaxRepetitionsFromSlider(float maxRep)
    {
        muProcInterface.MaxRepetitions = (int)maxRep;
        SetDisplayValues();
    }
    public void SetMaxValueForRepetitionsWhenRandomizedFromSlider(float randomMaxRep)
    {
        muProcInterface.MaxValueForRandomizedRepetitions = (int)randomMaxRep;
        SetDisplayValues();
    }

    private void SetDisplayValues()
    {
        debugValues.text = "Debug Values\n\n"+muProcInterface.GetDebugValues();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
