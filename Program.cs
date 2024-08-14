class Entry
{
    public static void Main(string[] args)
    {     
        #region sample tree creation   
        TokenTreeNode tt = new TokenTreeNode(new Token(TokenType.Identifier_func, "main"));
        
        TokenTreeNode newHead = tt.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Number, "29"));
        newHead.AddChild(new Token(TokenType.Identifier_var, "a"));

        //newHead = newHead.AddChild(new Token(TokenType.Operator, "+"));
        //newHead.AddChild(new Token(TokenType.Number, "15"));
        //newHead.AddChild(new Token(TokenType.Number, "10"));

        newHead = tt.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Number, "28"));
        newHead.AddChild(new Token(TokenType.Identifier_var, "b"));

        newHead = tt.AddChild(new Token(TokenType.Assignment, "="));
        newHead.AddChild(new Token(TokenType.Number, "30"));
        newHead.AddChild(new Token(TokenType.Identifier_var, "c"));

        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "if"));
        var ifGreaterHead = newHead.AddChild(new Token(TokenType.GreaterThan, ">"));
        ifGreaterHead.AddChild(new Token(TokenType.Identifier_var, "b"));
        ifGreaterHead.AddChild(new Token(TokenType.Identifier_var, "a"));
        
        newHead = newHead.AddChild(new Token(TokenType.Body, ""));
        newHead = newHead.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "15"));
        
        newHead = tt.AddChild(new Token(TokenType.Keyword, "if"));
        ifGreaterHead = newHead.AddChild(new Token(TokenType.GreaterThan, ">"));
        ifGreaterHead.AddChild(new(TokenType.Identifier_var, "c"));
        ifGreaterHead.AddChild(new(TokenType.Identifier_var, "b"));

        newHead = newHead.AddChild(new Token(TokenType.Body, ""));
        newHead = newHead.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "17"));

        newHead = tt.AddChild(new Token(TokenType.Keyword, "return"));
        newHead.AddChild(new Token(TokenType.Number, "10"));
        tt.AddChild(new Token(TokenType.EOF, ""));
        #endregion
        
        Synthesizer s = new Synthesizer(tt);
        s.Synthesize("test.S");
    }
}