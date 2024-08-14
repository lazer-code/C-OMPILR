using System.Data.Common;
using cOMPILR.POC.TextFormatters;

partial class Synthesizer
{
    List<string> bodies = [];
    
    /// <summary>
    /// This function turns a token into an assembly command
    /// </summary>
    /// <param name="tk">The token</param>
    /// <returns>The assembly command</returns>
    /// <exception cref="Exception">We currently only support a small number of operands, in case we haven't written something yet an exception will be thrown</exception>
    private string GenerateOpcodes(Token tk)
    {
        // Turns the token to an assembly operation, we prolly need a better way for doing this
        return tk.Type switch
        {
            TokenType.EOF => "",
            TokenType.Assignment => "movl",
            TokenType.Identifier_var => $"{this.varOffsets[tk.Value]}(%esp)",
            TokenType.Identifier_func => $"_{tk.Value}:",
            TokenType.Number => $"${tk.Value}",
            TokenType.Body => GetBodyName(),

            // Comparison operations
            TokenType.Equal => "je",
            TokenType.NotEqual => "jne",
            TokenType.GreaterThan => "jg",
            TokenType.GreaterThanOrEqual => "jge",
            TokenType.LessThan => "jl",
            TokenType.LessThanOrEqual => "jle",

            // Math operators
            TokenType.Operator => tk.Value switch 
            {
                "+" => "add",
                "*" => "mul",
                "-" => "sub",
                "/" => "div",
                _ => throw new Exception($"unknown operator{tk.Value}did you forget to implement it?"),
            },

            TokenType.Keyword => tk.Value switch
            {
                "if" => "",
                "return" => "movl",
                _ => throw new Exception($"unknown keyword{tk.Value}")
            },

            _ => throw new Exception("invalid line ( for now )")
        };
    }

    /// <summary>
    /// This function returns the name of the current body (conditional or loop)
    /// </summary>
    /// <returns>The name of the current bodey</returns>
    string GetBodyName() => "L" + (bodies.Count + 1);

    /// <summary>
    /// This function adds all bodies in the tree to the .S output file
    /// </summary>
    /// <param name="tk">The current token</param>
    /// <param name="isCurrentlyInABody">If the current token is inside a body</param>
    /// <param name="str">All previouse bodies</param>
    /// <param name="bodyCount">The number of the current bodies ready</param>
    /// <returns></returns>
    string AddBodies(TokenTreeNode tk, bool isCurrentlyInABody = false, string str = "", int bodyCount = 0)
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
                str += this.AddBodies(child, true, str, bodyCount);
            }

            if(tk.value.Type == TokenType.Keyword && tk.value.Value == "return")
            {
                str += " %eax ret";
            }

        }
        else
        {
            foreach (var child in tk.Children)
            {
                str = this.AddBodies(child, false, str, bodyCount);
            }
        }
        return str;
    }
    
    /// <summary>
    ///  This function recursively turns the tokens to assembly code and write it to a file
    /// </summary>
    /// <param name="outputFileName">The name of the output file</param>
    /// <returns>TODO: LATER</returns>
    private string GenerateOutputFile(string outputFileName)
    {
        string str = "";
        
        // get the assembly
        str += GenerateOutputFile(this.tree);

        // get the functions
        str += AddBodies(this.tree);

        // format it and write it to a file
        string formattedCode = ".global _main\n\n" + AssemblyFormatter.FormatAssembly(str);
        File.WriteAllText(outputFileName, formattedCode);

        // return the valid assembly as a string, just for fun
        return formattedCode;
    }

    /// <summary>
    /// This function recursively turns the tokens to opcodes and operands
    /// </summary>
    /// <param name="tk">The token tree root</param>
    /// <returns>A string with all opcodes ( unformatted ) </returns>
    private string GenerateOutputFile(TokenTreeNode tk)
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
                if(tk.value.Type != TokenType.GreaterThan)
                {
                    str += " ";
                    str += this.GenerateOutputFile(child);
                }
                
            }
        }
        else
        {
            str += " ";
        }

        // if the instruction is greater than, add it after its children
        if(tk.value.Type == TokenType.GreaterThan)
        {
            str += HandleComparisonOperands(tk.Children);
            str += " " + GenerateOpcodes(tk.value);
        }
            // TODO: find a way to do this that isn't hard coding an exception for this case 

        if(tk.value.Type == TokenType.Keyword && tk.value.Value == "return")
        {
            str += " %eax ret";
        }

        // return the string with all the instructions
        return str;
    }

    /// <summary>
    /// This function handles comparison
    /// </summary>
    /// <param name="children"></param>
    /// <returns>The correct comparison syntax</returns>
    private string HandleComparisonOperands(List<TokenTreeNode> children)
    {
        var left = children[0];
        var right = children[1];
        var rightOffset = this.varOffsets[right.value.Value];
        var leftOffset = this.varOffsets[left.value.Value];

        return $"movl {rightOffset}(%esp) %eax cmpl {leftOffset}(%esp) %eax";
    }

}