class Debug
{

    public static void Output(string message)
    {
        Console.WriteLine(message);
    }

    public static void Output(string input, ConsoleColor colour)
	{
		Console.ForegroundColor =colour;
		Console.WriteLine(input);
		Console.ResetColor();
	}

    public static void Error(string message)
    {
        __write_internal(message, "Errors", ConsoleColor.Yellow);
    }

    public static void Warn(string message)
    {
        __write_internal(message, "Warnings", ConsoleColor.Yellow);
    }

    private static void __write_internal(string message, string outputPath, ConsoleColor color)
    {
        File.WriteAllText("/log/" + outputPath + "/" + DateTime.Now.ToString() + ".txt", message);
        Output(message, color);
    }
} 