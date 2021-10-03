// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace LudumDare49
{
	public class SoundManager : MonoBehaviour
    {
        #region Global Members
        [Section("SoundManager")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private List<AudioSource> audioSourcePool = new List<AudioSource>();
        [SerializeField, Range(.1f, 1.0f)] private float transitionDuration = .25f; 
        public static SoundManager Instance;

        private static readonly string mainSnapshot_Name = "Snapshot - MAIN"; 
        private static readonly string LPESnapshot_Name = "Snapshot - Low Pass Echo"; 
        private static readonly string PSSnapshot_Name = "Snapshot - Pitch Shifter"; 

        public enum AudioEffectState
        {
            None, 
            LowPassAndEcho, 
            PitchShifter
        }
        #endregion

        #region Methods
        public void PlayAtPosition(AudioClip _clip, Vector2 _position, float _volume = 1.0f)
        {
            AudioSource _source;
            for (int i = 0; i < audioSourcePool.Count; i++)
            {
                if (audioSourcePool[i].isPlaying) continue;
                _source = audioSourcePool[i];
                _source.transform.position = _position;
                _source.PlayOneShot(_clip, _volume);
                return; 
            }
            _source = Instantiate(audioSourcePool[0], transform);           
            _source.playOnAwake = false;
            _source.outputAudioMixerGroup = mixer.outputAudioMixerGroup;
            _source.clip = _clip;
            _source.loop = false; 
        }

        public void ApplyAudioEffect(AudioEffectState _effectState)
        {
            switch (_effectState)
            {
                case AudioEffectState.None:
                    mixer.FindSnapshot(mainSnapshot_Name).TransitionTo(transitionDuration);
                    break;
                case AudioEffectState.LowPassAndEcho:
                    mixer.FindSnapshot(LPESnapshot_Name).TransitionTo(transitionDuration);
                    break;
                case AudioEffectState.PitchShifter:
                    mixer.FindSnapshot(PSSnapshot_Name).TransitionTo(transitionDuration);
                    break;
                default:
                    mixer.FindSnapshot(mainSnapshot_Name).TransitionTo(transitionDuration);
                    break;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this; 
        }
        #endregion
    }
}
