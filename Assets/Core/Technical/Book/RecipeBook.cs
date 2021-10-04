// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace LudumDare49
{
	public class RecipeBook : MonoBehaviour
    {
        public static event Action OnCloseBook = null;

        #region Global Members
        [Section("Recipe Book")]
        [SerializeField, Required] private Animator animator = null; 
        [SerializeField] private Sprite[] pageSprites = new Sprite[] { };
        [SerializeField, ReadOnly] private int currentIndex = -1;
        [Section("Pages")]
        [SerializeField, Required] private SpriteRenderer page = null;
        [Section("Interactions")]
        [SerializeField] private BookInteractable nextPageInteract = null; 
        [SerializeField] private BookInteractable previousPageInteract = null; 
        [SerializeField] private BookInteractable closeBookInteract = null;
        [SerializeField] private BookInteractable openBookInteract = null;
        [Section("Feedbacks")]
        [SerializeField] private AudioClip nextPageClip = null;
        [SerializeField] private AudioClip previousPageClip = null;
        [SerializeField] private AudioClip openBookClip = null;
        [SerializeField] private AudioClip closeBookClip = null; 
        private bool isInitialised = false; 
        #endregion

        #region Animation
        private static readonly int openBook_Hash = Animator.StringToHash("IsBookOpen");
        #endregion

        #region Methods
        public void OpenBook()
        {
            animator.SetBool(openBook_Hash, true);
            SoundManager.Instance.PlayAtPosition(openBookClip, transform.position);
            previousPageInteract.gameObject.SetActive(currentIndex != 0);
            nextPageInteract.gameObject.SetActive(currentIndex != pageSprites.Length - 1);
        }
        public void CloseBook()
        {
            animator.SetBool(openBook_Hash, false);
            SoundManager.Instance.PlayAtPosition(closeBookClip, transform.position);

            OnCloseBook?.Invoke();
        }

        public void OnPreviousPage()
        {
            currentIndex--;
            if (currentIndex == 0)
                previousPageInteract.gameObject.SetActive(false); 
            else
                previousPageInteract.gameObject.SetActive(true);

            nextPageInteract.gameObject.SetActive(true);

            SoundManager.Instance.PlayAtPosition(previousPageClip, transform.position);
            page.sprite = pageSprites[currentIndex]; 
        }
        public void OnNextPage()
        {
            currentIndex++;
            if (currentIndex == pageSprites.Length - 1)
                nextPageInteract.gameObject.SetActive(false);
            else
                nextPageInteract.gameObject.SetActive(true);

            previousPageInteract.gameObject.SetActive(true);
            
            SoundManager.Instance.PlayAtPosition(nextPageClip, transform.position);
            page.sprite = pageSprites[currentIndex];
        }
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            isInitialised = false;
            nextPageInteract.OnInteract += OnNextPage;
            previousPageInteract.OnInteract += OnPreviousPage;
            closeBookInteract.OnInteract += CloseBook;
            openBookInteract.OnInteract += OpenBook;
        }

        private void Update()
        {
            if(!isInitialised)
            {
                if (Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
                {
                    OpenBook();
                    isInitialised = true; 
                }
            }
        }
        #endregion
    }
}
