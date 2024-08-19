namespace Components
{
	class Entry
	{
		public static void Main(string[] args)
		{
			if (args.Length < 1)
				return;

			string filename = args[1];	
			string folderRoot = "./";
			if(args.Length > 0)
				folderRoot = args[0];

			string res = Preprocessor.Preparse(folderRoot + filename);
			Helper.ColourPrint("\n\n\nThe Preprocessor has generated:", ConsoleColor.Blue);
            Console.WriteLine(res);

			List<Token> tokens = Tokenizer.Tokenize(res);

			Helper.ColourPrint("\n\n\nThe Tokenizer has generated:", ConsoleColor.Blue);
			res = "[" + string.Join(", ", tokens.Select(x => x.ToString())) + "]";
			Console.WriteLine(res);

			TokenTreeNode res1 = Lexer.ParseEntryFile(tokens);
			Helper.ColourPrint("\n\n\nThe Lexer has generated:", ConsoleColor.Blue);
			res1.PrintTree();

			Helper.ColourPrint("\n\n\nThe Parser has generated:", ConsoleColor.Blue);
			TokenTreeNode? res2 = Parser.Parse(res1);
			res2?.PrintTree();
			
			if (res2 != null)
			{
				Synthesizer synthesizer = new(res2);
				res = synthesizer.Synthesize(folderRoot + "output.S");

				Helper.ColourPrint("\n\n\nThe Synthesizer has generated:", ConsoleColor.Blue);
				Console.WriteLine(res);
			}
		}
	}
}