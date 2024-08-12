public static class Extensions
{
    public static bool AddIfNotIncluded<T>(this List<T> list, T objectToAdd)
    {
        if(!list.Contains(objectToAdd))
        {
            list.Add(objectToAdd);
            return true;
        }
        return false;
    }
}