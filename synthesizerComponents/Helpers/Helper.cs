class Helper
{
    public static List<String> extractVariableNames(TokenTreeNode treeHead)
    {
        List<string> result = new();
        return extractVariableNames(treeHead, result);
    }

    private static List<string> extractVariableNames(TokenTreeNode head, List<string> names)
    {

        bool a = head.value.Type == TokenType.Identifier ? names.AddIfNotIncluded(head.value.Value) : false;

        foreach (var child in head.Children)
        {
            names = extractVariableNames(child, names);
        }
        return names;
    }
}