public enum TokenType
{
    Operator, Parenthesis, Brace, Bracket, Semicolon, Comma, Dot, Keyword, Number, String, Char, Identifier, Comment, LogicalAnd, LogicalOr, LogicalNot, Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, Assignment, BitwiseAnd, BitwiseOr, BitwiseXor, BitwiseNot, LeftShift, RightShift, COF, EOF, Unknown
}


struct Token
{
    public TokenType Type{get;}
    public string Value{get;}

    public Token(TokenType type, string value)
    {
        this.Type = type;
        this.Value = value;
    }
}


class TokenTreeNode
{
    public TokenTreeNode? Parent{get;}
    public List<TokenTreeNode> Children{get;}
    public Token value{get;}

    public TokenTreeNode(Token value, TokenTreeNode? parent = null, List<TokenTreeNode>? children = null)
    {
        if (children == null)
            this.Children = new List<TokenTreeNode>();
        else
            this.Children = children;


        this.Parent = parent;
        this.value = value;
    }

    public TokenTreeNode AddChild(Token Value)
    {
        TokenTreeNode ttn = new(Value, this);
        this.Children.Add(ttn);
        return ttn;
    }
}