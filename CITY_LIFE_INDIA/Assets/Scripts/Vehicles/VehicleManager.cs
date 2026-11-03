using System.Collections.Generic;
using UnityEngine;
using CityLifeIndia.SaveSystem;

namespace CityLifeIndia.Vehicles
{
    public class VehicleManager : MonoBehaviour
    {
        public static VehicleManager Instance { get; private set; }

        [Header("Vehicle Prefabs")]
        [SerializeField] private GameObject starterCarPrefab;
        [SerializeField] private GameObject starterBikePrefab;

        [Header("Spawn Points")]
        [SerializeField] private Transform garageSpawnPoint;
        [SerializeField] private List<Transform> streetParkingPoints;

        private List<GameObject> activeVehicles = new List<GameObject>();
        private GameObject currentSpawnedGarageVehicle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SpawnInitialVehicles();
        }

        public void SpawnInitialVehicles()
        {
            // Spawn default car at garage
            if (starterCarPrefab != null && garageSpawnPoint != null)
            {
                currentSpawnedGarageVehicle = Instantiate(starterCarPrefab, garageSpawnPoint.position, garageSpawnPoint.rotation);
                activeVehicles.Add(currentSpawnedGarageVehicle);
                ApplySavedPaint(currentSpawnedGarageVehicle);
            }

            // Spawn bike at first parking spot if available
            if (starterBikePrefab != null && streetParkingPoints != null && streetParkingPoints.Count > 0)
            {
                var bike = Instantiate(starterBikePrefab, streetParkingPoints[0].position, streetParkingPoints[0].rotation);
                activeVehicles.Add(bike);
            }
        }

        public void SwitchGarageVehicle(int vehicleTypeIndex)
        {
            if (currentSpawnedGarageVehicle != null)
            {
                activeVehicles.Remove(currentSpawnedGarageVehicle);
                Destroy(currentSpawnedGarageVehicle);
            }

            GameObject prefabToSpawn = (vehicleTypeIndex == 0) ? starterCarPrefab : starterBikePrefab;
            if (prefabToSpawn != null && garageSpawnPoint != null)
            {
                currentSpawnedGarageVehicle = Instantiate(prefabToSpawn, garageSpawnPoint.position, garageSpawnPoint.rotation);
                activeVehicles.Add(currentSpawnedGarageVehicle);
                ApplySavedPaint(currentSpawnedGarageVehicle);
            }
        }

        public void SetCurrentVehicleColor(Color color)
        {
            if (currentSpawnedGarageVehicle != null)
            {
                var vehicle = currentSpawnedGarageVehicle.GetComponent<VehicleController>();
                if (vehicle != null)
                {
                    vehicle.SetPaintColor(color);
                }
            }

            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData != null)
            {
                SaveManager.Instance.CurrentData.carColorHex = ColorUtility.ToHtmlStringRGBA(color);
                SaveManager.Instance.SaveGame();
            }
        }

        private void ApplySavedPaint(GameObject vehicleObj)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData != null)
            {
                if (ColorUtility.TryParseHtmlString("#" + SaveManager.Instance.CurrentData.carColorHex, out Color paintCol))
                {
                    var vehicle = vehicleObj.GetComponent<VehicleController>();
                    if (vehicle != null) vehicle.SetPaintColor(paintCol);
                }
            }
        }
    }
}
