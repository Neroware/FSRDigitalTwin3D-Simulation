using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller.SFM
{
    /// <summary>
    /// Visualizes the proxemic zones (Intimate, Personal, Social, Public)
    /// around the GameObject this script is attached to.
    /// Requires a reference to ProxemicDataEditable ScriptableObject
    /// </summary>
    [ExecuteAlways]
    public class ProxemicZoneVisualizerEditable : MonoBehaviour
    {
        [SerializeField] private ProxemicData proxemicData;

        [SerializeField] private Color intimateColor = new Color(1f, 0f, 0f, 0.5f);   // Red
        [SerializeField] private Color personalColor = new Color(1f, 0.5f, 0f, 0.2f); // Orange
        [SerializeField] private Color socialColor = new Color(0f, 0f, 1f, 0.2f);      // Blue
        [SerializeField] private Color publicColor = new Color(0f, 1f, 0f, 0.2f);      // Green

        private void OnDrawGizmos()
        {
            if (proxemicData == null)
            {
                Debug.LogWarning("ProxemicDataEditable is not assigned in ProxemicZoneVisualizerEditable.");
                return;
            }
            DrawZone("Intimate", intimateColor);
            DrawZone("Personal", personalColor);
            DrawZone("Social", socialColor);
            DrawZone("Public", publicColor);
        }

        public void DrawZone(string categoryName, Color color)
        {
            var category = proxemicData.distanceCategories.Find(cat => cat.label == categoryName);
            if (category == null)
                return;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, category.boundaries.z);
        }
    }
}
