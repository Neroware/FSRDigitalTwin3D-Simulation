using System.Collections.Generic;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller.SFM
{
    [CreateAssetMenu(fileName = "ProxemicData_Default", menuName = "Proxemic/Proxemic Data", order = 1)]
    public class ProxemicData : ScriptableObject
    {
        [System.Serializable]
        public class CategoryVector3
        {
            public string label;
            public Vector3 boundaries; // a, b, c für Triangle Membership
        }

        [System.Serializable]
        public class GainOutput
        {
            public string label;
            public float value;
        }

        [System.Serializable]
        public class FuzzyRule
        {
            public string distanceCategory;
            public string angleCategory;
            public string gainLabel;
        }

        [Header("Distance Categories (a,b,c)")]
        public List<CategoryVector3> distanceCategories = new List<CategoryVector3>()
        {
            new CategoryVector3() { label = "Intimate", boundaries = new Vector3(0f, 0f, 0.45f) },
            new CategoryVector3() { label = "Personal", boundaries = new Vector3(0f, 0.45f, 1.2f) },
            new CategoryVector3() { label = "Social", boundaries = new Vector3(0.45f, 1.2f, 3.6f) },
            new CategoryVector3() { label = "Public", boundaries = new Vector3(1.2f, 3.6f, 3.6f) }
        };

        [Header("Angle Categories (a,b,c)")]
        public List<CategoryVector3> angleCategories = new List<CategoryVector3>()
        {
            new CategoryVector3() { label = "front", boundaries = new Vector3(0f, 0f, 60f) },
            new CategoryVector3() { label = "side-front", boundaries = new Vector3(0f, 60f, 120f) },
            new CategoryVector3() { label = "side-back", boundaries = new Vector3(60f, 120f, 180f) },
            new CategoryVector3() { label = "back", boundaries = new Vector3(120f, 180f, 180f) }
        };

        [Header("Gain Outputs")]
        public List<GainOutput> gainOutputs = new List<GainOutput>()
        {
            new GainOutput() { label = "veryHigh", value = 50f },
            new GainOutput() { label = "high", value = 25f },
            new GainOutput() { label = "medium", value = 12.5f },
            new GainOutput() { label = "low", value = 6.25f },
            new GainOutput() { label = "veryLow", value = 3.125f }
        };

        [Header("Fuzzy Rules")]
        public List<FuzzyRule> fuzzyRules = new List<FuzzyRule>()
        {
            new FuzzyRule() { distanceCategory = "Intimate",  angleCategory = "front",      gainLabel = "veryHigh" },
            new FuzzyRule() { distanceCategory = "Intimate",  angleCategory = "side-front", gainLabel = "high" },
            new FuzzyRule() { distanceCategory = "Intimate",  angleCategory = "side-back",  gainLabel = "medium" },
            new FuzzyRule() { distanceCategory = "Intimate",  angleCategory = "back",       gainLabel = "low" },

            new FuzzyRule() { distanceCategory = "Personal",  angleCategory = "front",      gainLabel = "high" },
            new FuzzyRule() { distanceCategory = "Personal",  angleCategory = "side-front", gainLabel = "high" },
            new FuzzyRule() { distanceCategory = "Personal",  angleCategory = "side-back",  gainLabel = "medium" },
            new FuzzyRule() { distanceCategory = "Personal",  angleCategory = "back",       gainLabel = "low" },

            new FuzzyRule() { distanceCategory = "Social",    angleCategory = "front",      gainLabel = "high" },
            new FuzzyRule() { distanceCategory = "Social",    angleCategory = "side-front", gainLabel = "medium" },
            new FuzzyRule() { distanceCategory = "Social",    angleCategory = "side-back",  gainLabel = "low" },
            new FuzzyRule() { distanceCategory = "Social",    angleCategory = "back",       gainLabel = "low" },

            new FuzzyRule() { distanceCategory = "Public",    angleCategory = "front",      gainLabel = "high" },
            new FuzzyRule() { distanceCategory = "Public",    angleCategory = "side-front", gainLabel = "low" },
            new FuzzyRule() { distanceCategory = "Public",    angleCategory = "side-back",  gainLabel = "low" },
            new FuzzyRule() { distanceCategory = "Public",    angleCategory = "back",       gainLabel = "veryLow" },
        };
    }
}