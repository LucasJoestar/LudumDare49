// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using DG.Tweening; 

namespace LudumDare49
{
	public class SoundManager : MonoBehaviour
    {
        #region Global Members
        [Section("SoundManager")]
        private AudioMixerGroup currentGroup; 
        [SerializeField] private AudioMixerGroup mainMixer;
        [SerializeField] private AudioMixerGroup lowPassEchoMixer;
        [SerializeField] private AudioMixerGroup PitchShifterMixer;
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
            if (_clip == null) return; 
            AudioSource _source;
            Sequence _s;
            for (int i = 0; i < audioSourcePool.Count; i++)
            {
                if (audioSourcePool[i].enabled) continue;
                audioSourcePool[i].enabled = true; 
                _source = audioSourcePool[i];
                _source.transform.position = _position;
                _s = DOTween.Sequence();
                _s.AppendInterval(_clip.length).OnComplete(() => _source.enabled = false); 
                _source.PlayOneShot(_clip, _volume);
                return; 
            }
            _source = Instantiate(audioSourcePool[0], transform);
            audioSourcePool.Add(_source);
            _source.enabled = true;
            _source.playOnAwake = false;
            _source.outputAudioMixerGroup = currentGroup;
            _source.clip = _clip;
            _source.loop = false;
            _s = DOTween.Sequence();
            _s.AppendInterval(_clip.length).OnComplete(() => _source.enabled = false);
            _source.PlayOneShot(_clip, _volume);
        }

        public void ApplyAudioEffect(AudioEffectState _effectState)
        {
            switch (_effectState)
            {
                case AudioEffectState.None:
                    currentGroup = mainMixer; 
                    //mainMixer.FindSnapshot(mainSnapshot_Name).TransitionTo(transitionDuration);
                    break;
                case AudioEffectState.LowPassAndEcho:
                    //mainMixer.FindSnapshot(LPESnapshot_Name).TransitionTo(transitionDuration);
                    currentGroup = lowPassEchoMixer; 
                    break;
                case AudioEffectState.PitchShifter:
                    //mainMixer.FindSnapshot(PSSnapshot_Name).TransitionTo(transitionDuration);
                    currentGroup = PitchShifterMixer; 
                    break;
                default:
                    // mainMixer.FindSnapshot(mainSnapshot_Name).TransitionTo(transitionDuration);
                    currentGroup = mainMixer; 
                    break;
            }
            for (int i = 0; i < audioSourcePool.Count; i++)
            {
                audioSourcePool[i].outputAudioMixerGroup = currentGroup; 
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

            currentGroup = mainMixer;
        }
        #endregion
    }
}
