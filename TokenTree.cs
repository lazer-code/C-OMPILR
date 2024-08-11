/*
    Why do we need the "TockenType" enum?
        The "TockenType" enum defines every possible type of each tocken in the tockenlist.

        Here is a list for each type we support and which keywords coincide with its purpose.
         - Operator: + - * /
         - Parenthesis: (, )
         - Brace: {, }
         - Bracket: [, ]
         - Semicolon: ;
         - Comma: ,
         - Dot: .
         - Keyword: if, else, while, for, return
*/
public enum TokenType
{
    Operator, Parenthesis, Brace, Bracket, Semicolon, Comma, Dot, Keyword, Number, String, Char, Identifier, Comment, LogicalAnd, LogicalOr, LogicalNot, Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, Assignment, BitwiseAnd, BitwiseOr, BitwiseXor, BitwiseNot, LeftShift, RightShift, EOF, Unknown
}


struct Token
{
    public TokenType Type;
    public string Value;

    public Token(TokenType type, string value)
    {
        this.Type = type;
        this.Value = value;
    }
}


class TokenTree
{
    public TokenTree Parent{get;}
    public List<TokenTree> Children{get;}
    public Token value{get;}

    public TokenTree(TokenTree parent, Token value, List<TokenTree>? children = null)
    {
        if (children == null)
        {
            this.Children = new List<TokenTree>();
        }

        else
        {
            this.Children = children;
        }

        this.Parent = parent;
    }

    public void AddChild(Token value)
    {
        this.Children.Add(new(this, value));
    }
}