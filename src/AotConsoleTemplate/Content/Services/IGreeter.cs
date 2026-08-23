namespace AotConsoleTemplate.Services;

/// <summary>Formats a greeting for a name. A tiny sample service to show DI wiring.</summary>
public interface IGreeter
{
    /// <summary>Returns the configured greeting for <paramref name="name"/>.</summary>
    string Greet(string name);
}
