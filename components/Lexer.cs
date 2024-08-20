using System.Linq.Expressions;
using System.Net;
using System.Runtime.Versioning;

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

	class Condition
	{
		Func<string, int, bool> Expression;
		public Condition (Func<string, int, bool> expression)
		{
			Expression = expression;
		}

		public bool matches(string token, int level) => Expression(token, level);
	}

	class TokenClassifier : Condition
	{
		TokenType RetVal;
		public TokenClassifier (TokenType retVal, Func<string, int, bool> expression) : base (expression)
		{
			RetVal = retVal;
		}

		public TokenType GetTokenType () => RetVal;
		public new TokenType matches(string token, int level) => base.matches(token, level) ? RetVal : TokenType.Unknown;
	}

	class Lexer
	{

		static List<TokenClassifier> conditions = new List<TokenClassifier>();

		private static void addConditions()
		{
			conditions.Add(new TokenClassifier(TokenType.Identifier_var, (string value, int level) =>ClassificationHelper.IsIdentifier(value) && !value.Contains('(')));
			conditions.Add(new TokenClassifier(TokenType.Assignment, (string value, int level) => value == "="));
			conditions.Add(new TokenClassifier(TokenType.Number, (string value, int level) => ClassificationHelper.IsNumeric(value)));
			conditions.Add(new TokenClassifier(TokenType.Keyword, (string value, int level) => ClassificationHelper.IsKeyword(value)));
			conditions.Add(new TokenClassifier(TokenType.Operator, (string value, int level) => ClassificationHelper.IsOperator(value)));
			conditions.Add(new TokenClassifier(TokenType.String, (string value, int level) => value.Contains('"')));

		}

		static TokenType GetTokenType(string tokenValue, int blockIndex = 0)
		{
			var returnType = from condition in conditions // loop over all the conditions
			where condition.matches(tokenValue, blockIndex) != TokenType.Unknown // and if one of them is a match
			select condition;	// happy
		
			#region Debugging Output
			if(returnType.Count() > 0)
				Debug.Output("classified " + returnType.ElementAt(0).GetTokenType().ToString(), ConsoleColor.DarkGray);

			#endregion		
			
			return returnType.Count() > 0 ? returnType.ElementAt(0).GetTokenType() : TokenType.Unknown; //return the token type we found (if any), return Unknown if no matches were found
		}

		public static TokenTreeNode ParseEntryFile(List<Token> tokens)
		{
			addConditions();
			TokenTreeNode root = new(TokenType.Unknown, "root", false);
			TokenTreeNode currentParent = root;

			int blockIndex = 0;
			bool finishedParams = true;
			bool goingOverArgs = false;

			
			TokenType type;

			for (int i = 0; i < tokens.Count; i++)
			{
				if (currentParent == null)
					continue;

				if((type = GetTokenType(tokens[i].value)) != TokenType.Unknown)
				{
					currentParent.AddChild(new TokenTreeNode(type, tokens[i].value, goingOverArgs));
				}

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
					else if(ClassificationHelper.IsComparison(tokens[i].value))
					{
						TokenType ntype = ClassificationHelper.GetComparisonType(tokens[i].value);
						currentParent.AddChild(new TokenTreeNode(ntype, tokens[i].value, false));
					}

					else if (tokens[i].value.Contains('(') && !ClassificationHelper.IsKeyword(tokens[i - 1].value))
					{
						// finding function calls
						currentParent.AddChild(new TokenTreeNode(TokenType.Call, tokens[i - 1].value, false));
						goingOverArgs = true;
					}
				}
			}

			return root;
		}
	}
}