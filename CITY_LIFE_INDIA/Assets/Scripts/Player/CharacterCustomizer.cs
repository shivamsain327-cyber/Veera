using UnityEngine;
using CityLifeIndia.SaveSystem;

namespace CityLifeIndia.Player
{
    [System.Serializable]
    public class CharacterAppearanceData
    {
        public int skinToneIndex;
        public int hairStyleIndex;
        public int outfitIndex;
        public Color shirtColor = Color.red;
        public Color pantsColor = Color.blue;
    }

    public class CharacterCustomizer : MonoBehaviour
    {
        [Header("Renderers to customize")]
        [SerializeField] private SkinnedMeshRenderer bodyRenderer;
        [SerializeField] private MeshRenderer shirtRenderer;
        [SerializeField] private MeshRenderer pantsRenderer;

        [Header("Palettes")]
        [SerializeField] private Color[] skinTonePalette = new Color[]
        {
            new Color(0.95f, 0.76f, 0.65f),
            new Color(0.85f, 0.62f, 0.48f),
            new Color(0.68f, 0.46f, 0.32f),
            new Color(0.50f, 0.32f, 0.22f)
        };

        [SerializeField] private Color[] traditionalColors = new Color[]
        {
            new Color(0.85f, 0.2f, 0.2f), // Royal Jaipur Red
            new Color(0.95f, 0.55f, 0.1f), // Marigold Saffron
            new Color(0.1f, 0.6f, 0.7f),  // Jodhpur Blue
            new Color(0.2f, 0.7f, 0.3f),  // Shekhawati Green
            new Color(0.9f, 0.9f, 0.9f)   // Khadi White
        };

        private CharacterAppearanceData currentData = new CharacterAppearanceData();

        private void Start()
        {
            LoadSavedAppearance();
        }

        public void SetSkinTone(int index)
        {
            if (index >= 0 && index < skinTonePalette.Length)
            {
                currentData.skinToneIndex = index;
                if (bodyRenderer != null)
                {
                    bodyRenderer.material.color = skinTonePalette[index];
                }
            }
        }

        public void SetShirtColor(Color color)
        {
            currentData.shirtColor = color;
            if (shirtRenderer != null)
            {
                shirtRenderer.material.color = color;
            }
        }

        public void SetPantsColor(Color color)
        {
            currentData.pantsColor = color;
            if (pantsRenderer != null)
            {
                pantsRenderer.material.color = color;
            }
        }

        public void SaveAppearance()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData != null)
            {
                SaveManager.Instance.CurrentData.skinIndex = currentData.skinToneIndex;
                SaveManager.Instance.CurrentData.shirtColorHex = ColorUtility.ToHtmlStringRGBA(currentData.shirtColor);
                SaveManager.Instance.CurrentData.pantsColorHex = ColorUtility.ToHtmlStringRGBA(currentData.pantsColor);
                SaveManager.Instance.SaveGame();
            }
        }

        public void LoadSavedAppearance()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData != null)
            {
                var data = SaveManager.Instance.CurrentData;
                SetSkinTone(data.skinIndex);

                if (ColorUtility.TryParseHtmlString("#" + data.shirtColorHex, out Color shirtCol))
                {
                    SetShirtColor(shirtCol);
                }

                if (ColorUtility.TryParseHtmlString("#" + data.pantsColorHex, out Color pantsCol))
                {
                    SetPantsColor(pantsCol);
                }
            }
        }
    }
}
