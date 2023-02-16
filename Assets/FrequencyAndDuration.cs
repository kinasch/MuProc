using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyAndDuration
{
    private double frequency;
    /// <summary>
    /// 0.25 for 1/4 Note
    /// </summary>
    private double duration;

    public FrequencyAndDuration(double frequency, double duration)
    {
        this.frequency = frequency;
        this.duration = duration;
    }

    public double Frequency => frequency;

    public double Duration => duration;
}
