using System.Collections.Generic;

namespace ArkEcho
{
    public static class EM_List
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// Returns the last item in the List (Count -1), default if List is null or empty
        /// </summary>
        public static T Last<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
                return default(T);
            return list[list.Count - 1];
        }
    }
}
