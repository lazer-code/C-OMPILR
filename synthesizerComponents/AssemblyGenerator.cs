using cOMPILR.POC.TextFormatters;

partial class Synthesizer
{
    private string generateOutputFile(string outputFileName)
    {
        string str = generateOutputFile(outputFileName, this.tree);

        string formattedCode = AssemblyFormatter.FormatAssembly(str);
        File.WriteAllText(outputFileName, formattedCode);

        return formattedCode;
    }

    private string generateOutputFile(string outputFileName, TokenTreeNode tk, int tokenCount = 0)
    {
        string str = "";

        if(tk.value.Type != TokenType.GreaterThan)
            str = GenerateOpcodes(tk.value);
            // TODO: find a way to do this that isn't hard coding an exception for this case 
        
        if(tk.value.Value == "main")
        {
            Console.WriteLine("");
            tokenCount--;
        }   
        
        foreach (var child in tk.Children)
        {
            str+=" ";
            str += this.generateOutputFile(outputFileName, child, ++tokenCount);
        }
        if(tk.value.Type == TokenType.GreaterThan)
            str += " " + GenerateOpcodes(tk.value);
            // TODO: find a way to do this that isn't hard coding an exception for this case 

        return str;
    }
}