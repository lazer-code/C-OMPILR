namespace Components
{
	public class TokenizationHelper(string input)
    {
		private readonly string input = input;
		private int currentPosition = 0;

        public bool HasNext() => this.currentPosition < this.input.Length;

		public string? GetNextToken()
		{
			// Skip whitespace characters
			while (HasNext() && char.IsWhiteSpace(input[currentPosition]))
				currentPosition++;

			// Check for end of string
			if (!HasNext())
				return null;

			// Identify token based on first character
			int start = currentPosition;

			if (char.IsLetterOrDigit(input[currentPosition]))
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
		public static List<Token> Tokenize(string content)
		{
			string[] wordsInFile = content.Split(" ;,\t\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			List<Token> tokens = [];

			TokenizationHelper tokenizer = new(content);

			while (tokenizer.HasNext())
			{
				string? tkn = tokenizer.GetNextToken();
				if (tkn != null)
				{
					Token token = new(tkn);
					tokens.Add(token);
				}
			}

			HashSet<string> conditions = ["!", "<", ">", "="];
			List<Token> newTokens = [];

			for (int i = 0; i < tokens.Count; i++)
			{
				Token curr = tokens[i];

				if (i < tokens.Count - 1 && tokens[i+1].value == "=" && conditions.Contains(tokens[i].value))
				{
					i++;
					curr.value += "=";
				}

				newTokens.Add(curr);
			}
			
			return newTokens;
		}
	}
}