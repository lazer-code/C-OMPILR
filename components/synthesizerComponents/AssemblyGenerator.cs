namespace Components
{
	partial class Synthesizer
	{
		List<string> conditionsBodies = [];
        static HashSet<TokenType> conditionsOperators = [TokenType.Equal, TokenType.NotEqual, TokenType.GreaterThan, 
		TokenType.GreaterThanOrEqual, TokenType.LessThan, TokenType.LessThanOrEqual];

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
				TokenType.Identifier_var => $"{this.varOffsets[tk.value]}(%esp)",
				TokenType.Identifier_func => $"_{tk.value}:",
				TokenType.Number => $"${tk.value}",
				TokenType.Equal => "je",
				TokenType.NotEqual => "jne",
				TokenType.GreaterThan => "jg",
				TokenType.GreaterThanOrEqual => "jge",
				TokenType.LessThan => "jl",
				TokenType.LessThanOrEqual => "jle",

				TokenType.Operator => tk.value switch
				{
					"+" => "add",
					"*" => "mul",
					_ => throw new Exception($"unknown operator{tk.value}did you forget to implement it?"),
				},
				TokenType.Keyword => tk.value switch
				{
					"if" => "",
					"return" => "movl",
					"int" => "",
					_ => throw new Exception($"unknown keyword {tk.value}")
				},
				TokenType.Body => GetBodyName(),
				_ => throw new Exception("invalid line ( for now ) on token " +tk.GetType().Name + "("  + tk.value + ")"),
			};
		}

		string GetBodyName()
		{
			conditionsBodies.Add("" + (conditionsBodies.Count + 1));

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

				if (tk.Value.type == TokenType.Keyword && tk.Value.value == "return")
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
		/// recursively turn the tokens to opcodes and operands
		/// </summary>
		/// <param name="tk">the token tree root</param>
		/// <returns>a string with all opcodes ( unformatted ) </returns>
		private string GenerateOutputFile(TokenTreeNode tk)
		{
			string str = "";

			// if the instruction is greater than, add it after its children
			if (!conditionsOperators.Contains(tk.Value.type))
				str = GenerateOpcodes(tk.Value); // turn the instruction to a opcode
												 // TODO: find a way to do this that isn't hard coding an exception for this case 

			if (tk.Value.type != TokenType.Body)
			{
				// recursively traverse the tree and add all the instructions to a string 
				foreach (var child in tk.Children)
				{
					if (!conditionsOperators.Contains(tk.Value.type))
					{
						str += " ";
						str += this.GenerateOutputFile(child);
					}

				}
			}

			else
				str += " ";

			// if the instruction is greater than, add it after its children
			if (conditionsOperators.Contains(tk.Value.type))
			{
				str += HandleComparisonOperands(tk.Children);
				str += " " + GenerateOpcodes(tk.Value);
			}
			// TODO: find a way to do this that isn't hard coding an exception for this case 

			if (tk.Value.type == TokenType.Keyword && tk.Value.value == "return")
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
				leftOffset = this.varOffsets[left.Value.value];
				if(right.Value.type == TokenType.Identifier_var)
				{
					rightOffset = this.varOffsets[right.Value.value];
					return $"movl {rightOffset}(%esp) %eax cmpl {leftOffset}(%esp) %eax";
				}

				else
					return $"movl ${right.Value.value} %eax cmpl {leftOffset}(%esp) %eax";
			}

			else if(right.Value.type == TokenType.Identifier_var)
			{
				rightOffset = this.varOffsets[right.Value.value];
				return $"movl {rightOffset}(%esp) %eax cmpl ${left.Value.value} %eax";
			}

			else
				return $"cmpl ${left.Value.value} ${right.Value.value}";			
		}
	}
}