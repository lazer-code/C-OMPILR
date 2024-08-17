namespace testCompiler
{
	class Helper
	{
		public static void ColourPrint(string input, ConsoleColor colour)
		{
			Console.ForegroundColor =colour;
			Console.WriteLine(input);
			Console.ResetColor();
		}
		
		/// <summary>
		/// function recursively finds identifiers in the AST and adds every unique one to a list
		/// </summary>
		/// <param name="head">the root of the AST</param>
		/// <returns>all the unique Identifiers in the tree as a List of strings</returns>
		public static List<String> extractVariableNames(TokenTreeNode treeHead)
		{
			// call the recursive function
			List<string> result = [];
			return extractVariableNames(treeHead, result);
		}

		/// <summary>
		/// function recursively finds identifiers in the AST and adds every unique one to a list
		/// </summary>
		/// <param name="head">the root of the AST</param>
		/// <param name="names">the initial list of the names ( empty string List ) </param>
		/// <returns>all the unique Identifiers in the tree as a List of strings</returns>
		private static List<string> extractVariableNames(TokenTreeNode head, List<string> names)
		{
			// the boolean was created just for me to be able to not use a if else function and still be able to call the function
			bool added = (head.Value.type == TokenType.Identifier_var || head.Value.type == TokenType.Identifier_func) && names.AddIfNotIncluded(head.Value.value);

			// recuresively traverse the entire tree looking for identifier
			foreach (var child in head.Children)
				names = extractVariableNames(child, names);

			return names;
		}
	}
}