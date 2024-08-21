using System.Collections;

namespace IEnumerableImplementation;

public static class TestStreamReaderEnumerable
{
    public static void Test()
    {
        long memoryBefore = GC.GetTotalMemory(false);
        IEnumerable<string> stringsFound;
        try
        {
            stringsFound =
                from line in new StreamReaderEnumerable(
                    @"C:\Build\Dev\DotNetImplementations\IEnumerableImplementation\TestReadingFile.txt")
                where line.Contains("string to search for")
                select line;
            Console.WriteLine("Found: " + stringsFound.Last());
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine(@"This example requires a file named C:\temp\tempFile.txt.");
            return;
        }
        
        long memoryAfter = GC.GetTotalMemory(false);
        Console.WriteLine("Memory Used With Iterator = \t"
                          + string.Format(((memoryAfter - memoryBefore) / 1000).ToString(), "n") + "kb");
    }
}