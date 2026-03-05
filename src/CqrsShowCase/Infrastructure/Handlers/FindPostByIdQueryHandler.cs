using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using MediatR;

namespace CqrsShowCase.Infrastructure.Handlers;

public class FindPostByIdQueryHandler : IRequestHandler<FindPostByIdQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id);
        return post != null ? new List<PostEntity> { post } : new List<PostEntity>();
    }
}
