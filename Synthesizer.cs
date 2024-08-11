using System.Globalization;

class Synthesizer
{
    TokenTreeNode tree;
    public Synthesizer(TokenTreeNode tokenTree)
    {
        this.tree = tokenTree;
    }

    public void synthesize()
    {
        PrintTree(tree);
    }

    public static void PrintTree(TokenTreeNode node, int level = 0)
    {
        Console.WriteLine("" + new string('\t', level) + node.value.Type + ":" + node.value.Value);
        foreach (var child in node.Children)
        {
            PrintTree(child, level + 1);
        }
    }
}
