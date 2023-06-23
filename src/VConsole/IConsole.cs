namespace VConsole;
public interface IConsole
{
    void WriteMessage(string message);
    void WriteLine(string line);
    void WriteError(string message);
    string ReadValue(string message);
}
