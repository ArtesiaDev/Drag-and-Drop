using System;
using _Project.Scripts.Core.Models;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.AudioManagement
{
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundSource;
        [SerializeField] private AudioSource _necessarySoundSource;
        [SerializeField] private AudioSource _musicSource;

        private AudioModel _audioModel;

        [Inject]
        private void Construct(AudioModel audioModel) =>
            _audioModel = audioModel;

        private void Awake() =>
            DontDestroyOnLoad(this);

        public void SetSoundsVolume(float percentage)
        {
            var volume = Mathf.Lerp(-20f, 20f, percentage);
            if (percentage == 0)
                volume = -80;
            _audioModel.AudioMixer.SetFloat(MixerParameters.SoundsVolume.ToString(), volume);
        }

        public void SetMusicVolume(float percentage)
        {
            var volume = Mathf.Lerp(-20f, 20f, percentage);
            if (percentage == 0)
                volume = -80;
            _audioModel.AudioMixer.SetFloat(MixerParameters.MusicVolume.ToString(), volume);
        }

        public void PlayOneShotSound(AudioClip clipName, float volume = 1f, float pitch = 1f,
            bool playAfterComplete = false, bool playNecessary = false)
        {
            var source = playNecessary 
                ? _necessarySoundSource
                : _soundSource;
            
            if (playAfterComplete && source.isPlaying)
               return;
            
            var clip = _audioModel.AudioClips[clipName];
            source.pitch = pitch;
            source.loop = false;
            source.PlayOneShot(clip, volume);
        }

        public void PlayBackgroundMusic(AudioClip clipName, float volume = 1f, float pitch = 1f,
            float graduallyPlayDuration = default, Action onComplete = null)
        {
            var clip = _audioModel.AudioClips[clipName];
            _musicSource.volume = 0;
            _musicSource.pitch = pitch;
            _musicSource.loop = true;
            _musicSource.clip = clip;
            _musicSource.Play();
            _musicSource.DOFade(volume, graduallyPlayDuration).SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void StopBackgroundMusic(float graduallyStopDuration = default, Action onComplete = default) =>
            _musicSource.DOFade(0f, graduallyStopDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                _musicSource.Stop();
                _musicSource.clip = null;
                onComplete?.Invoke();
            });

        public void PlayClipAtPoint(AudioClip clipName, Vector3 position, float volume = 1f)
        {
            var clip = _audioModel.AudioClips[clipName];
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}