using System.Net;
using System.Web;

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
            catch (HttpException ex) when (ex.ErrorCode == (int)HttpStatusCode.InternalServerError)
            {
                // Handle 500 - Internal Server Error
            }
            catch (HttpException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
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
