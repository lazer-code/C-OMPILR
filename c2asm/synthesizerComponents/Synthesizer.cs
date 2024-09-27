using System.Globalization;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Principal;

class Synthesizer
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

        List<string> variables = Helper.extractVariableNames(this.tree);
        variables.ForEach(i => Console.WriteLine(i));
        variableLocations = GenerateASMLocations(variables.ToArray());
        generateOutputFile("test.asm");

        Console.WriteLine("");
        foreach (var variable in variableLocations)
        {
            Console.WriteLine(string.Format("{0} : {1}", variable.Key, variable.Value));
        }

        PrintTree(this.tree);
    }

    private string GenerateOpcodes(Token tk)
    {
        return tk.Type switch
        {
            TokenType.Assignment => "mov",
            TokenType.Identifier => "[ds+" + this.varOffsets[tk.Value] + "]",
            TokenType.Number => "" + tk.Value,
            TokenType.Operator => tk.Value switch 
            {
                "+" => "add",
                "*" => "mul",
                _ => throw new Exception("unknown operator " + tk.Value + "did you forget to implement it?"),
            },
            TokenType.GreaterThan => "jg",
            TokenType.Keyword => tk.Value switch
            {
                "if" => "cmp",
                "return" => "mov eax,",
                _ => throw new Exception("unknown keyword" + tk.Value)
            },
            TokenType.COF => "setJumpAddress",
            TokenType.Body => "setJumpAddress",
            _ => "invalid line ( for now )",

        };
    }

    private void generateOutputFile(string outputFileName)
    {
        generateOutputFile(outputFileName, this.tree);
    }

    private void generateOutputFile(string outputFileName, TokenTreeNode tk)
    {
        if(tk.value.Type != TokenType.GreaterThan)
            Console.Write("" + GenerateOpcodes(tk.value));
            // TODO: find a way to do this that isn't hard coding an exception for this case 

        foreach (var child in tk.Children)
        {
            Console.Write(" ");
            this.generateOutputFile(outputFileName, child);
        }
        if(tk.value.Type == TokenType.GreaterThan)
            Console.Write(" " + GenerateOpcodes(tk.value));
            // TODO: find a way to do this that isn't hard coding an exception for this case 

    }

    public void PrintTree(TokenTreeNode node, int level = 0)
    {
        Console.WriteLine("" + new string('\t', level) + node.value.Type + ":" + node.value.Value);
        foreach (var child in node.Children)
        {
            PrintTree(child, level + 1);
        }
    }
}
