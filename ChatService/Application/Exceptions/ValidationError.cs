using ChatService.Domain.Abstractions;

namespace ChatService.Application.Exceptions;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            HttpResponseStatusCodes.BadRequest)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }
}
