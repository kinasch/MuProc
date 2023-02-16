using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Oscillator : MonoBehaviour
{
    private double increment;
    private double phase;
    private int cord = 0;
    private double samplingFrequency = 48000.0;

    [SerializeField] private float gain;
    private float currentGain;
    [SerializeField] private float bpm=100f;
    private double time=0.0;
    [SerializeField] private List<FrequencyAndDuration> frequencies;
    private double duration = 0;

    private void Start()
    {
        frequencies = new List<FrequencyAndDuration>()
        {
            new(200.0,0.125),
            new(250.0,0.125),
            new(300.0,0.125),
            new(350.0,0.125),
            new(400.0,0.25),
            new(400.0,0.25),
            
            new(450.0,0.125),
            new(450.0,0.125),
            new(450.0,0.125),
            new(450.0,0.125),
            new(400.0,0.25),
            new(0.0,0.25),
            
            new(450.0,0.125),
            new(450.0,0.125),
            new(450.0,0.125),
            new(450.0,0.125),
            new(400.0,0.25),
            new(0.0,0.25),
            
            new(350.0,0.125),
            new(350.0,0.125),
            new(350.0,0.125),
            new(350.0,0.125),
            new(300.0,0.25),
            new(300.0,0.25),
            
            new(400.0,0.125),
            new(400.0,0.125),
            new(400.0,0.125),
            new(400.0,0.125),
            new(200.0,0.5),
            
            
        };

        duration = frequencies[0].Duration * 4 * (60/bpm) * 2; // 2|4 für alle meine Entchen, deswegen * 2 am Ende, bei 4/4 nichts
        currentGain = gain;
    }

    private void Update()
    {
        time += Time.deltaTime;
        currentGain = Mathf.Lerp(gain, 0, (float)time);
        if (time >= duration)
        {
            currentGain = gain;
            cord = (cord + 1) % frequencies.Count;
            time = 0.0;
            duration = frequencies[cord].Duration * 4 * (60 / bpm);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = (frequencies[cord].Frequency * 2.0 * Mathf.PI / samplingFrequency);
        
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            data[i] = (float)(currentGain * (Mathf.Sin((float)phase) /* + new wave */));

            var flipper = data[i] >= 0 ? 1 : -1;
            data[i] = (flipper * (float)currentGain) * 0.6f;

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            phase %= (Mathf.PI * 2);
        }
    }
}
