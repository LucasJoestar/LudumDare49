// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;

namespace EnhancedEditor
{
    /// <summary>
    /// Ends a foldout group began with <see cref="BeginFoldoutAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class EndFoldoutAttribute : EnhancedPropertyAttribute
    {
        #region Global Members
        /// <summary>
        /// Unique GUID of this attribute.
        /// </summary>
        internal Guid guid = default;

        // -----------------------

        /// <inheritdoc cref="EndFoldoutAttribute"/>
        public EndFoldoutAttribute()
        {
            guid = Guid.NewGuid();
        }
        #endregion
    }
}
