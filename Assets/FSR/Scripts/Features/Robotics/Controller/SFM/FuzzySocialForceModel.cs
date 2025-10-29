using System.Collections.Generic;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller.SFM
{
    /// <summary>
    /// A hard-coded fuzzy social force model using TriangleMembership and Mamdani-Inference
    /// to adapt the force gain of the robot depending on proxemic data
    /// </summary>
    public class FuzzySocialForceModel : SocialForceModel
    {
        private Dictionary<string, Vector3> distanceCategories;
        private Dictionary<string, Vector3> angleCategories;
        private Dictionary<string, float> gainOutputs;
        private Dictionary<(string distance, string angle), string> fuzzyRules;
        [SerializeField] public ProxemicData proxemicData;

        private void Awake()
        {
            distanceCategories = new Dictionary<string, Vector3>();
            foreach (var cat in proxemicData.distanceCategories)
            {
                distanceCategories[cat.label] = cat.boundaries;
            }

            angleCategories = new Dictionary<string, Vector3>();
            foreach (var cat in proxemicData.angleCategories)
            {
                angleCategories[cat.label] = cat.boundaries;
            }

            gainOutputs = new Dictionary<string, float>();
            foreach (var gain in proxemicData.gainOutputs)
            {
                gainOutputs[gain.label] = gain.value;
            }

            fuzzyRules = new Dictionary<(string, string), string>();
            foreach (var rule in proxemicData.fuzzyRules)
            {
                fuzzyRules[(rule.distanceCategory, rule.angleCategory)] = rule.gainLabel;
            }
        }

        private float TriangleMembershipFunction(float x, float a, float b, float c)
        {
            if (x < a || x > c)
                return 0.0f;
            if (x <= b)
                return (x - a) / (b - a);
            if (x <= c)
                return (c - x) / (c - b);
            else
                Debug.LogError("TriangleMembershipFunction: This should never happen, x is not in the range of a, b, c");
            return 0.0f;
        }
        
        private Dictionary<string, float> ComputeDegreeOfMembership(float x, Dictionary<string, Vector3> categories)
        {
            Dictionary<string, float> categoryValues = new(); 
            foreach (var category in categories)
            {
                string label = category.Key;
                var vec = category.Value;
                categoryValues[label] = TriangleMembershipFunction(x, vec.x, vec.y, vec.z);
            }
            return categoryValues;
        }

        protected override float ComputeGain(float distance, float angle)
        {
            // Debug.Log($"ComputeGain Input - distance: {distance}, angle: {angle}");

            // Step 1: Fuzzification
            var distanceMembershipDegree = ComputeDegreeOfMembership(distance, distanceCategories);
            var angleMembershipDegree = ComputeDegreeOfMembership(angle, angleCategories);

            float numerator = 0.0f;
            float denominator = 0.0f;

            // Step 2: Rule Evaluation
            foreach (var distanceCategory in distanceMembershipDegree)
            {
                string distanceLabel = distanceCategory.Key;
                float distanceDegree = distanceCategory.Value;
                foreach (var angleCategory in angleMembershipDegree)
                {
                    string angleLabel = angleCategory.Key;
                    float angleDegree = angleCategory.Value;
                    if (fuzzyRules.TryGetValue((distanceLabel, angleLabel), out string gainLabel))
                    {
                        float gainValue = gainOutputs[gainLabel];
                        float minDegree = Mathf.Min(distanceDegree, angleDegree);
                        numerator += minDegree * gainValue;
                        denominator += minDegree;
                    }
                }
            }

            // Step 3: Defuzzification (Weighted Average)
            return (denominator > 0.0f) ? numerator / denominator : 0.0f;
        }
    }
}