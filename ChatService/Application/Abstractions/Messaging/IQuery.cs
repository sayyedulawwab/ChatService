using ChatService.Domain.Abstractions;
using MediatR;

namespace ChatService.Application.Abstractions.Messaging;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}