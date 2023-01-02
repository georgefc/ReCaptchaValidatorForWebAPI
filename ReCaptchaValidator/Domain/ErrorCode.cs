using System.Runtime.Serialization;

namespace ReCaptchaValidator.Domain
{
    internal enum ErrorCode
    {
        [EnumMember(Value = "missing-input-secret")]
        MissingInputSecret,
        [EnumMember(Value = "invalid-input-secret")]
        InvalidInputSecret,
        [EnumMember(Value = "missing-input-response")]
        MissingInputResponse,
        [EnumMember(Value = "invalid-input-response")]
        InvalidInputResponse,
        [EnumMember(Value = "bad-request")]
        BadRequest,
        [EnumMember(Value = "timeout-or-duplicate")]
        TimeoutOrDuplicate
    }
}
