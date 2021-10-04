// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using DG.Tweening;
using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare49
{
    public class LivingReceptable : MonoBehaviour
    {
        #region Global Members
        [Section("LivingReceptable")]

        [SerializeField, Required] private GameObject[] livingPrefabs = new GameObject[] { };
        [SerializeField, Required] private GameObject generateFX = null;
        [SerializeField, Required] private AudioClip generateClip = null;

        [Space(5f)]

        [SerializeField] private Transform[] anchors = new Transform[] { };
        private List<Transform> availableAnchors = new List<Transform>();
        #endregion

        #region Methods
        public void Generate(float _duration)
        {
            if (availableAnchors.Count == 0)
                return;

            int _index = Random.Range(0, availableAnchors.Count);
            Transform _transform = availableAnchors[_index];

            GameObject _instance = Instantiate(livingPrefabs[Random.Range(0, livingPrefabs.Length)]);
            SpriteRenderer _sprite = _instance.GetComponentInChildren<SpriteRenderer>();

            _instance.transform.position = _transform.position;
            _instance.transform.rotation = Quaternion.identity;
            _sprite.color = new Color(1f, 1f, 1f, 0f);

            Sequence _s = DOTween.Sequence();
            _s.Append(_sprite.DOFade(1f, 1f));
            _s.AppendInterval(_duration);
            _s.Append(_sprite.DOFade(0f, 1f));
            _s.AppendCallback(() =>
            {
                Destroy(_instance);
                availableAnchors.Add(_transform);
            });

            availableAnchors.RemoveAt(_index);

            if (generateFX != null)
            {
                GameObject _fx = Instantiate(generateFX);
                _fx.transform.position = _transform.position;
                _fx.transform.rotation = Quaternion.identity;

                SoundManager.Instance.PlayAtPosition(generateClip, _transform.position);
            }
        }

        private void Start()
        {
            availableAnchors = new List<Transform>(anchors);
        }
        #endregion
    }
}
