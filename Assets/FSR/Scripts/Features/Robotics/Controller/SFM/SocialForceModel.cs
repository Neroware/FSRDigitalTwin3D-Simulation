// =================================================================================================================================== //
// A Social Force Model (SFM)
//
// This script is based on a project work by Robert Figl
// =================================================================================================================================== //

using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller.SFM
{
    public class SocialForceModel : MonoBehaviour, ISocialForceModel
    {
        [Header("General Settings")]
        [SerializeField] private Transform goal;
        [SerializeField] private ArticulationBody agentArticulationBody;
        [SerializeField] private List<Transform> obstacles = new();
        [SerializeField] private float mass = 1.0f;
        [SerializeField] private bool useArticulationBodyMass = true;

        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 1.0f;
        [Tooltip("Reaction time dt for the agent to respond to forces.")]
        [SerializeField] private float reactionTime = 0.5f;

        [Header("Obstacle Avoidance Settings")]
        [Tooltip("Gain factor for the obstacle repulsive force (when fuzzy inference system is deactivated).")]
        [SerializeField] private float obstacleForceGain = 20.0f;
        [Tooltip("Effective range within which the agent starts reacting to obstacles.")]
        [SerializeField] private float effectiveRange = 1.0f;

        public List<Transform> Obstacles => obstacles;
        public Transform Goal { get => goal; set => goal = value; }
        public Vector3 GoalForce => ComputeGoalForce();
        public Vector3 RepulsiveForce => ComputeRepulsiveForce();
        public Vector3 TotalForce => GoalForce + RepulsiveForce;

        private void Start()
        {
            mass = useArticulationBodyMass ? agentArticulationBody.mass : mass;
        }

        protected virtual float ComputeGain(float distance, float angle) => obstacleForceGain;

        private Vector3 ComputeGoalForce()
        {
            Vector3 directionToGoal = (goal.position - agentArticulationBody.transform.position).normalized;
            Vector3 desiredVelocity = directionToGoal * maxSpeed;
            Vector3 currentVelocity = agentArticulationBody.velocity;
            Vector3 goalForce = mass * (desiredVelocity - currentVelocity) / reactionTime;
            return goalForce;
        }

        private Vector3 ComputeRepulsiveForce()
        {
            Vector3 totalRepulsiveForce = Vector3.zero;
            foreach (Transform obstacle in obstacles)
            {
                // Debug.Log($"Obstacle: {obstacle.name}, Position: {obstacle.position}");
                // Debug.Log($"Number of obstacles: {obstacles.Count}");

                Vector3 direction = (agentArticulationBody.transform.position - obstacle.position).normalized;
                float distance = Vector3.Distance(agentArticulationBody.transform.position, obstacle.position);

                float agentRadius = 0.5f;

                BoxCollider agentCollider = agentArticulationBody.GetComponent<BoxCollider>();
                if (agentCollider != null)
                {
                    Vector2 agentSizeXZ = new(agentCollider.size.x * agentArticulationBody.transform.localScale.x,
                        agentCollider.size.z * agentArticulationBody.transform.localScale.z);
                    agentRadius = agentSizeXZ.magnitude * 0.5f;
                }
                // else
                // {
                //     Debug.LogWarning($"{agentArticulationBody.name} does not have a BoxCollider - using default-radius {agentRadius}");
                // }

                float obstacleRadius = 0.5f; 
                BoxCollider obstacleCollider = obstacle.GetComponent<BoxCollider>();
                if (obstacleCollider != null)
                {
                    Vector2 obstacleSizeXZ = new(
                        obstacleCollider.size.x * obstacle.localScale.x,
                        obstacleCollider.size.z * obstacle.localScale.z);
                    obstacleRadius = obstacleSizeXZ.magnitude * 0.5f;
                }
                // else
                // {
                //     Debug.LogWarning($"{obstacle.name} does not have a BoxCollider - using default-radius {obstacleRadius}");
                // }

                float combinedRadius = agentRadius + obstacleRadius;
                float distanceToObstacle = Mathf.Max(distance, 0.01f); // Prevent divide by zero
                Vector3 directionToObstacle = direction;

                Vector3 toObstacle = obstacle.position - agentArticulationBody.transform.position;
                float angle = Vector3.Angle(agentArticulationBody.transform.forward, toObstacle);

                float k = ComputeGain(distance, angle);
                Vector3 social = Vector3.zero;
                if (useArticulationBodyMass)
                {
                    social = mass * k * Mathf.Exp((combinedRadius - distanceToObstacle) / effectiveRange) * directionToObstacle;
                }
                else
                {
                    social = k * Mathf.Exp((combinedRadius - distanceToObstacle) / effectiveRange) * directionToObstacle;
                }
                Vector3 physical = Vector3.zero;
                if (distanceToObstacle < combinedRadius)
                    physical = k * (combinedRadius - distanceToObstacle) * directionToObstacle;

                totalRepulsiveForce += social + physical;
            }
            return totalRepulsiveForce;
        }
    }
}