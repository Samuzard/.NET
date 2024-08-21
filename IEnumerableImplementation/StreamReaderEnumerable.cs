using System.Collections;

namespace IEnumerableImplementation;

public class StreamReaderEnumerable : IEnumerable<string>
{
    private string _filePath;

    public StreamReaderEnumerable(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return new StreamReaderEnumerator(_filePath);
    }

    private IEnumerator GetEnumerator1()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator1();
    }
}