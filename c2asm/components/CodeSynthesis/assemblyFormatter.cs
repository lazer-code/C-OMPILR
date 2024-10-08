using System.Text;

namespace cOMPILR.Components.Synthesis
{
	class AssemblyFormatter
	{

		// all the single operand instruction in asm, list needs changing 
		private static readonly List<string> singleParameterInstructions = ["jg", "jne", "je", "jb", "jbe", "jge", "jl", "jle"];
		
		// all the no operand instructions in asm, list also needs change
		private static readonly List<string> noParameterInstructions = ["ret", "dex"];
		
		public static string FormatAssembly(string assemblyCode)
		{
			var instructions = assemblyCode.Split(' ');
			var formattedCode = new StringBuilder();

			int i = -1;

			// loop over every instruction, if it has a single operand print both of them in the same line with a comma between the first and second operand,
			// if it only has a single operand print it and add a new line after it
			foreach (var instruction in instructions)
			{
				if (instruction.Trim().Length != 0)
				{
					if (i < 0)
					{
						if (i == -2)
						{
							// if this is the first operand, print it and add a comma afterwards
							formattedCode.Append(instruction + ", ");
						}

						else if (i == -1)
						{
							// if this is the last operand add a newline afterwards
							formattedCode.Append(instruction + "\n");
						}
					}
					
					else
					{
						// if this is the instruction print it and add a space afterwards for the first operand, set operand id to -2 ( first operand)
						formattedCode.Append(instruction + " ");
						i = -3;
					}

					if (singleParameterInstructions.Contains(instruction))
					{
						// if its a single operand instruction, skip the first operand
						i++;
					}

					if(noParameterInstructions.Contains(instruction))
					{
						formattedCode.Append('\n');
						i+=2;
					}

					if(instruction.Contains(":"))
					{
						i+=2;
						formattedCode.Append('\n');
					}
					
					i++;
				}
			}

			return formattedCode.ToString();
		}
	}
}