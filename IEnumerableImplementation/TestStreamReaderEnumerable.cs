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
                // where line.Contains("string to search for")
                select line;

            DetermineForEachBahvior(new StreamReaderEnumerable(
                @"C:\Build\Dev\DotNetImplementations\IEnumerableImplementation\TestReadingFile.txt"));
            
            var a = TestTest(stringsFound);

            foreach (var line in a)
            {
                Console.WriteLine(line);
            }
            
            var b = a.ToList();//Exception
            
            Console.WriteLine("Found: " + b.Last());//Exception?
            
            var numbers = ProduceEvenNumbers(5);
            Console.WriteLine("Caller: about to iterate.");
            foreach (int i in numbers)
            {
                Console.WriteLine($"Caller: {i}");
            }
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

    static void DetermineForEachBahvior(IEnumerable<string> collection)
    {
        foreach (var line in collection)
        {
            Console.WriteLine(line);
        }
    }
    
    static IEnumerable<int> ProduceEvenNumbers(int upto)
    {
        Console.WriteLine("Iterator: start.");
        for (int i = 0; i <= upto; i += 2)
        {
            Console.WriteLine($"Iterator: about to yield {i}");
            yield return i;
            Console.WriteLine($"Iterator: yielded {i}");
        }
        Console.WriteLine("Iterator: end.");
    }
    
    private static IEnumerable<string> TestTest(IEnumerable<string> collections)
    {
        foreach (var c in collections)
        {
            // if (c.Contains(("Line 2")))
            //     yield break;
            
            yield return c;
        }
    }
}