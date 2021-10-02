// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class Potion : PhysicsObject
    {
        #region Global Members
        [Section("Potion")]
        [SerializeField] private Recipe potionRecipe = null;
        [SerializeField, ReadOnly] private int score = 0;
        public int Score => score;

        [Section("Physics")]

        [SerializeField] private new Rigidbody2D rigidbody = null;
        [SerializeField] private new Collider2D collider = null;

        [SerializeField] private LayerMask physicsLayer = new LayerMask();
        [SerializeField] private LayerMask snapLayer = new LayerMask();
        [SerializeField] private LayerMask potionLayer = new LayerMask();

        private ContactFilter2D contactFilter = new ContactFilter2D();
        #endregion

        #region Methods
        public void ApplyAction(ActionPotion _action) => score += potionRecipe.GetActionScore(_action);

        // -----------------------

        protected static Collider2D[] overlapBuffer = new Collider2D[6];

        private void ExtractFromCollisions()
        {
            // Physics overlap extraction.
            int _count = OverlapCollider(physicsLayer);
            for (int _i = 0; _i < _count; _i++)
            {
                // If overlap, extract from collision.
                ColliderDistance2D _distance = collider.Distance(overlapBuffer[_i]);

                if (_distance.isOverlapped)
                    rigidbody.position += _distance.normal * _distance.distance;
            }

            // Snap test.
            _count = OverlapCollider(snapLayer);
            if (_count > 0)
            {
                float _bestDistance = 999f;
                Vector3 _position = rigidbody.position;

                for (int _i = 0; _i < _count; _i++)
                {
                    // If overlap, extract from collision.
                    Collider2D _overlap = overlapBuffer[_i];
                    float _distanceValue = Mathf.Abs(Vector2.Distance(rigidbody.position, _overlap.attachedRigidbody.position));

                    if (_distanceValue < _bestDistance)
                    {
                        _bestDistance = _distanceValue;
                        _position = _overlap.attachedRigidbody.position;
                    }
                }

                rigidbody.position = _position;
            }

            // Mix in Potion
            _count = OverlapCollider(potionLayer);
            if (_count > 0)
            {
                float _bestDistance = 999f;
                int _bestIndex = 0;

                for (int _i = 0; _i < _count; _i++)
                {
                    // If overlap, extract from collision.
                    Collider2D _overlap = overlapBuffer[_i];
                    float _distanceValue = Mathf.Abs(Vector2.Distance(rigidbody.position, _overlap.attachedRigidbody.position));

                    if (_distanceValue < _bestDistance)
                    {
                        _bestDistance = _distanceValue;
                        _bestIndex = _i;
                    }
                }

                // Mix.
            }
        }

        private int OverlapCollider(LayerMask _mask)
        {
            contactFilter.layerMask = _mask;
            return collider.OverlapCollider(contactFilter, overlapBuffer);
        }
        #endregion
    }
}
