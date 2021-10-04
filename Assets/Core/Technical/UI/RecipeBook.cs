// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.UI; 

namespace LudumDare49
{
	public class RecipeBook : MonoBehaviour
    {
        #region Global Members
        [Section("Recipe Book")]
        [SerializeField, Required] private Animator animator = null; 
        [SerializeField] private Sprite[] pageSprites = new Sprite[] { };
        [SerializeField, ReadOnly] private int currentIndex = 0;
        [Section("Pages")]
        [SerializeField, Required] private SpriteRenderer page = null; 
        #endregion

        #region Animation
        private static readonly int openBook_Hash = Animator.StringToHash("IsBookOpen"); 
        #endregion

        #region Methods
        public void OpenBook() => animator.SetBool(openBook_Hash, true);
        public void CloseBook() => animator.SetBool(openBook_Hash, false);

        public void OnNextPage()
        {
            int _currentIndex = currentIndex++ >= pageSprites.Length ? pageSprites.Length - 1 : currentIndex;
            if (_currentIndex < currentIndex)
                currentIndex = _currentIndex;
            else
                page.sprite = pageSprites[currentIndex]; 
        }
        public void OnPreviousPage()
        {
            int _currentIndex = currentIndex-- < 0 ? 0 : currentIndex;
            if (_currentIndex > currentIndex)
                currentIndex = _currentIndex;
            else
                page.sprite = pageSprites[currentIndex];
        }
        #endregion
    }
}
