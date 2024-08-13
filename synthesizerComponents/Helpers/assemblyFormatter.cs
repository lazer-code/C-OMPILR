using System.Text;
using System.Text.RegularExpressions;

namespace cOMPILR.POC.TextFormatters
{
	class AssemblyFormatter
	{

		private static readonly List<string> singleParameterInstructions = ["jg", "jne", "jb", "jbe", "jge"];
		public static string FormatAssembly(string assemblyCode)
		{
			var instructions = assemblyCode.Split(' ');
			var formattedCode = new StringBuilder();

			int i = -1;
			foreach (var instruction in instructions)
			{
				if (instruction.Trim().Length == 0)
				{

				}
				else
				{
					if (i < 0)
					{
						if (i == -2)
						{
							formattedCode.Append(" " + instruction + ",");
						}
						else if (i == -1)
						{
							formattedCode.Append(" " + instruction + "\n");
						}
					}
					else
					{
						formattedCode.Append(instruction);
						i = -3;
					}
					if (singleParameterInstructions.Contains(instruction))
					{
						i++;
					}
					i++;
				}
			}

			return formattedCode.ToString();
		}
	}
}