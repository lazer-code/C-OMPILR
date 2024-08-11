using System.Globalization;

class Synthesizer
{
    TokenTree tree;
    public Synthesizer(TokenTree tokenTree)
    {
        this.tree = tokenTree;
    }

    public void synthesize()
    {
        int indentLevel = 0;
        PrintTree(tree, indentLevel);
    }

    public static void PrintTree(TokenTree node, int level = 0)
    {
        Console.WriteLine(new string(' ', level * 2) + $"{node.value.Type}: {node.value.Value}");
        foreach (var child in node.Children)
        {
            PrintTree(child, level + 1);
        }
    }
}
