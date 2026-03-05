using CqrsShowCase.Query.Domain.Entities;
using MediatR;

namespace CqrsShowCase.Application.Queries;

public class FindPostsWithLikesQuery : IRequest<List<PostEntity>>
{
    public int NumberOfLikes { get; set; }
}
