namespace cOMPILR.Components
{
    public enum TokenType
	{
    	Operator, Parenthesis, Brace, Bracket, Semicolon, Comma, Dot, Keyword, Number, String, Char, Identifier_var, Identifier_func, Comment, LogicalAnd, LogicalOr, LogicalNot, Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, Assignment, BitwiseAnd, BitwiseOr, BitwiseXor, BitwiseNot, LeftShift, RightShift, EOF, Unknown, Body, Call
	}

	public struct Token(string value, TokenType type = TokenType.Unknown)
    {
        public string value = value;
		public TokenType type = type;
        public override string ToString()
        {
            return "{" + this.type + ", " +  this.value + "}";
        }
    }

    public class TokenTreeNode
	{
		public TokenTreeNode? Parent { get; set; }
		public Token Value { get; set; }
		public List<TokenTreeNode> Children { get; set; }
		public bool IsArgument { get; }

		public TokenTreeNode(TokenType type, string content, bool isArgument)
		{
			this.Value = new Token(content, type);
			this.Children = [];
			this.Parent = null;
			this.IsArgument = isArgument;
		}

		public TokenTreeNode(TokenTreeNode ttn)
		{
			this.Value = new Token(ttn.Value.value, ttn.Value.type);
			this.Children = [];
			this.IsArgument = ttn.IsArgument;
			this.Parent = null;
		}

		public void AddChild(TokenTreeNode? child)
		{
			if (child != null)
			{
				child.Parent = this;
				this.Children.Add(child);
			}
		}

		public void PrintTree() => PrintTree(this);

		public static void PrintTree(TokenTreeNode node, int level = 0)
		{
			Console.WriteLine("" + new string('\t', level) + node.Value.type + ":" + node.Value.value);
			
			foreach (var child in node.Children)
                PrintTree(child, level + 1);
		}

		public override string ToString() => $"[isArgument : {this.IsArgument}]{this.Value.type.ToString()}({this.Value.value})";

		private static void PrintNode(TokenTreeNode node, int indentLevel)
		{
			string indent = new(' ', indentLevel * 4);
			Console.WriteLine($"{indent}{node.Value.type.ToString()}({node.Value.value})" + (node.IsArgument ? "[ARGUMENT]" : ""));

			foreach (var child in node.Children)
                PrintNode(child, indentLevel + 1);
		}
	}
}
