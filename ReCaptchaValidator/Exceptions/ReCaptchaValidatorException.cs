namespace ReCaptchaValidator.Exceptions
{
    /// <summary>
    /// The ReCaptchaValidatorException is thrown when an error occurs in the ReCaptchaValidator class.
    /// </summary>
    [Serializable]
    public class ReCaptchaValidatorException : Exception
    {
        public ReCaptchaValidatorException(string message) : base(message) { }

        public ReCaptchaValidatorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
