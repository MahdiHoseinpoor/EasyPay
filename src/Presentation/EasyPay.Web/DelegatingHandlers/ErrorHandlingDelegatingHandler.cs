using EasyPay.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Web.DelegatingHandlers
{
    public class ErrorHandlingDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                try
                {
                    var validationProblemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (validationProblemDetails?.Errors.Any() == true)
                    {
                        throw new ValidationApiException((Dictionary<string, string[]>)validationProblemDetails.Errors);
                    }
                }
                catch (JsonException) { /* Not a validation problem, fall through */ }
            }

            Error apiError;
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
    }
}