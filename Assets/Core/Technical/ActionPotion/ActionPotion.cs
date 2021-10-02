// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    [CreateAssetMenu(fileName = "DAT_ActionPotion", menuName = "Datas/ActionPotion", order = 150)]
	public class ActionPotion : ScriptableObject
    {
        #region Content
        [Section("ActionPotion")]
        [SerializeField] private ParticleSystem actionParticles = null;
        [SerializeField] private AudioClip      audioClip       = null; 
        #endregion
    }
}
