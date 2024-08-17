using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;
using System.Timers;

namespace testCompiler
{
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
			this.Value = new Token(ttn.Value.GetValue(), ttn.Value.type);
			this.Children = new List<TokenTreeNode>();
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

		public override string ToString() => $"[isArgument : {this.IsArgument}]{this.Value.type.ToString()}({this.Value.GetValue()})";

		public void PrintTree() => PrintNode(this, 0);

		private static void PrintNode(TokenTreeNode node, int indentLevel)
		{
			string indent = new(' ', indentLevel * 4);
			Console.WriteLine($"{indent}{node.Value.type.ToString()}({node.Value.GetValue()})" + (node.IsArgument ? "[ARGUMENT]" : ""));

			foreach (var child in node.Children)
                PrintNode(child, indentLevel + 1);
		}
	}

	class classificationHelper
	{
		// C reserved words, see https://en.cppreference.com/w/c/keyword
		static readonly List<string> keywords = ["alignas", "alignof", "auto", "bool", "break", "case", "char", "const", "constexpr", "continue", "default", "do", "double", "else", "enum", "extern", "false", "float", "for", "goto", "if", "inline", "int", "long", "nullptr", "register", "restrict", "return", "short", "signed", "sizeof", "static", "static_assert", "struct", "switch", "thread_local", "true", "typedef", "typeof", "typeof_unqual", "union", "usigned", "void", "volatiole", "while"];
		static readonly List<string> operators = ["+", "*", "-", "/", "%"];
		static readonly List<string> comparisonOperators = ["==", ">=", "<=", ">", "<"];
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

		public static bool isOperator(string tokenValue) => operators.Contains(tokenValue.Trim());
		public static bool isComparison(string tokenValue) => comparisonOperators.Contains(tokenValue.Trim());

		public static TokenType getComparisonType(string tokenValue)
		{
			return tokenValue switch
			{
				"==" => TokenType.Equal,
				"!=" => TokenType.NotEqual,
				"<" => TokenType.LessThan,
				">" => TokenType.GreaterThan,
				"<=" => TokenType.LessThanOrEqual,
				">=" => TokenType.GreaterThanOrEqual,
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
		public static TokenTreeNode ParseEntryFile(string folderRoot)
		{
			string[] files = Directory.GetFiles(folderRoot);
			
			string fileText = File.ReadAllText(files[1]);

			List<Token> tokens = Tokenizer.tokenize(fileText);

			TokenTreeNode root = new(TokenType.Unknown, "root", false);
			TokenTreeNode currentParent = root;

			int blockIndex = 0;
			bool finishedParams = true;
			bool goingOverArgs = false;

			for (int i = 0; i < tokens.Count; i++)
			{
				if (currentParent == null)
					continue;

				if (classificationHelper.IsIdentifier(tokens[i].GetValue()) && !tokens[i + 1].GetValue().Contains('('))
					currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_var, tokens[i].GetValue(), goingOverArgs));

				else if (tokens[i].GetValue().Contains("="))
					currentParent.AddChild(new TokenTreeNode(TokenType.Assignment, tokens[i].GetValue(), goingOverArgs));

				else if (classificationHelper.IsNumeric(tokens[i].GetValue()))
					currentParent.AddChild(new TokenTreeNode(TokenType.Number, tokens[i].GetValue(), goingOverArgs));

				else if (tokens[i].GetValue().Trim() != "")
				{
					if (tokens[i].GetValue() == "{" || tokens[i].GetValue().Contains('{'))
					{
						if(currentParent.Children[0].Value.type == TokenType.Identifier_func)
							currentParent = currentParent.Children[0];

						else
						{
							var body = new TokenTreeNode(TokenType.Body, "", false);
							currentParent.AddChild(body);
							currentParent = body;
						}

						blockIndex++;
					}

					else if (tokens[i].GetValue() == "}" || tokens[i].GetValue().Contains('}'))
					{
						blockIndex--;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        currentParent = currentParent.Parent;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    }

					else if (tokens[i].GetValue() == ")" && blockIndex != 0)
						goingOverArgs = false;

					else if (blockIndex == 0)
					{
						if (!tokens[i].GetValue().Contains('('))
						{
							if (!finishedParams)
							{
								if (!tokens[i].GetValue().Equals(")"))
								{
									// finding function parameters
									currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_var, tokens[i].GetValue(), true));
								}
							}

							else
							{
								// finding function modifiers
								currentParent.AddChild(new TokenTreeNode(TokenType.Keyword, tokens[i].GetValue(), false));
							}
						}
						else if (tokens[i].GetValue().Contains(')'))
						{
							finishedParams = true;

							if (tokens[i].GetValue().Contains('('))
							{
								currentParent.Children.RemoveAt(currentParent.Children.Count - 1);
								currentParent.Children.RemoveAt(currentParent.Children.Count - 1);

								currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_func, tokens[i - 2].GetValue() + " " + tokens[i - 1].GetValue(), goingOverArgs));

								string funcParams = tokens[i].GetValue()[tokens[i].GetValue().IndexOf('(')..tokens[i].GetValue().IndexOf(')')];
								
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

							currentParent.AddChild(new TokenTreeNode(TokenType.Identifier_func, tokens[i - 2].GetValue() + " " + tokens[i - 1].GetValue(), false));

							finishedParams = false;
						}
					}

					else if(classificationHelper.IsKeyword(tokens[i].GetValue()))
						currentParent.AddChild(new TokenTreeNode(TokenType.Keyword, tokens[i].GetValue(), false));

					else if(classificationHelper.isOperator(tokens[i].GetValue()))
						currentParent.AddChild(new TokenTreeNode(TokenType.Operator, tokens[i].GetValue(), false));

					else if(classificationHelper.isComparison(tokens[i].GetValue()))
					{
						TokenType type = classificationHelper.getComparisonType(tokens[i].GetValue());
						currentParent.AddChild(new TokenTreeNode(type, tokens[i].GetValue(), false));
					}

					else if (tokens[i].GetValue().Contains('(') && !classificationHelper.IsKeyword(tokens[i - 1].GetValue()))
					{
						// finding function calls
						currentParent.AddChild(new TokenTreeNode(TokenType.Call, tokens[i - 1].GetValue(), false));
						goingOverArgs = true;
					}

					if (tokens[i].GetValue().Contains('"') && currentParent != null)
					{
						// finding a string
						int startIndex = tokens[i].GetValue().IndexOf('"'), endIndex = tokens[i].GetValue().LastIndexOf('"');
						currentParent.AddChild(new TokenTreeNode(TokenType.String, tokens[i].GetValue().Substring(startIndex, endIndex - startIndex + 1), goingOverArgs));
					}
				}
			}

			return root;
		}

		public static List<LexResultObject> ParseEntryFile2(string folderRoot)
		{
			List<LexResultObject> resultObjects = [];
			string[] files = Directory.GetFiles(folderRoot);
			string fileText = File.ReadAllText(files[0]);

			#region debugging output 
			
			// seperate string on whitespaces
			string[] wordsInFile = fileText.Split(null);

			#endregion

			#region classifying objects
			
			int blockIndex = 0;

			for (int i = 0; i < wordsInFile.Length; i++)
			{
				if (wordsInFile[i].Trim() != "")
				{
					if (wordsInFile[i] == "{" || wordsInFile[i].Contains('{'))
					{
						if (wordsInFile[i - 1].Trim() == "" && blockIndex == 0)
							resultObjects.RemoveAt(resultObjects.Count - 1);

						blockIndex++;
					}

					if (wordsInFile[i] == "}" || wordsInFile[i].Contains('}'))
						blockIndex--;

					else if (blockIndex == 0)
					{
						if(wordsInFile[i].Contains('('))
						{
							// finding function definitions
							//resultObjects.RemoveAt(resultObjects.Count);
							resultObjects.Add(new LexResultObject(TokenType.Identifier_func, wordsInFile[i - 1] + " " + wordsInFile[i]));
						}
					}
					
					else if (wordsInFile[i].Contains('('))
					{
						// finding function calls
						resultObjects.Add(new LexResultObject(TokenType.Call, wordsInFile[i][..wordsInFile[i].IndexOf('(')]));
					}

					if (wordsInFile[i].Contains('"'))
					{
						// finding a string
						int startIndex = wordsInFile[i].IndexOf('"');
						int endIndex = wordsInFile[i].IndexOf('"', startIndex + 1);

						string strLiteral = "";
						strLiteral += wordsInFile[i];

						bool foundStrEnd = endIndex != -1;

						while (!foundStrEnd)
						{
							i++;
							strLiteral += " " + wordsInFile[i];
							endIndex = wordsInFile[i].IndexOf('"');
							foundStrEnd = endIndex != -1;
						}

						endIndex = strLiteral.LastIndexOf('"');

						resultObjects.Add(new LexResultObject(TokenType.String, strLiteral.Substring(startIndex, endIndex - startIndex + 1)));
					}
				}
			}
			#endregion

			return resultObjects;
		}
	}
}