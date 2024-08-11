using System.Globalization;

class Synthesizer
{
    TokenTreeNode tree;
    public Synthesizer(TokenTreeNode tokenTree)
    {
        this.tree = tokenTree;
    }

    public void Synthesize()
    {
        Dictionary<string, int> variableLocations = new();

        List<string> variables = Helper.extractVariableNames(tree);
        variables.ForEach(i => Console.WriteLine(i));
        variableLocations = GenerateASMLocations(variables.ToArray());

        foreach (var variable in variableLocations)
        {
            Console.WriteLine(string.Format("{0} : {1}", variable.Key, variable.Value));
        }

        PrintTree(tree);
    }

    private static Dictionary<string, int> GenerateASMLocations(string[] variables)
    {
        Dictionary<string, int> res = new();
        int lastLocation = 0;

        foreach (string variable in variables)
        {
            res[variable] = lastLocation+=8; 
        }

        return res;
    }

    public static void Synthesize(TokenTreeNode tokenTree)
    {
        // NAME | OFFSET
        Dictionary<string, int> variableLocations = new();
        Console.WriteLine("getting variables");
        List<string> variables = Helper.extractVariableNames(tokenTree);
        Console.WriteLine(variables.ToArray().ToString());
        PrintTree(tokenTree);
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
