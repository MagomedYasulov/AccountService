namespace AccountService.Exceptions;

public class ServiceException(string title, string detail, int statusCode) : Exception(detail)
{
    public int StatusCode { get; set; } = statusCode;
    public string Title { get; set; } = title;
}