using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using MediatR;

namespace CqrsShowCase.Infrastructure.Handlers;

public class FindAllPostsQueryHandler : IRequestHandler<FindAllPostsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindAllPostsQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindAllPostsQuery request, CancellationToken cancellationToken)
        => await _postRepository.ListAllAsync();
}
