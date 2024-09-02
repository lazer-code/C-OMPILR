namespace Components
{
	/// <summary>
	/// for the actual assembly generation part of this class, check out AssemblyGenerator.cs
	/// </summary>
	partial class Synthesizer(TokenTreeNode tokenTree)
    {
		TokenTreeNode tree = tokenTree;
		Dictionary<string, int> varOffsets = [];

        private Dictionary<string, int> GenerateASMLocations(string[] variables)
		{
			// turn all of the identifiers to offsets
			Dictionary<string, int> res = [];
			int lastLocation = 0;

			foreach (string variable in variables)
				res[variable] = lastLocation += 8;

			this.varOffsets = res;

			return this.varOffsets;
		}

		public string Synthesize(string output)
		{
			Dictionary<string, int> variableLocations = new();

			// extract variables and assign them locations
			List<string> variables = Helper.extractVariableNames(this.tree);
			variables.ForEach(i => Console.WriteLine(i));
			variableLocations = GenerateASMLocations(variables.ToArray());

			// generate the assembly code and write it to a file and the screen
			string res = "Table of variables' offsets\n";

			foreach (var variable in variableLocations)
				res += $"{variable.Key} : {variable.Value}\n";

			res += "\n\n\n" + GenerateOutputFile(output);

			return res;
		}
	}
}