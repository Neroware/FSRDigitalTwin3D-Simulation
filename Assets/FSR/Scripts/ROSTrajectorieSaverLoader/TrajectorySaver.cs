// TrajectorySaver.cs
// Speichert Request + Response als JSON-Datei (Newtonsoft.Json)

using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using RosMessageTypes.Ur5eMoveit;

public static class TrajectorySaver
{
    private static readonly string CacheDir = Path.Combine(Application.dataPath, "Trajectories");

    /// <summary>
    /// Speichert das Request/Response-Paar als JSON in Application.persistentDataPath/TrajectoryCache.
    /// Dateiname: cache_{GUID}.json
    /// </summary>
    public static void Save(string name, MoverServiceResponse response)
    {
        if (!Directory.Exists(CacheDir))
            Directory.CreateDirectory(CacheDir);

        var cache = new CacheFile
        {
            response = ExtractHelpers.convertOriToResponseData(response)
        };

        string path = Path.Combine(CacheDir, name) + ".json";

        try
        {
            string json = JsonConvert.SerializeObject(cache, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log($"[TrajectorySaver] Gespeichert: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TrajectorySaver] Fehler beim Speichern: {ex}");
        }
    }
    
    // public static void Save(MoverServiceRequest request, MoverServiceResponse response)
    // {
    //     if (!Directory.Exists(CacheDir))
    //         Directory.CreateDirectory(CacheDir);

    //     var cache = new CacheFile
    //     {
    //         request = ExtractHelpers.convertOriToRequestData(request),
    //         response = ExtractHelpers.convertOriToResponseData(response)
    //     };

    //     string filename = $"cache_{Guid.NewGuid()}.json";
    //     string path = Path.Combine(CacheDir, filename);

    //     try
    //     {
    //         string json = JsonConvert.SerializeObject(cache, Formatting.Indented);
    //         File.WriteAllText(path, json);
    //         Debug.Log($"[TrajectorySaver] Gespeichert: {path}");
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError($"[TrajectorySaver] Fehler beim Speichern: {ex}");
    //     }
    // }
}
