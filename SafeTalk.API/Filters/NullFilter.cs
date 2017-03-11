using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace SafeTalk.API.Filters
{
    // http://stackoverflow.com/questions/12694719/asp-net-web-api-returning-404-for-ienumerablet-get-when-null
    public class NullResponseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var response = actionExecutedContext.Response;

            object responseValue;
            bool hasContent = response.TryGetContentValue(out responseValue);

            if (!hasContent || responseValue == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}