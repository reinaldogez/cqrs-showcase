using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using MediatR;

namespace CqrsShowCase.Infrastructure.Handlers;

public class FindPostsByAuthorQueryHandler : IRequestHandler<FindPostsByAuthorQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostsByAuthorQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostsByAuthorQuery request, CancellationToken cancellationToken)
        => await _postRepository.ListByAuthorAsync(request.Author);
}
