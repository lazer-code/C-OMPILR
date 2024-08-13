/// <summary>
/// for the actual assembly generation part of this class, check out AssemblyGenerator.cs
/// </summary>
partial class Synthesizer
{
    TokenTreeNode tree;
    Dictionary<string, int> varOffsets;
    public Synthesizer(TokenTreeNode tokenTree)
    {
        this.tree = tokenTree;
        varOffsets = new();
    }

    private Dictionary<string, int> GenerateASMLocations(string[] variables)
    {
        // turn all of the identifiers to offsets
        Dictionary<string, int> res = new();
        int lastLocation = 0;

        foreach (string variable in variables)
        {
            res[variable] = lastLocation+=8; 
        }

        this.varOffsets = res;

        return this.varOffsets;
    }

    public void Synthesize()
    {
        Dictionary<string, int> variableLocations = new();

        // extract variables and assign them locations
        List<string> variables = Helper.extractVariableNames(this.tree);
        variables.ForEach(i => Console.WriteLine(i));
        variableLocations = GenerateASMLocations(variables.ToArray());
     
        // generate the assembly code and write it to a file and the screen
        string res = generateOutputFile("test.asm");
        Console.WriteLine(res);

        // print the variables and their offset to have as a reference
        Console.WriteLine("");
        foreach (var variable in variableLocations)
        {
            Console.WriteLine(string.Format("{0} : {1}", variable.Key, variable.Value));
        }

        PrintTree(this.tree);
    }

    /// <summary>
    /// recursively prints the tree and all of its values
    /// </summary>
    /// <param name="node">tree root</param>
    /// <param name="level">the tab amount</param>
    public void PrintTree(TokenTreeNode node, int level = 0)
    {
        Console.WriteLine("" + new string('\t', level) + node.value.Type + ":" + node.value.Value);
        foreach (var child in node.Children)
        {
            PrintTree(child, level + 1);
        }
    }
}
