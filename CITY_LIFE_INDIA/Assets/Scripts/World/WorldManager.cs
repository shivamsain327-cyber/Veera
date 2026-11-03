using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityLifeIndia.World
{
    [System.Serializable]
    public class CityDistrict
    {
        public string districtName;
        public Vector3 center;
        public float radius;
        public string description;
    }

    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        [Header("City Districts (GDD Section 2)")]
        [SerializeField] private List<CityDistrict> districts = new List<CityDistrict>();

        [Header("World Settings")]
        [SerializeField] private float cityBoundarySize = 1000f; // 1km x 1km

        private CityDistrict currentDistrict;
        public event Action<CityDistrict> OnDistrictEntered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializeDistricts();
        }

        private void InitializeDistricts()
        {
            if (districts.Count == 0)
            {
                districts.Add(new CityDistrict { districtName = "City Center Plaza", center = Vector3.zero, radius = 120f, description = "Main roads, central offices and fountains" });
                districts.Add(new CityDistrict { districtName = "Old Johari Market", center = new Vector3(80, 0, 50), radius = 100f, description = "Traditional Indian shops, spice stalls and dhabas" });
                districts.Add(new CityDistrict { districtName = "Hawa Palace Heritage", center = new Vector3(50, 0, -220), radius = 110f, description = "Historical architecture and tourist hotspots" });
                districts.Add(new CityDistrict { districtName = "Green Meadows Residential", center = new Vector3(-120, 0, 100), radius = 130f, description = "Modern apartments and quiet neighbourhood streets" });
                districts.Add(new CityDistrict { districtName = "Transport Hub & Terminal", center = new Vector3(-200, 0, -100), radius = 120f, description = "Bus terminal and interstate taxi stand" });
                districts.Add(new CityDistrict { districtName = "Amber Hill Overlook", center = new Vector3(250, 15, 200), radius = 150f, description = "Scenic viewpoint overlooking Rajasthan valley" });
            }
        }

        private void Update()
        {
            CheckPlayerDistrict();
        }

        private void CheckPlayerDistrict()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            Vector3 playerPos = player.transform.position;
            CityDistrict matched = null;

            foreach (var d in districts)
            {
                if (Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), new Vector3(d.center.x, 0, d.center.z)) <= d.radius)
                {
                    matched = d;
                    break;
                }
            }

            if (matched != null && matched != currentDistrict)
            {
                currentDistrict = matched;
                OnDistrictEntered?.Invoke(currentDistrict);
                Debug.Log($"[WorldManager] Entered district: {currentDistrict.districtName}");
            }
        }
    }
}
