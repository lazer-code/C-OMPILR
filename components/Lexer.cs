namespace Components
{
	class ClassificationHelper
	{
		// C reserved words, see https://en.cppreference.com/w/c/keyword
		static readonly List<string> keywords = ["alignas", "alignof", "auto", "bool", "break", "case", "char", "const", "constexpr", "continue", "default", "do", "double", "else", "enum", "extern", "false", "float", "for", "goto", "if", "inline", "int", "long", "nullptr", "register", "restrict", "return", "short", "signed", "sizeof", "static", "static_assert", "struct", "switch", "thread_local", "true", "typedef", "typeof", "typeof_unqual", "union", "usigned", "void", "volatiole", "while"];
		static readonly List<string> operators = ["+", "*", "-", "/", "%"];
		static readonly List<string> comparisonOperators = ["==", "!=", ">=", "<=", ">", "<"];
		public static bool IsIdentifier(string tokenValue)
		{
			tokenValue = tokenValue.Trim();

			if (string.IsNullOrEmpty(tokenValue))
				return false;

			if (keywords.Contains(tokenValue))
				return false;

			// Check for valid first character (letter or underscore)
			if (!char.IsLetter(tokenValue[0]) && tokenValue[0] != '_')
				return false;

			// Check for subsequent characters (letters, numbers, or underscores)
			for (int i = 1; i < tokenValue.Length; i++)
				if (!char.IsLetterOrDigit(tokenValue[i]) && tokenValue[i] != '_')
					return false;

			// Valid identifier
			return true;
		}

		public static bool IsCalculation(string tokenValue)
		{
			if (string.IsNullOrEmpty(tokenValue))
				return false;

			// Simple check for presence of arithmetic operators (+, -, *, /)
			return tokenValue.Contains('+') || tokenValue.Contains('-') || tokenValue.Contains('*') || tokenValue.Contains('/');
		}

		public static bool IsOperator(string tokenValue) => operators.Contains(tokenValue.Trim());
		public static bool IsComparison(string tokenValue) => comparisonOperators.Contains(tokenValue.Trim());

		public static TokenType GetComparisonType(string tokenValue)
		{
			return tokenValue switch
			{
				"==" => TokenType.Equal,
				"!=" => TokenType.NotEqual,
				">" => TokenType.GreaterThan,
				">=" => TokenType.GreaterThanOrEqual,				
				"<" => TokenType.LessThan,
				"<=" => TokenType.LessThanOrEqual,
				_ => throw new Exception("invalid comparison operator. (how did you manage this?)"),
			};
		}

		public static bool IsKeyword(string tokenValue) => !string.IsNullOrWhiteSpace(tokenValue) && keywords.Contains(tokenValue);

		public static bool IsNumeric(string str) => !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
	}

	class LexResultObject(TokenType type, string content, bool isArgument = false)
    {
		private readonly TokenType type = type;
		private readonly string content = content;
		private readonly bool isArgument = isArgument;

        public new TokenType GetType() => this.type;
		public string GetContent() => this.content;

		public override string ToString() => "" + (this.isArgument ? " [argument] " : " ") + this.type.ToString() + "(" + this.content + ")";
	}

	class Lexer
	{
		public static TokenTreeNode ParseEntryFile(List<Token> tokens)
		{
			TokenTreeNode root = new(TokenType.Unknown, "root", false);
			TokenTreeNode currentParent = root;

			int blockIndex = 0;
			bool finishedParams = true;
			bool goingOverArgs = false;

			for (int i = 0; i < tokens.Count; i++)
			{
				if (currentParent == null)
					continue;

				// Checking for variables identigication
				if (ClassificationHelper.IsIdentifier(tokens[i].value) && !tokens[i + 1].value.Contains('('))
					currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_var, tokens[i].value, goingOverArgs));

				// Checking for variable assignment
				else if (tokens[i].value == "=")
					currentParent.AddChild(new TokenTreeNode(TokenType.Assignment, tokens[i].value, goingOverArgs));

				// Checking for numeric values
				else if (ClassificationHelper.IsNumeric(tokens[i].value))
					currentParent.AddChild(new TokenTreeNode(TokenType.Number, tokens[i].value, goingOverArgs));

				else if (tokens[i].value.Trim() != "")
				{	
					// Checking for starting a body of condition or function
					if (tokens[i].value == "{")
					{
						// Checking for function definitions
						if(currentParent.Children[0].Value.type == TokenType.Identifier_func)
							currentParent = currentParent.Children[0];

						// Body definition
						else
						{
							var body = new TokenTreeNode(TokenType.Body, "", false);
							currentParent.AddChild(body);
							currentParent = body;
						}

						blockIndex++;
					}

					// Checking for body closer
					else if (tokens[i].value == "}")
					{
						blockIndex--;
                        currentParent = currentParent.Parent;
                    }

					else if (tokens[i].value == ")" && blockIndex != 0)
						goingOverArgs = false;

					else if (blockIndex == 0)
					{
						if (!tokens[i].value.Contains('('))
						{
							if (!finishedParams)
							{
								if (!tokens[i].value.Equals(")"))
								{
									// finding function parameters
									currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_var, tokens[i].value, true));
								}
							}

							else
							{
								// finding function modifiers
								currentParent.AddChild(new TokenTreeNode(TokenType.Keyword, tokens[i].value, false));
							}
						}
						else if (tokens[i].value.Contains(')'))
						{
							finishedParams = true;

							if (tokens[i].value.Contains('('))
							{
								currentParent.Children.RemoveAt(currentParent.Children.Count - 1);
								currentParent.Children.RemoveAt(currentParent.Children.Count - 1);

								currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_func, tokens[i - 2].value + " " + tokens[i - 1].value, goingOverArgs));

								string funcParams = tokens[i].value[tokens[i].value.IndexOf('(')..tokens[i].value.IndexOf(')')];
								
								if (funcParams.Length > 0)
									currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_var, funcParams, false));
							}
						}

						else
						{
							// finding function definitions

							// removing 2 previous elements (function return type, function name)
							currentParent.Children.RemoveAt(currentParent.Children.Count - 1);
							currentParent.Children.RemoveAt(currentParent.Children.Count - 1);

							currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_func, tokens[i - 2].value + " " + tokens[i - 1].value, false));

							finishedParams = false;
						}
					}

					else if(ClassificationHelper.IsKeyword(tokens[i].value))
						currentParent.AddChild(new TokenTreeNode(TokenType.Keyword, tokens[i].value, false));

					else if(ClassificationHelper.IsOperator(tokens[i].value))
						currentParent.AddChild(new TokenTreeNode(TokenType.Operator, tokens[i].value, false));

					else if(ClassificationHelper.IsComparison(tokens[i].value))
					{
						TokenType type = ClassificationHelper.GetComparisonType(tokens[i].value);
						currentParent.AddChild(new TokenTreeNode(type, tokens[i].value, false));
					}

					else if (tokens[i].value.Contains('(') && !ClassificationHelper.IsKeyword(tokens[i - 1].value))
					{
						// finding function calls
						currentParent.AddChild(new TokenTreeNode(TokenType.Call, tokens[i - 1].value, false));
						goingOverArgs = true;
					}

					if (tokens[i].value.Contains('"') && currentParent != null)
					{
						// finding a string
						int startIndex = tokens[i].value.IndexOf('"'), endIndex = tokens[i].value.LastIndexOf('"');
						currentParent.AddChild(new TokenTreeNode(TokenType.String, tokens[i].value.Substring(startIndex, endIndex - startIndex + 1), goingOverArgs));
					}
				}
			}

			return root;
		}
	}
}