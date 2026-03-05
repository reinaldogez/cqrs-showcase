using CqrsShowCase.Query.Domain.Entities;
using MediatR;

namespace CqrsShowCase.Application.Queries;

public class FindPostsWithCommentsQuery : IRequest<List<PostEntity>>
{
}
