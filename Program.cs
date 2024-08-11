class Entry
{
    public static void Main(string[] args)
    {        
        TokenTreeNode tt = new TokenTreeNode(new Token(TokenType.Identifier, "main"));
        TokenTreeNode newHead = tt.AddChild(new Token(TokenType.Identifier, "a"));
        
        newHead = newHead.AddChild(new Token(TokenType.Assignment, "="));
        newHead = newHead.AddChild(new Token(TokenType.Operator, "+"));
        newHead.AddChild(new Token(TokenType.Number, "15"));
        newHead.AddChild(new Token(TokenType.Number, "10"));

        newHead = tt.AddChild(new Token(TokenType.Identifier, "b"));
        newHead.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Number, "28"));

        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "if"));
        newHead = newHead.AddChild(new Token(TokenType.GreaterThan, ">"));
        newHead.AddChild(new Token(TokenType.Identifier, "a"));
        newHead.AddChild(new Token(TokenType.Identifier, "b"));
        newHead = newHead.AddChild(new Token(TokenType.COF, ""));
        newHead = newHead.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "15"));

        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "10"));

        Synthesizer s = new Synthesizer(tt);
        s.Synthesize();
    }
}