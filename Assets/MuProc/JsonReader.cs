using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MuProc
{
    [Serializable]
    public class JsonReader : MonoBehaviour
    {
        public static Dictionary<string, double[]> GetNoteChances([NotNull]TextAsset jsonData)
        {
            var test = JObject.Parse(jsonData.text);
            Dictionary<string, double[]> result = new Dictionary<string, double[]>();

            foreach (var kvp in test)
            {
                // Check for non-existing and wrong data
                if (kvp.Key == null || kvp.Value == null) continue;
                if(kvp.Key.GetType() != typeof(string) || kvp.Value.GetType() != typeof(double[])) continue;
                
                result.Add(
                    kvp.Key, kvp.Value.ToObject<double[]>()
                );
            }

            return result.Count != 12 ? null : result;
        }
    }
}
