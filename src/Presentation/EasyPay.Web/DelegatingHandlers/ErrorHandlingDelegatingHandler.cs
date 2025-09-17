using EasyPay.Common.Errors;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Web.DelegatingHandlers
{
    public class ErrorHandlingDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Error apiError = null;
                try
                {
                    apiError = await response.Content.ReadFromJsonAsync<Error>(cancellationToken: cancellationToken);
                }
                catch
                {
                    apiError = new Error((int)response.StatusCode, $"HTTP Error: {response.ReasonPhrase}");
                }

                throw new ApiException(apiError ?? new Error((int)response.StatusCode, "An unknown API error occurred."));
            }

            return response;
        }
    }
}