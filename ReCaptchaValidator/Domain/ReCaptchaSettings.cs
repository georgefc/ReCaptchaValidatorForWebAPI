namespace ReCaptchaValidator.Domain
{
    /// <summary>
    /// ReCaptcha settings
    /// </summary>
    public class ReCaptchaSettings
    {
        public string ApiURL { get; set; }

        public bool IsActive { get; set; }

        public bool IsUseScore { get; set; }

        public float MinimumScore { get; set; }

        public string SecretKey { get; set; }
    }
}
