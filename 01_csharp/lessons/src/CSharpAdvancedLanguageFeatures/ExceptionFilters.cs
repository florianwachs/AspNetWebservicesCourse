using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CSharpAdvancedLanguageFeatures
{
    public class ExceptionFilters
    {
        public void DoStuffThatThrows()
        {
            try
            {
                // Do Stuff
            }
            catch (HttpResponseException ex) when (ex.Response.StatusCode == HttpStatusCode.InternalServerError)
            {
                // Handle 500 - Internal Server Error
            }
            catch (HttpResponseException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle 404 - Not Found
            }
            catch
            {
                // The Rest
            }
        }
    }
}
