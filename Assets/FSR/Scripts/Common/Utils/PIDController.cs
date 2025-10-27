using UnityEngine;

namespace FSR.DigitalTwin.Client.Common.Utils
{
    [System.Serializable]
    public class PIDController
    {
        public float Kp { init; get; } = 5.0f;
        public float Ki { init; get; } = 0.01f;
        public float Kd { init; get; } = 1.0f;
        public float IntegralLimit { init; get; } = 10.0f;
        public float DerivativeLimit { init; get; } = 5.0f;

        private float _integral;
        private float _lastError;

        public PIDController() { }
        public PIDController(float kp, float ki, float kd)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
        }

        public float NextValue(float error, float deltaTime)
        {
            _integral += error * deltaTime;
            _integral = Mathf.Clamp(_integral, -IntegralLimit, IntegralLimit);

            float derivative = (error - _lastError) / Mathf.Max(deltaTime, 0.0001f);
            derivative = Mathf.Clamp(derivative, -DerivativeLimit, DerivativeLimit);

            float output = Kp * error + Ki * _integral + Kd * derivative;

            _lastError = error;

            return output;
        }

        public void Reset() => _integral = _lastError = 0.0f;

    }
}