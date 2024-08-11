class Helper
{
    public static List<String> extractVariableNames(TokenTreeNode treeHead)
    {
        List<string> result = new();
        return extractVariableNames(treeHead, result);
    }

    private static List<string> extractVariableNames(TokenTreeNode head, List<string> names)
    {
        foreach (var child in head.Children)
        {
            if(child.value.Type == TokenType.Identifier)
            {
                names.AddIfNotIncluded(child.value.Value);
            }
            names = extractVariableNames(child, names);
        }
        return names;
    }
}