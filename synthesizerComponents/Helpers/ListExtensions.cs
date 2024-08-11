public static class Extensions
{
    public static void AddIfNotIncluded<T>(this List<T> list, T objectToAdd)
    {
        if(!list.Contains(objectToAdd))
        {
            list.Add(objectToAdd);
        }
    }
}