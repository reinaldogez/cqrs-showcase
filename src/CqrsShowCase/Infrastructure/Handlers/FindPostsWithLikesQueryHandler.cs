using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using MediatR;

namespace CqrsShowCase.Infrastructure.Handlers;

public class FindPostsWithLikesQueryHandler : IRequestHandler<FindPostsWithLikesQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostsWithLikesQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostsWithLikesQuery request, CancellationToken cancellationToken)
        => await _postRepository.ListWithLikesAsync(request.NumberOfLikes);
}
