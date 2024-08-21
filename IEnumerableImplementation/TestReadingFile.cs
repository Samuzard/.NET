namespace IEnumerableImplementation;

public static class TestReadingFile
{
    public static void Test()
    {
        long memoryBefore = GC.GetTotalMemory(false);
        StreamReader _sr;
        try
        {
            _sr = File.OpenText(@"C:\Build\Dev\DotNetImplementations\IEnumerableImplementation\TestReadingFile.txt");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found");
            return;
        }

        List<string> fileContents = new();
        while (!_sr.EndOfStream)
        {
            fileContents.Add(_sr.ReadLine());
        }

        var stringFound = from line in fileContents
            where line.Contains("string to search for")
            select line;
        
        _sr.Close();
        
        long memoryAfter = GC.GetTotalMemory(false);
        Console.WriteLine("Memory Used Without Iterator = \t" +
                          string.Format(((memoryAfter - memoryBefore) / 1000).ToString(), "n") + "kb");    }
}