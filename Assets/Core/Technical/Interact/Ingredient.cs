// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class Ingredient : PhysicsObject
    {
        #region Global Members
        [Section("Ingredient")]

        [SerializeField] private PotionAction action = null;
        public PotionAction Action => action;

        [SerializeField] private bool canBeMixedUp = true;

        public bool CanBeMixedUp => canBeMixedUp;
        #endregion

        #region Behaviour
        public override void Shake()
        {
            base.Shake();

            // Mix in.
            int _count = OverlapCollider(triggerMask, true);
            if (_count > 0)
            {
                float _bestDistance = 999f;
                int _bestIndex = -1;

                for (int _i = 0; _i < _count; _i++)
                {
                    Collider2D _overlap = overlapBuffer[_i];
                    float _distanceValue = Mathf.Abs(Vector2.Distance(rigidbody.position, _overlap.attachedRigidbody.position));

                    if (_overlap.isTrigger && (_distanceValue < _bestDistance))
                    {
                        _bestDistance = _distanceValue;
                        _bestIndex = _i;
                    }
                }

                // Trigger.
                if ((_bestIndex > -1) && overlapBuffer[_bestIndex].TryGetComponent(out AddInPotionTrigger _trigger))
                {
                    _trigger.OnTrigger(this);
                }
            }
        }
        #endregion
    }
}
