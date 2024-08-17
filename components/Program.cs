namespace testCompiler
{
	class Entry
	{
		public static void Main(string[] args)
		{
			string folderRoot = "./";
			if(args.Length > 0)
				folderRoot = args[0];

			Preprocessor.Preparse(folderRoot + "/entry.c");

			Console.WriteLine("looking for entry file in folder : " + folderRoot);
			List<LexResultObject> res = Lexer.parseEntryFile(folderRoot);

			TokenTreeNode res1 = Lexer.ParseEntryFile(folderRoot);
			res1.PrintTree();

			TokenTreeNode? res2 = Parser.Parse(res1);

			if (res2 != null)
			{
				Synthesizer synthesizer = new(res2);
				synthesizer.Synthesize(folderRoot + "output.S");
			}
		}
	}
}