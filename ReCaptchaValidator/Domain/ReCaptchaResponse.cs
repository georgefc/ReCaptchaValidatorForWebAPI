using Newtonsoft.Json;

namespace ReCaptchaValidator.Domain
{
    /// <summary>
    /// API ReCaptcha response.
    /// </summary>
    internal class ReCaptchaResponse
    {
        /// <summary>
        /// Whether this request was a valid reCAPTCHA token for your site.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// The score for this request (0.0 - 1.0).
        /// </summary>
        [JsonProperty("score")]
        public float Score { get; set; }

        /// <summary>
        /// The action name for this request (important to verify).
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// Timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ).
        /// </summary>
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }

        /// <summary>
        /// The package name of the app where the reCAPTCHA was solved.
        /// </summary>
        [JsonProperty("apk_package_name")]
        public string ApkPackageName { get; set; }

        /// <summary>
        /// The hostname of the site where the reCAPTCHA was solved.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// List of error codes returned from the API.
        /// </summary>
        [JsonProperty("error-codes")]
        public IEnumerable<ErrorCode> ErrorCodes { get; set; }
    }
}
