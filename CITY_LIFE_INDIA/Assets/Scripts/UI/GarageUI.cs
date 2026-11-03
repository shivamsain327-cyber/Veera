using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CityLifeIndia.Vehicles;

namespace CityLifeIndia.UI
{
    public class GarageUI : MonoBehaviour
    {
        [Header("Vehicle Selection")]
        [SerializeField] private TextMeshProUGUI vehicleNameText;
        [SerializeField] private Slider speedStatSlider;
        [SerializeField] private Slider handlingStatSlider;

        [Header("Color Swatches")]
        [SerializeField] private Button[] colorButtons;
        [SerializeField] private Color[] vehicleColorPalette = new Color[]
        {
            new Color(0.85f, 0.15f, 0.15f), // Jaipur Royal Crimson
            new Color(0.12f, 0.45f, 0.85f), // Jodhpur Indigo
            new Color(0.95f, 0.60f, 0.10f), // Marigold Gold
            new Color(0.12f, 0.12f, 0.12f), // Midnight Phantom
            new Color(0.92f, 0.92f, 0.95f), // Pearl White
            new Color(0.15f, 0.70f, 0.40f)  // Emerald Green
        };

        private int selectedVehicleIndex = 0; // 0 = Car, 1 = Bike

        private void Start()
        {
            UpdateVehicleDisplay();
        }

        public void SelectNextVehicle()
        {
            selectedVehicleIndex = (selectedVehicleIndex + 1) % 2;
            UpdateVehicleDisplay();
            VehicleManager.Instance?.SwitchGarageVehicle(selectedVehicleIndex);
        }

        public void SelectPreviousVehicle()
        {
            selectedVehicleIndex = (selectedVehicleIndex - 1 + 2) % 2;
            UpdateVehicleDisplay();
            VehicleManager.Instance?.SwitchGarageVehicle(selectedVehicleIndex);
        }

        public void ApplyPaintColor(int colorIndex)
        {
            if (colorIndex >= 0 && colorIndex < vehicleColorPalette.Length)
            {
                Color chosen = vehicleColorPalette[colorIndex];
                VehicleManager.Instance?.SetCurrentVehicleColor(chosen);
                Debug.Log($"[GarageUI] Applied color #{ColorUtility.ToHtmlStringRGB(chosen)} to vehicle.");
            }
        }

        private void UpdateVehicleDisplay()
        {
            if (selectedVehicleIndex == 0)
            {
                if (vehicleNameText != null) vehicleNameText.text = "Desi Cruiser V8 (Car)";
                if (speedStatSlider != null) speedStatSlider.value = 0.85f;
                if (handlingStatSlider != null) handlingStatSlider.value = 0.70f;
            }
            else
            {
                if (vehicleNameText != null) vehicleNameText.text = "Rajputana Bullet 350 (Bike)";
                if (speedStatSlider != null) speedStatSlider.value = 0.70f;
                if (handlingStatSlider != null) handlingStatSlider.value = 0.90f;
            }
        }

        public void CloseGarage()
        {
            UIManager.Instance?.HideGarage();
        }
    }
}
