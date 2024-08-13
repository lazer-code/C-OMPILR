using System.Data.Common;
using cOMPILR.POC.TextFormatters;

partial class Synthesizer
{

    /// <summary>
    /// turns a token to an assembly command
    /// </summary>
    /// <param name="tk">the token</param>
    /// <returns>the assembly command</returns>
    /// <exception cref="Exception">we currently only support a small number of operands, in case we haven't written something yet an exception will be thrown</exception>
    private string GenerateOpcodes(Token tk)
    {
        // turn the token to a asm operation, we prolly need a better way for doing this
        return tk.Type switch
        {
            TokenType.Assignment => "mov",
            TokenType.Identifier => "[ds+" + this.varOffsets[tk.Value] + "]",
            TokenType.Number => "$" + tk.Value,
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
                "return" => "mov eax",
                _ => throw new Exception("unknown keyword" + tk.Value)
            },
            TokenType.COF => "setJumpAddress",
            TokenType.Body => "setJumpAddress",
            _ => throw new Exception("invalid line ( for now )")
        };
    }

    /// <summary>
    ///  recursively turn the tokens to assembly code and write it to a file
    /// </summary>
    /// <param name="outputFileName"> the file the assembly will be written to </param>
    /// <returns></returns>
    private string generateOutputFile(string outputFileName)
    {
        // get the assembly
        string str = generateOutputFile(this.tree);

        // format it and write it to a file
        string formattedCode = AssemblyFormatter.FormatAssembly(str);
        File.WriteAllText(outputFileName, formattedCode);

        // return the valid assembly as a string, just for fun
        return formattedCode;
    }

    /// <summary>
    /// recursively turn the tokens to opcodes and operands
    /// </summary>
    /// <param name="tk">the token tree root</param>
    /// <returns>a string with all opcodes (unformatted ) </returns>
    private string generateOutputFile(TokenTreeNode tk)
    {
        string str = "";

        // if the instruction is greater than, add it after its children
        if(tk.value.Type != TokenType.GreaterThan)
            str = GenerateOpcodes(tk.value); // turn the instruction to a opcode
            // TODO: find a way to do this that isn't hard coding an exception for this case 
        
        // recursively traverse the tree and add all the instructions to a string 
        foreach (var child in tk.Children)
        {
            str += " ";
            str += this.generateOutputFile(child);
        }

        // if the instruction is greater than, add it after its children
        if(tk.value.Type == TokenType.GreaterThan)
            str += " " + GenerateOpcodes(tk.value);
            // TODO: find a way to do this that isn't hard coding an exception for this case 

        if(tk.value.Type == TokenType.Keyword && tk.value.Value == "return")
        {
            str += " ret";
        }

        // return the string with all the instructions
        return str;
    }
}