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
	public class PotionAction : ScriptableObject
    {
        #region Content
        [Section("Action Potion")]

        public ParticleSystem Particles = null;
        public AudioClip AudioClip = null; 
        #endregion
    }
}
