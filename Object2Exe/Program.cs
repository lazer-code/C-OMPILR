namespace lInker
{
	class Entry
	{
		public static void Main(string[] args)
		{
			COFFObjectFile cof = new(@"test.o");
			cof.parse();
		}
	}
}