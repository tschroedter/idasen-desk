using System.Text ;
using Serilog ;
using Serilog.Core ;
using Serilog.Events ;

namespace Idasen.TestLogger;

/// <summary>
///     Test logger that captures log output for verification in unit tests.
///     Provides a real Serilog logger instance that writes to an in-memory string buffer.
/// </summary>
public sealed class InMemoryLogger
    : ILogger,
      IDisposable
{
    private readonly StringBuilder _output;
    private readonly TestLoggerSink _sink;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InMemoryLogger" /> class.
    /// </summary>
    /// <param name="minimumLevel">The minimum log level to capture. Defaults to Verbose.</param>
    public InMemoryLogger(LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        _output = new StringBuilder();
        _sink = new TestLoggerSink(_output);

        Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.Sink(_sink)
                .CreateLogger();
    }

    /// <summary>
    ///     Gets the Serilog logger instance that can be passed to classes under test.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    ///     Gets all captured log output as a single string.
    /// </summary>
    public string Output => _output.ToString();

    /// <summary>
    ///     Gets all captured log lines as an array.
    /// </summary>
    public string[] Lines =>
        Output.Split([Environment.NewLine],
                       StringSplitOptions.RemoveEmptyEntries);

    public void Dispose()
    {
        if (_disposed)
            return;

        (Logger as IDisposable)?.Dispose();

        _disposed = true;
    }

    public void Write(LogEvent logEvent)
    {
        _sink.Emit(logEvent);
    }

    /// <summary>
    ///     Checks if the log output does not contain the specified text.
    /// </summary>
    /// <param name="expectedText">The text to search for.</param>
    /// <returns>True if the log output does not contain the text; otherwise, false.</returns>
    public bool DoesNotContains(string expectedText)
    {
        return ! Output.Contains ( expectedText ,
                                   StringComparison.OrdinalIgnoreCase ) ;
    }

    /// <summary>
    ///     Checks if the log output contains the specified text.
    /// </summary>
    /// <param name="expectedText">The text to search for.</param>
    /// <returns>True if the log output contains the text; otherwise, false.</returns>
    public bool Contains(string expectedText)
    {
        return Output.Contains(expectedText,
                               StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Checks if the log output contains the specified text.
    /// </summary>
    /// <param name="expectedText">The text to search for.</param>
    /// <param name="times">The number of times the text is expected to appear.</param>
    /// <returns>True if the log output contains the text the specified number of times; otherwise, false.</returns>
    public bool Contains(string expectedText, int times)
    {
        var count = Lines.Where ( x => x.Contains ( expectedText ) )
                         .ToArray ( ) ;
        return count.Length == times ;
    }

    /// <summary>
    ///     Checks if the log output contains the specified text.
    /// </summary>
    /// <param name="expectedText">The text to search for.</param>
    /// <param name="level">The log event level to match.</param>
    /// <returns>True if the log output contains the text with the specified level; otherwise, false.</returns>
    public bool Contains(string expectedText, LogEventLevel level)
    {
        var count = Lines.Where(x => x.Contains(expectedText) && x.StartsWith($"[{level}]"))
                         .ToArray();

        return count.Length > 0;
    }

    /// <summary>
    ///     Checks if the log output contains the specified text with case-sensitive matching.
    /// </summary>
    /// <param name="expectedText">The text to search for.</param>
    /// <returns>True if the log output contains the text; otherwise, false.</returns>
    public bool ContainsExact(string expectedText)
    {
        return Output.Contains(expectedText,
                                 StringComparison.Ordinal);
    }

    /// <summary>
    ///     Checks if any log line matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test each line against.</param>
    /// <returns>True if any line matches; otherwise, false.</returns>
    public bool AnyLine(Func<string, bool> predicate)
    {
        return Lines.Any(predicate);
    }

    /// <summary>
    ///     Gets the count of log lines that contain the specified text.
    /// </summary>
    /// <param name="text">The text to search for.</param>
    /// <returns>The number of lines containing the text.</returns>
    public int CountLinesContaining(string text)
    {
        return Lines.Count(line => line.Contains(text,
                                                     StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Clears all captured log output.
    /// </summary>
    public void Clear()
    {
        _output.Clear();
    }

    /// <summary>
    ///     Returns the captured log output as a string for debugging purposes.
    /// </summary>
    public override string ToString()
    {
        return Output;
    }

    public bool ContainsLevel ( LogEventLevel level )
    {
        var levelText = $"[{level}]";

        return Lines.Any ( x => x.StartsWith ( levelText ) ) ;
    }
}

/// <summary>
///     Internal sink for writing log events to a StringBuilder.
/// </summary>
internal sealed class TestLoggerSink : ILogEventSink
{
    private readonly StringBuilder _output;

    public TestLoggerSink(StringBuilder output)
    {
        ArgumentNullException.ThrowIfNull(output);

        _output = output;
    }

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        // Simple format: [Level] Message
        _ = _output.Append('[')
                   .Append(logEvent.Level)
                   .Append("] ")
                   .Append(logEvent.RenderMessage());

        if (logEvent.Exception != null)
            _ = _output.Append(" - ")
                       .Append(logEvent.Exception.Message);

        _ = _output.AppendLine();
    }
}
