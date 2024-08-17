namespace testCompiler
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

		public void Synthesize(string output)
		{
			Dictionary<string, int> variableLocations = new();

			// extract variables and assign them locations
			List<string> variables = Helper.extractVariableNames(this.tree);
			variables.ForEach(i => Console.WriteLine(i));
			variableLocations = GenerateASMLocations(variables.ToArray());

			Helper.ColourPrint("The Lexer has generated:", ConsoleColor.Blue);
            PrintTree(this.tree);

			// generate the assembly code and write it to a file and the screen
			string res = GenerateOutputFile(output);

			Helper.ColourPrint("The Synthesizer has generated:", ConsoleColor.Blue);
			Console.WriteLine(res);

			// print the variables and their offset to have as a reference
			Console.WriteLine("");

			foreach (var variable in variableLocations)
				Console.WriteLine(string.Format("{0} : {1}", variable.Key, variable.Value));
		}

		/// <summary>
		/// recursively prints the tree and all of its values
		/// </summary>
		/// <param name="node">tree root</param>
		/// <param name="level">the tab amount</param>
		public static void PrintTree(TokenTreeNode node, int level = 0)
		{
			Console.WriteLine("" + new string('\t', level) + node.Value.type + ":" + node.Value.GetValue());
			
			foreach (var child in node.Children)
                PrintTree(child, level + 1);
		}
	}
}