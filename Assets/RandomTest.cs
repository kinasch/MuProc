using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTest : MonoBehaviour
{
    private static int seed = 420;

    public static void StopOrResume()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Debug.Log(Time.timeScale);
    }
    
    public static void LogRandomNumber()
    {
        Random.InitState(seed);
        Debug.Log(Random.Range(0,100)+" | "+seed);
    }
}
