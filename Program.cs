class Entry
{
    public static void Main(string[] args)
    {
        Console.WriteLine("hello, world!");
        TokenTree tt = new(null, new Token(TokenType.Operator, "+"));
        tt.AddChild(new Token(TokenType.Number, "10"));
        
        Synthesizer s = new Synthesizer(tt);
        s.synthesize();
    }
}