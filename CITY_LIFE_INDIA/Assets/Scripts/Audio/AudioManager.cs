using System.Collections.Generic;
using UnityEngine;

namespace CityLifeIndia.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambienceSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip cityAmbience;
        [SerializeField] private AudioClip missionSuccessClip;
        [SerializeField] private AudioClip missionFailClip;
        [SerializeField] private AudioClip checkpointDingClip;
        [SerializeField] private AudioClip buttonClickClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlayAmbience();
        }

        public void PlayAmbience()
        {
            if (ambienceSource != null && cityAmbience != null)
            {
                ambienceSource.clip = cityAmbience;
                ambienceSource.loop = true;
                ambienceSource.volume = 0.5f;
                ambienceSource.Play();
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1.0f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickClip, 0.7f);
        }

        public void PlayMissionSuccess()
        {
            PlaySFX(missionSuccessClip, 1.0f);
        }

        public void PlayMissionFail()
        {
            PlaySFX(missionFailClip, 1.0f);
        }

        public void PlayCheckpointDing()
        {
            PlaySFX(checkpointDingClip, 0.8f);
        }

        public void SetMasterVolume(float vol)
        {
            AudioListener.volume = Mathf.Clamp01(vol);
        }
    }
}
