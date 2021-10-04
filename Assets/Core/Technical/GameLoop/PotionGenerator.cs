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
        [SerializeField, Required] private GameObject blockInteract = null;

        [Section("Audio")]

        [SerializeField, Required] private AudioClip alertClip = null;

        [Section("Settings")]

        [SerializeField, MinMax(0f, 100f)] private Vector2 generatePotionInterval = new Vector2(25f, 30f);
        [SerializeField, MinMax(1, 20)] private int beltLoop = 10;
        [SerializeField] private Potion[] potions = new Potion[] { };

        [Space(5f)]

        [SerializeField] private Color alertColor = Color.red;
        [SerializeField, Range(0, 10)] private int alertLoop = 2;
        [SerializeField, Range(0f, 5f)] private float alertDuration = .5f;
        [SerializeField] private Ease alertEase = Ease.Linear;

        [Section("State")]

        [SerializeField] private float timer = 0f;
        #endregion

        #region Behaviour
        private Potion potion = null;
        private bool canGenerate = true;
        private bool isEnabled = false;

        // -----------------------

        private void OnEndRoll()
        {
            SoundManager.Instance.PlayAtPosition(alertClip, alertObject.transform.position);
            alertObject.DOColor(alertColor, alertDuration).SetEase(alertEase).SetLoops(alertLoop, LoopType.Yoyo);

            potion.Rigidbody.isKinematic = false;
            blockInteract.SetActive(false);

            canGenerate = true;
        }

        private void Generate()
        {
            canGenerate = false;

            // Generate.
            potion = Instantiate(potions[Random.Range(0, potions.Length)]);
            potion.transform.position = generateTransform.position;
            potion.transform.rotation = Quaternion.identity;

            potion.Rigidbody.isKinematic = true;

            blockInteract.SetActive(true);
            belt.Roll(potion.transform, beltLoop);

            // Set timer.
            timer = Random.Range(generatePotionInterval.x, generatePotionInterval.y);
        }

        // -----------------------

        private void Update()
        {
            if (!isEnabled)
                return;

            timer -= Time.deltaTime;
            if ((timer < 0f) && canGenerate)
            {
                Generate();
            }
        }

        private void OnApplicationQuit()
        {
            canGenerate = false;
        }

        private void Start()
        {
            belt.OnEndRoll += OnEndRoll;
            RecipeBook.OnCloseBook += () => isEnabled = true;

            Potion.OnNoPotion += () =>
            {
                if (canGenerate)
                    Generate();
            };
        }
        #endregion
    }
}
