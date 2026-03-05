using CqrsShowCase.Query.Domain.Entities;
using MediatR;

namespace CqrsShowCase.Application.Queries;

public class FindAllPostsQuery : IRequest<List<PostEntity>>
{
}
