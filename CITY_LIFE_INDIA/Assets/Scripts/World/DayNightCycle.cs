using UnityEngine;

namespace CityLifeIndia.World
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Sun & Light")]
        [SerializeField] private Light sunLight;
        [SerializeField] private float dayDurationSeconds = 300f; // 5 minute full day cycle

        [Header("Lighting Gradients")]
        [SerializeField] private Gradient skyColorGradient;
        [SerializeField] private Gradient equatorColorGradient;
        [SerializeField] private AnimationCurve sunIntensityCurve;

        [Header("Street Lights / Emissives")]
        [SerializeField] private GameObject[] cityStreetLamps;

        [Range(0f, 24f)]
        [SerializeField] private float timeOfDay = 10.0f; // Start at 10 AM morning

        public float TimeOfDay => timeOfDay;
        public bool IsNight => timeOfDay < 6f || timeOfDay > 18.5f;

        private void Update()
        {
            timeOfDay += (Time.deltaTime / dayDurationSeconds) * 24f;
            if (timeOfDay >= 24f) timeOfDay = 0f;

            UpdateSunRotation();
            UpdateStreetLamps();
        }

        private void UpdateSunRotation()
        {
            // 0 = midnight, 6 = sunrise, 12 = noon, 18 = sunset
            float sunAngle = (timeOfDay / 24f) * 360f - 90f;
            if (sunLight != null)
            {
                sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

                float t = timeOfDay / 24f;
                if (skyColorGradient != null)
                {
                    RenderSettings.ambientSkyColor = skyColorGradient.Evaluate(t);
                }

                if (sunIntensityCurve != null)
                {
                    sunLight.intensity = sunIntensityCurve.Evaluate(t);
                }
            }
        }

        private void UpdateStreetLamps()
        {
            bool night = IsNight;
            if (cityStreetLamps != null)
            {
                foreach (var lamp in cityStreetLamps)
                {
                    if (lamp != null && lamp.activeSelf != night)
                    {
                        lamp.SetActive(night);
                    }
                }
            }
        }
    }
}
