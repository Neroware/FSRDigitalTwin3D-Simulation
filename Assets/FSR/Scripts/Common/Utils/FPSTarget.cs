using UnityEngine;

namespace FSR.DigitalTwin.Client.Common.Utils {
    public class FPSTarget : MonoBehaviour
    {
        public int target = 60;
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = target;
        }

        private void Update()
        {
            if(Application.targetFrameRate != target)
                Application.targetFrameRate = target;
        }
    }

}