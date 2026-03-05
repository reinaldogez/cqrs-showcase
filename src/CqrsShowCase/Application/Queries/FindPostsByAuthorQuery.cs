using CqrsShowCase.Query.Domain.Entities;
using MediatR;

namespace CqrsShowCase.Application.Queries;

public class FindPostsByAuthorQuery : IRequest<List<PostEntity>>
{
    public string Author { get; set; }
}
