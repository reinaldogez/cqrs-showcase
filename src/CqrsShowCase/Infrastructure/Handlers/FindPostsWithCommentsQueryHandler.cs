using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using MediatR;

namespace CqrsShowCase.Infrastructure.Handlers;

public class FindPostsWithCommentsQueryHandler : IRequestHandler<FindPostsWithCommentsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostsWithCommentsQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostsWithCommentsQuery request, CancellationToken cancellationToken)
        => await _postRepository.ListWithCommentsAsync();
}
