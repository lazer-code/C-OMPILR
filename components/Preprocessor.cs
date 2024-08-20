namespace Components
{
    class Preprocessor
    {
        public static string Preparse(string filePath)
        {
            // read all content from file in filePath
            List<string> strings= File.ReadAllLines(filePath).ToList();

            Debug.Output("The source file:", ConsoleColor.Blue);

            foreach (string line in strings)
            {
                Console.WriteLine(line);

                if(line.StartsWith("#define"))
                {
                    string[] parts = line.Split(" ");
                    List<string> strings1 = [];

                    foreach(string line1 in strings)
                        strings1.Add(line1.Replace(parts[1], parts[2]));

                    strings = strings1;
                    strings.Remove(line.Replace(parts[1], parts[2]));
                }
                
                else if (line.StartsWith("#include"))
                {
                    string[] parts = line.Split(' ');
                    
                    //var filename = parts[1].Substring(1, parts[1].Length - 2);
                    var filename = parts[1][1..^1];

                    Console.WriteLine(filename);
                    
                    string[] fileContent;
                    try
                    {
                        fileContent = File.ReadAllLines(filename);
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception("LNK12: COULD NOT FIND FILE " + ex.FileName);
                    }
                    catch(Exception)
                    {
                        throw new Exception("preprocessor could not finish his job. err: " + new Random().NextInt64(0, 599));
                    }
                    
                    strings = [..fileContent, ..strings];
                    strings.Remove(line);
                }
            }

            string text = string.Join("\n", strings);
            return text;
        }
    }
}
