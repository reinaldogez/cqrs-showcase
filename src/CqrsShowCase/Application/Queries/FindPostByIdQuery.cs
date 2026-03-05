using CqrsShowCase.Query.Domain.Entities;
using MediatR;

namespace CqrsShowCase.Application.Queries;

public class FindPostByIdQuery : IRequest<List<PostEntity>>
{
    public Guid Id { get; set; }
}
