using System.Data.Common;
using cOMPILR.POC.TextFormatters;

namespace testCompiler
{
	partial class Synthesizer
	{
		List<string> conditionsBodies = [];

		/// <summary>
		/// turns a token to an assembly command
		/// </summary>
		/// <param name="tk">the token</param>
		/// <returns>the assembly command</returns>
		/// <exception cref="Exception">we currently only support a small number of operands, in case we haven't written something yet an exception will be thrown</exception>
		private string GenerateOpcodes(Token tk)
		{
			// turn the token to a asm operation, we prolly need a better way for doing this
			return tk.type switch
			{
				TokenType.Unknown => "",
				TokenType.EOF => "",
				TokenType.Assignment => "movl",
				TokenType.Identifier_var => $"{this.varOffsets[tk.GetValue()]}(%esp)",
				TokenType.Identifier_func => $"_{tk.GetValue()}:",
				TokenType.Number => $"${tk.GetValue()}",
				TokenType.Operator => tk.GetValue() switch
				{
					"+" => "add",
					"*" => "mul",
					_ => throw new Exception($"unknown operator{tk.GetValue()}did you forget to implement it?"),
				},
				TokenType.GreaterThan => "jg",
				TokenType.Keyword => tk.GetValue() switch
				{
					"if" => "",
					"return" => "movl",
					"int" => "",
					_ => throw new Exception($"unknown keyword {tk.GetValue()}")
				},
				TokenType.Body => GetBodyName(),
				_ => throw new Exception("invalid line ( for now ) on token " +tk.GetType().Name + "("  + tk.GetValue() + ")"),
			};
		}

		string GetBodyName()
		{
			conditionsBodies.Add("" + (conditionsBodies.Count + 1));

			Helper.ColourPrint($"Body added: L{conditionsBodies.Count}", ConsoleColor.Cyan);

			return "L" + conditionsBodies.Count;
		}

		string AddBodies(TokenTreeNode tk, bool isCurrentlyInABody = false, string str = "", int bodyCount = 0)
		{
			if (tk.Value.type == TokenType.Body || isCurrentlyInABody)
			{
				bodyCount++;
				if (!isCurrentlyInABody)
				{
					string str1 = conditionsBodies[0];
					conditionsBodies.RemoveAt(0);
					str += "L" + str1 + ": ";
					isCurrentlyInABody = true;
				}

				else
					str = " " + GenerateOpcodes(tk.Value);

				foreach (var child in tk.Children)
					str += this.AddBodies(child, true, str, bodyCount);

				if (tk.Value.type == TokenType.Keyword && tk.Value.GetValue() == "return")
					str += " %eax ret ";
			}

			else
				foreach (var child in tk.Children)
					str = this.AddBodies(child, false, str, bodyCount);

			return str;
		}

		/// <summary>
		///  recursively turn the tokens to assembly code and write it to a file
		/// </summary>
		/// <param name="outputFileName"> the file the assembly will be written to </param>
		/// <returns></returns>
		private string generateOutputFile(string outputFileName)
		{
			string str = "";

			// get the assembly
			str += generateOutputFile(this.tree);

			// get the functions
			str += AddBodies(this.tree);

			// format it and write it to a file
			string formattedCode = ".global _main\n\n" + AssemblyFormatter.FormatAssembly(str);
			File.WriteAllText(outputFileName, formattedCode);

			// return the valid assembly as a string, just for fun
			return formattedCode;
		}

		/// <summary>
		/// recursively turn the tokens to opcodes and operands
		/// </summary>
		/// <param name="tk">the token tree root</param>
		/// <returns>a string with all opcodes ( unformatted ) </returns>
		private string generateOutputFile(TokenTreeNode tk)
		{
			string str = "";

			// if the instruction is greater than, add it after its children
			if (tk.Value.type != TokenType.GreaterThan)
				str = GenerateOpcodes(tk.Value); // turn the instruction to a opcode
												 // TODO: find a way to do this that isn't hard coding an exception for this case 

			if (tk.Value.type != TokenType.Body)
			{
				// recursively traverse the tree and add all the instructions to a string 
				foreach (var child in tk.Children)
				{
					if (tk.Value.type != TokenType.GreaterThan)
					{
						str += " ";
						str += this.generateOutputFile(child);
					}

				}
			}

			else
				str += " ";

			// if the instruction is greater than, add it after its children
			if (tk.Value.type == TokenType.GreaterThan)
			{
				str += HandleComparisonOperands(tk.Children);
				str += " " + GenerateOpcodes(tk.Value);
			}
			// TODO: find a way to do this that isn't hard coding an exception for this case 

			if (tk.Value.type == TokenType.Keyword && tk.Value.GetValue() == "return")
				str += " %eax ret ";

			// return the string with all the instructions
			return str;
		}

		private string HandleComparisonOperands(List<TokenTreeNode> children)
		{
			var left = children[0];
			var right = children[1];
			var leftOffset = 0;
			var rightOffset = 0;

			if(left.Value.type == TokenType.Identifier_var)
			{
				leftOffset = this.varOffsets[left.Value.GetValue()];
				if(right.Value.type == TokenType.Identifier_var)
				{
					rightOffset = this.varOffsets[right.Value.GetValue()];
					return $"movl {rightOffset}(%esp) %eax cmpl {leftOffset}(%esp) %eax";
				}

				else
					return $"movl ${right.Value.GetValue()} %eax cmpl {leftOffset}(%esp) %eax";
			}

			else if(right.Value.type == TokenType.Identifier_var)
			{
				rightOffset = this.varOffsets[right.Value.GetValue()];
				return $"movl {rightOffset}(%esp) %eax cmpl ${left.Value.GetValue()} %eax";
			}

			else
				return $"cmpl ${left.Value.GetValue()} ${right.Value.GetValue()}";			
		}
	}
}