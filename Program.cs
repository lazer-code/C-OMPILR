class Entry
{
    public static void Main(string[] args)
    {
        Console.WriteLine("hello, world!");
        
        TokenTreeNode tt = new TokenTreeNode(new Token(TokenType.Unknown, ""));
        
        TokenTreeNode newHead = tt.AddChild(new Token(TokenType.Operator, "+"));
        newHead.AddChild(new Token(TokenType.Number, "10"));
        newHead.AddChild(new Token(TokenType.Number, "15"));
        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "if"));
        newHead.AddChild(new Token(TokenType.Number, "4"));
        newHead.AddChild(new Token(TokenType.Operator, "+"));
        newHead.AddChild(new Token(TokenType.Number, "5"));
        newHead.AddChild(new Token(TokenType.GreaterThanOrEqual, "9"));
        
        
        newHead = newHead.AddChild(new Token(TokenType.EOF, ""));
        newHead.AddChild(new Token(TokenType.Identifier, "NewFunction()"));


        Synthesizer s = new Synthesizer(tt);
        s.synthesize();
    }
}