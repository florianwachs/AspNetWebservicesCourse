using System.Net;
using Xunit;

namespace Advanced;

public class ExceptionFilters
{
    [Fact]
    public void DoStuffThatThrows()
    {
        try
        {
            // Do Stuff
        }
        catch (HttpResponseException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
        {
            // Handle 500 - Internal Server Error
        }
        catch (HttpResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Handle 404 - Not Found
        }
        catch
        {
            // The Rest
        }
    }
}

public class HttpResponseException : Exception
{
    public HttpResponseException() : base()
    {
    }

    public HttpResponseException(string message) : base(message)
    {
    }

    public HttpResponseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public HttpStatusCode StatusCode { get; set; }
}
