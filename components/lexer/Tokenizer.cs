namespace testCompiler
{
	public enum TokenType
	{
    	Operator, Parenthesis, Brace, Bracket, Semicolon, Comma, Dot, Keyword, Number, String, Char, Identifier_var, Identifier_func, Comment, LogicalAnd, LogicalOr, LogicalNot, Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, Assignment, BitwiseAnd, BitwiseOr, BitwiseXor, BitwiseNot, LeftShift, RightShift, EOF, Unknown, Body, Call
	}

	public struct Token(string value, TokenType type = TokenType.Unknown)
    {
        readonly string value = value;
		public TokenType type = type;

        public readonly string GetValue() => this.value;
	}

	public class TokenizationHelper(string input)
    {
		private readonly string input = input;
		private int currentPosition = 0;

        public bool HasNext() => this.currentPosition < this.input.Length;

		public string? GetNextToken()
		{
			// Skip whitespace characters
			while (HasNext() && Char.IsWhiteSpace(input[currentPosition]))
				currentPosition++;

			// Check for end of string
			if (!HasNext())
				return null;

			// Identify token based on first character
			int start = currentPosition;

			if (Char.IsLetterOrDigit(input[currentPosition]))
			{
				// Read alphanumeric characters
				while (HasNext() && (Char.IsLetterOrDigit(input[currentPosition]) || input[currentPosition] == '_'))
					currentPosition++;
			}

			else if (input[currentPosition] == '"')
			{
				// Read string literal (ignoring escape sequences for simplicity)
				currentPosition++;

				while (HasNext() && input[currentPosition] != '"')
					currentPosition++;

				// Include quotes in the token
				if (HasNext())
					currentPosition++;
			}

			else
			{
				// Single character tokens (e.g., operators, punctuation)
				currentPosition++;
			}

			return input[start..currentPosition];
		}
	}

	class Tokenizer
	{
		public static List<Token> tokenize(string content)
		{
			string[] wordsInFile = content.Split(" ;,\t\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			List<Token> tokens = [];

			TokenizationHelper tokenizer = new TokenizationHelper(content);

			while (tokenizer.HasNext())
			{
				string? tkn = tokenizer.GetNextToken();
				if (tkn != null)
				{
					Token token = new(tkn);
					tokens.Add(token);
				}
			}

			return tokens;
		}
	}
}