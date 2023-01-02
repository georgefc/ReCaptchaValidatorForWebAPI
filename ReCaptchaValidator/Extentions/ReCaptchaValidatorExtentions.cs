using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReCaptchaValidator.Domain;

namespace ReCaptchaValidator.Extentions
{
    /// <summary>
    /// Extensions for configuring ReCaptchaValidator.
    /// </summary>
    public static class ReCaptchaValidatorExtentions
    {
        /// <summary>
        /// Configures ReCaptchaValidator.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to retrieve the service object from.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>A service object of type <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddReCaptchaValidator(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ReCaptchaSettings>(config.GetSection("ReCaptchaSettings"));

            return services;
        }
    }
}
