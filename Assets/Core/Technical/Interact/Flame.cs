// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    public class Flame : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private ParticleSystem loveVFX = null; 

        private static readonly int isHorny_ToHash = Animator.StringToHash("IsHorny");

        public void MakeHorny()
        {
            animator.SetBool(isHorny_ToHash, true);
            if (loveVFX)
                loveVFX.Play(); 
        }
    }
}
