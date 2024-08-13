using System.Data.Common;
using cOMPILR.POC.TextFormatters;

partial class Synthesizer
{
    List<string> conditionsBodies = new();
    
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
            TokenType.EOF => "",
            TokenType.Assignment => "movl",
            TokenType.Identifier_var => this.varOffsets[tk.Value] + "(%esp)",
            TokenType.Identifier_func => "_" + tk.Value + ":",
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
            TokenType.Body => getBodyName(),
            _ => throw new Exception("invalid line ( for now )")
        };
    }

    string getBodyName()
    {
        return "L" + (conditionsBodies.Count + 1);
    }

    string addBodies(TokenTreeNode tk, bool isCurrentlyInABody = false, string str = "", int bodyCount = 0)
    {
        
        if(tk.value.Type == TokenType.Body || isCurrentlyInABody)
        {
            bodyCount++;
            if(!isCurrentlyInABody)
            {
                str += "L" + bodyCount + ": ";
                isCurrentlyInABody = true;
            }
            else
            {
                str = " " + GenerateOpcodes(tk.value);
            }

            foreach (var child in tk.Children)
            {
                str += this.addBodies(child, true, str, bodyCount);
            }

            if(tk.value.Type == TokenType.Keyword && tk.value.Value == "return")
            {
                str += " ret";
            }

        }
        else
        {
            foreach (var child in tk.Children)
            {
                str = this.addBodies(child, false, str, bodyCount);
            }
        }
        return str;
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

        str += addBodies(this.tree);

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
        
        if(tk.value.Type != TokenType.Body)
        {
            // recursively traverse the tree and add all the instructions to a string 
            foreach (var child in tk.Children)
            {
                str += " ";
                str += this.generateOutputFile(child);
            }
        }
        else
        {
            str += " ";
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