using System.Globalization;

namespace VConsole;

public class ParserSettings : IDisposable
{

    private bool disposed;
    private bool caseSensitive;
    private CultureInfo? parsingCulture;
    private const char valueSeparator = '=';
    private char separator;

    public ParserSettings()
    {
        caseSensitive = true;
        parsingCulture = CultureInfo.InvariantCulture;
        separator = valueSeparator;
    }

    public char Separator
    {
        get
        {
            return separator;
        }

        init
        {
            separator = value;
        }
    }
    public CultureInfo ParsingCulture
    {
        get
        {
            return parsingCulture ?? CultureInfo.InvariantCulture;
        }

        init
        {
            parsingCulture = value;
        }
    }

    public bool CaseSensitive
    {
        get
        {
            return caseSensitive;
        }
        init
        {
            caseSensitive = value;
        }
    }

    public bool InteractiveMode { get; init; }
    public StringComparer NameComparer
    {
        get
        {
            return caseSensitive
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase;
        }
    }

    ~ParserSettings()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            parsingCulture = null;
            disposed = true;
        }
    }
}
