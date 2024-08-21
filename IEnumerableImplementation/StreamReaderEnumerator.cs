using System.Collections;

namespace IEnumerableImplementation;

public class StreamReaderEnumerator : IEnumerator<string>
{
    private readonly StreamReader _sr;

    public StreamReaderEnumerator(string filePath)
    {
        _sr = new StreamReader(filePath);
    }

    private string _current;

    public string Current
    {
        get
        {
            if (_sr == null || _current == null)
            {
                throw new InvalidOperationException();
            }

            return _current;
        }
    }

    private object Current1 => this.Current;

    object IEnumerator.Current => this.Current1;

    public bool MoveNext()
    {
        _current = _sr.ReadLine();
        if (_current == null)
            return false;
        return true;
    }

    public void Reset()
    {
        _sr.DiscardBufferedData();
        _sr.BaseStream.Seek(0, SeekOrigin.Begin);
        _current = null;
    }
    
    private bool disposedValue = false;
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                //Dispose of managed resourcces
            }
            
            _current = null;
            if (_sr != null)
            {
                _sr.Close();
                _sr.Dispose();
            }
        }
        
        disposedValue = true;
    }

    ~StreamReaderEnumerator()
    {
        Dispose(disposing: false);
    }
}