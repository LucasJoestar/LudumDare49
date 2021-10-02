// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System.Collections.Generic;

namespace EnhancedEditor
{
    /// <summary>
    /// Contains multiple <see cref="List{T}"/>-related extension methods.
    /// </summary>
	public static class ListExtensions
    {
        #region Content
        /// <summary>
        /// Changes the number of elements in this list to a specific size.
        /// </summary>
        /// <typeparam name="T">List content type.</typeparam>
        /// <param name="_list">List to resize.</param>
        /// <param name="_size">New list size.</param>
        public static void Resize<T>(this List<T> _list, int _size)
        {
            int _count = _list.Count;
            if (_count > _size)
            {
                _list.RemoveRange(_size, _count - _size);
            }
            else if (_size > _count)
            {
                if (_size > _list.Capacity)
                    _list.Capacity = _size;

                for (int _i = _count; _i < _size; _i++)
                {
                    _list.Add(default);
                }
            }
        }
        #endregion
    }
}
