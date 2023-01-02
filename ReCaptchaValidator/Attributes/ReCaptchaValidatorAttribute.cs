using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReCaptchaValidator.Domain;
using ReCaptchaValidator.Exceptions;
using System.Net;

namespace ReCaptchaValidator.Attributes
{
    /// <summary>
    /// Indicates that the decorated class or method will be validated by ReCaptchaValidator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ReCaptchaValidatorAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The expected header in a request
        /// </summary>
        private readonly string _headerKey = "x-token-recaptcha";

        /// <summary>
        /// ReCaptcha validator handler.
        /// </summary>
        /// <param name="context">An object of type <see cref="ActionExecutingContext"/> that contains the request context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Validate(context);
        }

        /// <summary>
        /// Validate if the ReCaptcha token is valid.
        /// </summary>
        /// <param name="context">An object of type <see cref="ActionExecutingContext"/> that contains the request context.</param>
        private void Validate(ActionExecutingContext context)
        {
            ReCaptchaSettings settings = GetReCaptchaSettings(context.HttpContext.RequestServices);

            if (!settings.IsActive) { return; }

            Dictionary<string, string> content = PrepareContent(context.HttpContext.Request.Headers, settings.SecretKey);

            HttpResponseMessage response = SendRequest(content, settings.ApiURL);

            ReCaptchaResponse result = GetReCaptchaResponse(response);

            ValidateScore(result, settings);
        }

        /// <summary>
        /// Get the ReCaptcha settings.
        /// </summary>
        /// <param name="services">An object that contains the application services.</param>
        /// <returns>An object type <see cref="ReCaptchaSettings"/> that containing the ReCaptcha settings.</returns>
        private ReCaptchaSettings GetReCaptchaSettings(IServiceProvider services)
        {
            IOptions<ReCaptchaSettings> option = services.GetService<IOptions<ReCaptchaSettings>>();

            ValidateSettings(option);

            return option.Value;
        }

        /// <summary>
        /// Prepare body content to make a request.
        /// </summary>
        /// <param name="headers">An object that contains the request headers.</param>
        /// <param name="secretKey">A string that the api url.</param>
        /// <returns>An object of type <see cref="Dictionary{string, string}"/> that contains the content to be requested.</returns>
        private Dictionary<string, string> PrepareContent(IHeaderDictionary headers, string secretKey)
        {
            ValidateHeader(headers);

            Dictionary<string, string> content = new()
            {
                { "secret", secretKey },
                { "response", headers[_headerKey] }
            };

            return content;
        }

        /// <summary>
        /// Send a request to validate the ReCaptcha token.
        /// </summary>
        /// <param name="content">An object that contains the content to be requested.</param>
        /// <param name="apiURL">A string tha contais the api url.</param>
        /// <returns>An object of type<see cref="HttpResponseMessage"/> that contains the API response.</returns>
        private HttpResponseMessage SendRequest(Dictionary<string, string> content, string apiURL)
        {
            using FormUrlEncodedContent contentBody = new(content);
            using HttpRequestMessage request = new();
            using HttpClient httpClient = new();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(apiURL);
            request.Content = contentBody;

            return httpClient.Send(request);
        }

        /// <summary>
        /// Get ReCaptcha response.
        /// </summary>
        /// <param name="response"> object of type <see cref="HttpResponseMessage"/> that contains the API response.</param>
        /// <returns>A object of type <see cref="ReCaptchaResponse"/> that contains ReCaptcha response.</returns>
        /// <exception cref="ReCaptchaValidatorException"></exception>
        private ReCaptchaResponse GetReCaptchaResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ReCaptchaValidatorException("Unable to validate ReCaptcha.");
            }

            string responseContent = response.Content.ReadAsStringAsync().Result;

            ReCaptchaResponse result = JsonConvert.DeserializeObject<ReCaptchaResponse>(responseContent, new StringEnumConverter());

            if (!result.Success)
            {
                string erroMessage = GetErrorMessage(result.ErrorCodes.First());

                throw new ReCaptchaValidatorException(erroMessage);
            }

            return result;
        }

        /// <summary>
        /// Get the error message by error code.
        /// </summary>
        /// <param name="error">An object of type <see cref="ErrorCode"/> that contains the erro code.</param>
        /// <returns>A string that containing the error message.</returns>
        private string GetErrorMessage(ErrorCode error)
        {
            return error switch
            {
                ErrorCode.MissingInputSecret => "The secret parameter is missing.",
                ErrorCode.InvalidInputSecret => "The secret parameter is invalid or malformed.",
                ErrorCode.MissingInputResponse => "The response parameter is missing.",
                ErrorCode.InvalidInputResponse => "The response parameter is invalid or malformed.",
                ErrorCode.BadRequest => "The request is invalid or malformed.",
                ErrorCode.TimeoutOrDuplicate => "The response is no longer valid: either is too old or has been used previously.",
                _ => "Unable to validate ReCaptcha."
            };
        }

        /// <summary>
        /// Validates if header with ReCaptcha token was provided.
        /// </summary>
        /// <param name="headers">An object that contains the request headers.</param>
        /// <exception cref="ReCaptchaValidatorException">There is no expected header.</exception>
        private void ValidateHeader(IHeaderDictionary headers)
        {
            if (!headers.Any(header => header.Key.ToUpperInvariant().Equals(_headerKey, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ReCaptchaValidatorException("ReCaptcha token not provided.");
            }
        }

        /// <summary>
        /// Validates that header with ReCaptcha settings is provided.
        /// </summary>
        /// <param name="option">An object that contains the ReCaptcha settings.</param>
        /// <exception cref="ReCaptchaValidatorException">There is no ReCaptcha settings.</exception>
        private void ValidateSettings(IOptions<ReCaptchaSettings> option)
        {
            if (option == null || option.Value == null)
            {
                throw new ReCaptchaValidatorException("ReCaptcha settings not provided.");
            }
        }

        /// <summary>
        /// Validates if the minimun score has been reached.
        /// </summary>
        /// <param name="result">An object that contaisn the ReCaptch result.</param>
        /// <param name="settings">An object that contains the ReCaptcha settings.</param>
        /// <exception cref="ReCaptchaValidatorException">Minimun score not reached.</exception>
        private void ValidateScore(ReCaptchaResponse result, ReCaptchaSettings settings)
        {
            if (!settings.IsUseScore)
            {
                return;
            }

            if (result.Score < settings.MinimumScore)
            {
                throw new ReCaptchaValidatorException("Minimun score not reached.");
            }
        }
    }
}
