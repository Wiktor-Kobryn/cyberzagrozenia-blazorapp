namespace CyberApka.Shared.Results;

public class CyberApkaResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
