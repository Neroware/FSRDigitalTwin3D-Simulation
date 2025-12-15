using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RosMessageTypes.Moveit;
using RosMessageTypes.Trajectory;
using RosMessageTypes.FsrMoveit;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils
{
    public static class TrajectoryHelper 
    {
        private static readonly string CACHE_DIRECTORY = Path.Combine(Application.dataPath, "Moveit.Trajectories");

        public static ResponseData ToPickAndPlaceResponseData(this PickAndPlaceServiceResponse resp)
        {
            var traj_ = new TrajectoryData[resp.trajectories.Length];
            int i = 0;
            foreach (var traj in resp.trajectories)
            {
                var jt_ = new JointTrajectoryPoints[traj.joint_trajectory.points.Length];
                if (traj?.joint_trajectory?.points != null)
                {
                    int n = 0;
                    foreach (var p in traj.joint_trajectory.points)
                    {
                        jt_[n] = new JointTrajectoryPoints { positions = p.positions ?? new double[0] };
                        n++;
                    }
                }
                traj_[i] = new TrajectoryData { jointTrajectory = new JointTrajectory { points = jt_ } };
                i++;
            }
            return new ResponseData { trajectories = traj_ };
        }

        public static PickAndPlaceServiceResponse ToMoveItPickAndPlaceServiceResponse(this ResponseData resp) => new()
            {
                trajectories = resp.trajectories.Select(traj => new RobotTrajectoryMsg
                {
                    joint_trajectory = new JointTrajectoryMsg
                    {
                        header = null,
                        joint_names = null,
                        points = traj.jointTrajectory.points.Select(point => new JointTrajectoryPointMsg
                        {           
                            positions = point.positions,
                            velocities = null,
                            accelerations = null,
                            effort = null,
                            time_from_start = null
                        }).ToArray()
                    },
                    multi_dof_joint_trajectory = null
                }).ToArray()
            };
        
        /// <summary>
        /// Speichert das Request/Response-Paar als JSON in Application.persistentDataPath/TrajectoryCache.
        /// Dateiname: cache_{GUID}.json
        /// </summary>
        public static void Save(string name, PickAndPlaceServiceResponse response)
        {
            if (!Directory.Exists(CACHE_DIRECTORY))
                Directory.CreateDirectory(CACHE_DIRECTORY);
            var cache = new CacheFile
            {
                response = response.ToPickAndPlaceResponseData()
            };
            string path = Path.Combine(CACHE_DIRECTORY, name) + ".json";
            try
            {
                string json = JsonConvert.SerializeObject(cache, Formatting.Indented);
                File.WriteAllText(path, json);
                Debug.Log($"[TrajectoryHelper] Gespeichert: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TrajectoryHelper] Fehler beim Speichern: {ex}");
            }
        }

        public static List<CacheFile> LoadAll()
        {
            var list = new List<CacheFile>();
            if (!Directory.Exists(CACHE_DIRECTORY)) return list;

            var files = Directory.GetFiles(CACHE_DIRECTORY, "*.json");
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
                    Debug.LogError($"[TrajectoryHelper] Fehlschlag beim Laden {f}: {ex.Message}");
                }
            }
            Debug.Log($"[TrajectoryHelper] Geladene Caches: {list.Count}");
            return list;
        }

        public static bool IsAvaliable(string name, out PickAndPlaceServiceResponse response)
        {
            List<CacheFile> files = LoadAll();
            foreach (CacheFile file in files)
            {
                if (file.filename == name)
                {
                    response = file.response.ToMoveItPickAndPlaceServiceResponse();
                    return true;
                }
            }
            response = null;
            return false;
        }
    } // class TrajectoryHelper
} // namespace FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils

