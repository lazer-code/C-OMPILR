namespace cOMPILR
{
    public static class Extensions
    {
        /// <summary>
        /// add item to a list if it's not in it already
        /// </summary>
        /// <typeparam name="T">the type of the object and list</typeparam>
        /// <param name="list">this list of objects</param>
        /// <param name="objectToAdd">the object you want to add to the list</param>
        /// <returns>success state</returns>
        public static bool AddIfNotIncluded<T>(this List<T> list, T objectToAdd)
        {
            // only add the item if it is not in the list, return whether or not it was added
            if(!list.Contains(objectToAdd))
            {
                list.Add(objectToAdd);
                return true;
            }
            
            return false;
        }
    }
}
