// TrajectoryLoader.cs
// Lädt alle JSON-Caches, konvertiert/vergleich und findet Matches

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using RosMessageTypes.Ur5eMoveit;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public static class TrajectoryLoader
{
    private static readonly string CacheDir = Path.Combine(Application.dataPath, "Trajectories");

    /// Lädt alle CacheFile JSON-Dateien aus dem Cache-Ordner.
    // public static List<CacheFile> LoadAll()
    // {
    //     var list = new List<CacheFile>();
    //     if (!Directory.Exists(CacheDir)) return list;

    //     var files = Directory.GetFiles(CacheDir, "*.json");
    //     foreach (var f in files)
    //     {
    //         try
    //         {
    //             string json = File.ReadAllText(f);
    //             var cache = JsonConvert.DeserializeObject<CacheFile>(json);
    //             if (cache != null) list.Add(cache);
    //         }
    //         catch (Exception ex)
    //         {
    //             Debug.LogError($"[TrajectoryLoader] Fehlschlag beim Laden {f}: {ex.Message}");
    //         }
    //     }
    //     Debug.Log($"[TrajectoryLoader] Geladene Caches: {list.Count}");
    //     return list;
    // }

    public static List<CacheFile> LoadAll()
    {
        var list = new List<CacheFile>();
        if (!Directory.Exists(CacheDir)) return list;

        var files = Directory.GetFiles(CacheDir, "*.json");
        foreach (var f in files)
        {
            try
            {
                string json = File.ReadAllText(f);
                var cache = JsonConvert.DeserializeObject<CacheFile>(json);
                if (cache != null)
                {
                    cache.filename = Path.GetFileNameWithoutExtension(f);
                    list.Add(cache);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TrajectoryLoader] Fehlschlag beim Laden {f}: {ex.Message}");
            }
        }
        Debug.Log($"[TrajectoryLoader] Geladene Caches: {list.Count}");
        return list;
    }

    public static MoverServiceResponse isAvaliable(string name)
    {
        List<CacheFile> files = LoadAll();
        foreach (CacheFile file in files)
        {
            if (file.filename == name)
            {
                return ExtractHelpers.convertResponseDataToOri(file.response);
            }
        }
        return null;
    }

    // public static MoverServiceResponse isAvaliable(MoverServiceRequest req)
    // {
    //     RequestData ori_req = ExtractHelpers.convertOriToRequestData(req);
    //     List<CacheFile> files = LoadAll();
    //     foreach (CacheFile file in files)
    //     {
    //         if (isEqual(file.request, ori_req))
    //         {
    //             return ExtractHelpers.convertResponseDataToOri(file.response);
    //         }
    //     }
    //     return null;
    // }

    // public static bool isEqual(RequestData a, RequestData b)
    // {
    //     if (!AreJointInputsEqual(a, b))
    //     {
    //         return false;
    //     }
    //     else if (!ArePoseDataEqual(a.joints_input.pick_pose, b.joints_input.pick_pose))
    //     {
    //         return false;
    //     }
    //     else if (!ArePoseDataEqual(a.joints_input.place_pose, b.joints_input.place_pose))
    //     {
    //         return false;
    //     }
    //     else if (!ArePoseDataEqual(a.pick_pose, b.pick_pose))
    //     {
    //         return false;
    //     }
    //     else if (!ArePoseDataEqual(a.place_pose, b.place_pose))
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         return true;
    //     }
    // }

    // public static bool AreJointInputsEqual(RequestData a, RequestData b, double tolerance = 1e-4)
    // {
    //     if (a?.joints_input == null || b?.joints_input == null)
    //         return false;
    //     if (a.joints_input.joints_input.Length != b.joints_input.joints_input.Length)
    //         return false;
        
    //     for (int i = 0; i < a.joints_input.joints_input.Length; i++)
    //     {
    //         double diff = Math.Abs(a.joints_input.joints_input[i] - b.joints_input.joints_input[i]);
    //         if (diff > tolerance)
    //         {
    //             Debug.Log($"[AreJointInputsEqual] Unterschied an Index {i}: {a.joints_input.joints_input[i]} vs {b.joints_input.joints_input[i]} (Δ={diff})");
    //             return false;
    //         }
    //     }

    //     return true;
    // }

    // public static bool ArePoseDataEqual(PoseData a, PoseData b, double tolerance = 1e-4)
    // {
    //     if (a == null || b == null) return false;

    //     if (a.position == null || b.position == null || a.position.Length != b.position.Length)
    //         return false;
    //     if (a.orientation == null || b.orientation == null || a.orientation.Length != b.orientation.Length)
    //         return false;

    //     for (int i = 0; i < a.position.Length; i++)
    //         if (Math.Abs(a.position[i] - b.position[i]) > tolerance)
    //             return false;

    //     for (int i = 0; i < a.orientation.Length; i++)
    //         if (Math.Abs(a.orientation[i] - b.orientation[i]) > tolerance)
    //             return false;

    //     return true;
    // }
}