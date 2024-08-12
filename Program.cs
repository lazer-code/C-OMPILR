class Entry
{
    public static void Main(string[] args)
    {     
        #region sample tree creation   
        TokenTreeNode tt = new TokenTreeNode(new Token(TokenType.Identifier, "main"));
        
        TokenTreeNode newHead = tt.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Identifier, "a"));
        newHead.AddChild(new Token(TokenType.Number, "25"));
        
        //newHead = newHead.AddChild(new Token(TokenType.Operator, "+"));
        //newHead.AddChild(new Token(TokenType.Number, "15"));
        //newHead.AddChild(new Token(TokenType.Number, "10"));

        newHead = tt.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Identifier, "b"));
        newHead.AddChild(new Token(TokenType.Number, "28"));

        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "if"));
        var ifGreaterHead = newHead.AddChild(new Token(TokenType.GreaterThan, ">"));
        ifGreaterHead.AddChild(new Token(TokenType.Identifier, "a"));
        ifGreaterHead.AddChild(new Token(TokenType.Identifier, "b"));
        
        newHead = newHead.AddChild(new Token(TokenType.Body, ""));
        newHead = newHead.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "15"));

        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "10"));
        #endregion
        
        Synthesizer s = new Synthesizer(tt);
        s.Synthesize();
    }
}