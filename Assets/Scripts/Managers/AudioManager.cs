using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public class Sound
        {
            [SerializeField] bool loop;
            public bool Loop { get => loop; set => loop = value; }

            [SerializeField] string name;
            public string Name { get => name; set => name = value; }

            [SerializeField] AudioClip clip;
            public AudioClip Clip { get => clip; set => clip = value; }
        }

        #region Inspector

        [SerializeField] Sound[] musicSounds;
        public Sound[] MusicSounds { get => musicSounds; set => musicSounds = value; }

        [SerializeField] Sound[] sfxSounds;
        public Sound[] SfxSounds { get => sfxSounds; set => sfxSounds = value; }

        [SerializeField] AudioSource musicSource;
        public AudioSource MusicSource { get => musicSource; set => musicSource = value; }

        [SerializeField] AudioSource sfxSource;
        public AudioSource SfxSource { get => sfxSource; set => sfxSource = value; }

        #endregion

        #region Properties



        #endregion

        #region Fields

        private float m_InitVolume = 0.5f;

        private float m_MusicVolume;

        private float m_SfxVolume;

        private bool m_PlayingMusic = false;

        private bool m_PlayingSfx = false;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            // 音量の初期化
            SetMusicVolume(m_InitVolume);
            SetSfxVolume(m_InitVolume);

            // BGMを流す
            PlayMusic("BGM");
        }

        #endregion

        #region Methods

        public void SetMusicVolume(float volume)
        {
            m_MusicVolume = volume;
            musicSource.volume = m_MusicVolume;
        }

        public void SetSfxVolume(float volume)
        {
            m_SfxVolume = volume;
            sfxSource.volume = m_SfxVolume;
        }

        public void PlayMusic(string name)
        {
            // BGMが設定されていなければ
            if (musicSounds == null) return;

            Sound s = Array.Find(musicSounds, sound => sound.Name == name);

            if (s == null)
            {
                Debug.Log($"{name} not found");
            }
            else
            {
                // AudioSourceが設定されていなければ
                if (musicSource == null) return;

                musicSource.loop = s.Loop;
                musicSource.clip = s.Clip;
                musicSource.Play();

                m_PlayingMusic = true;
            }
        }

        public void PlaySFX(string name)
        {
            // 効果音が設定されていなければ
            if (sfxSounds == null) return;

            Sound s = Array.Find(sfxSounds, sound => sound.Name == name);

            if (s == null)
            {
                Debug.Log($"{name} not found");
            }
            else
            {
                // AudioSourceが設定されていなければ
                if (sfxSource == null) return;

                sfxSource.loop = s.Loop;
                sfxSource.clip = s.Clip;
                sfxSource.Play();

                m_PlayingSfx = true;
            }
        }

        public void StopMusic(string name)
        {
            // BGMが設定されていなければ
            if (musicSounds == null) return;

            Sound s = Array.Find(musicSounds, sound => sound.Name == name);

            if (s == null)
            {
                Debug.Log($"{name} not found");
            }
            else
            {
                // AudioSourceが設定されていなければ
                if (musicSource == null) return;

                musicSource.Stop();

                m_PlayingMusic = false;
            }
        }

        public void StopSFX(string name)
        {
            // 効果音が設定されていなければ
            if (sfxSounds == null) return;

            Sound s = Array.Find(sfxSounds, sound => sound.Name == name);

            if (s == null)
            {
                Debug.Log($"{name} not found");
            }
            else
            {
                // AudioSourceが設定されていなければ
                if (sfxSource == null) return;

                sfxSource.Stop();

                m_PlayingSfx = false;
            }
        }

        public void ToggleMusic(string name)
        {
            //　BGMが流れていれば
            if (m_PlayingMusic)
                StopMusic(name);
            else
                PlayMusic(name);
        }

        public void ToggleSFX(string name)
        {
            //　Sfxが流れていれば
            if (m_PlayingSfx)
                StopSFX(name);
            else
                PlaySFX(name);
        }

        #endregion
    }
}
