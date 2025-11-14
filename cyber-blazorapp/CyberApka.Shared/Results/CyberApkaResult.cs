namespace CyberApka.Shared.Results;

public class CyberApkaResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static CyberApkaResult<T> Success(T data)
    {
        return new CyberApkaResult<T>
        {
            IsSuccess = true,
            Data = data,
            ErrorMessage = null
        };
    }

    public static CyberApkaResult<T> Failure(string errorMessage)
    {
        return new CyberApkaResult<T>
        {
            IsSuccess = false,
            Data = default,
            ErrorMessage = errorMessage
        };
    }
}
