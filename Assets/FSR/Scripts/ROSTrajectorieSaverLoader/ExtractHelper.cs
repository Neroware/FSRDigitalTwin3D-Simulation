using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Moveit;
using RosMessageTypes.Trajectory;
using RosMessageTypes.Ur5eMoveit;
using UniRx;
using UnityEngine;

public static class ExtractHelpers
{
    // public static RequestData convertOriToRequestData(MoverServiceRequest req)
    // {
    //     var rd = new RequestData
    //     {
    //         joints_input = new Joints {
    //             joints_input = req.joints_input.joints,
    //             pick_pose = PoseDataGenerator(req.joints_input.pick_pose),
    //             place_pose = PoseDataGenerator(req.joints_input.pick_pose)
    //         },
    //         pick_pose = PoseDataGenerator(req.pick_pose),
    //         place_pose = PoseDataGenerator(req.place_pose)
    //     };
    //     return rd;
    // }
    
    // public static PoseData PoseDataGenerator(PoseMsg p)
    // {
    //     var pd = new PoseData
    //     {
    //         position = new double[] { p.position.x, p.position.y, p.position.z },
    //         orientation = new double[] { p.orientation.x, p.orientation.y, p.orientation.z, p.orientation.w}
    //     };
    //     return pd;
    // }
    
    public static ResponseData convertOriToResponseData(MoverServiceResponse resp)
    {
        //if (resp?.trajectories == null) return new ResponseData();

        var traj_tmp = new TrajectoryData[resp.trajectories.Length];

        int i = 0;
        foreach (var traj in resp.trajectories)
        {

            var jt_tmp = new JointTrajectoryPoints[traj.joint_trajectory.points.Length];
            if (traj?.joint_trajectory?.points != null)
            {
                int n = 0;
                foreach (var p in traj.joint_trajectory.points)
                {
                    jt_tmp[n] = new JointTrajectoryPoints { positions = p.positions ?? new double[0] };
                    n++;
                }
            }
            traj_tmp[i] = new TrajectoryData { joint_trajectory = new JointTrajectory { points = jt_tmp } };
            i++;
        }
        return new ResponseData { trajectories = traj_tmp };
    }
    
    public static MoverServiceResponse convertResponseDataToOri(ResponseData resp)
    {
        var response = new MoverServiceResponse
        {
            trajectories = resp.trajectories.Select(traj => new RobotTrajectoryMsg
            {
                joint_trajectory = new JointTrajectoryMsg
                {
                    header = null,
                    joint_names = null,
                    points = traj.joint_trajectory.points.Select(point => new JointTrajectoryPointMsg
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
        return response;
    }
}