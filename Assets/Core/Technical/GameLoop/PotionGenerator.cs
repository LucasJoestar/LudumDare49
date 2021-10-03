// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using DG.Tweening;
using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class PotionGenerator : MonoBehaviour
    {
        #region Global Members
        [Section("Potion Generator")]

        [SerializeField, Required] private ConveyorBelt belt = null;
        [SerializeField, Required] private Transform generateTransform = null;
        [SerializeField, Required] private SpriteRenderer alertObject = null;

        [Section("Audio")]

        [SerializeField, Required] private AudioClip alertClip = null;

        [Section("Settings")]

        [SerializeField, MinMax(0f, 100f)] private Vector2 generatePotionInterval = new Vector2(25f, 30f);
        [SerializeField, MinMax(1, 20)] private int beltLoop = 10;
        [SerializeField] private Potion[] potions = new Potion[] { };

        [Space(5f)]

        [SerializeField] private Gradient alertGradient = new Gradient();
        [SerializeField] private Ease alertEase = Ease.Linear;

        [Section("State")]

        [SerializeField] private float timer = 0f;
        #endregion

        #region Behaviour
        private Potion potion = null;

        // -----------------------

        private void OnEndRoll()
        {
            AudioSource.PlayClipAtPoint(alertClip, alertObject.transform.position);
            alertObject.DOGradientColor(alertGradient, alertClip.length).SetEase(alertEase);
            
            potion.Activate();
        }

        // -----------------------

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                // Generate.
                potion = Instantiate(potions[Random.Range(0, potions.Length)]);
                potion.transform.position = generateTransform.position;
                potion.transform.rotation = Quaternion.identity;

                belt.Roll(potion.transform, beltLoop);

                // Set timer.
                timer = Random.Range(generatePotionInterval.x, generatePotionInterval.y);
            }
        }

        private void Start()
        {
            belt.OnEndRoll += OnEndRoll;
        }
        #endregion
    }
}
