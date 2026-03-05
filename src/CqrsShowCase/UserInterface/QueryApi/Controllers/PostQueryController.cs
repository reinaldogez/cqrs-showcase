using CqrsShowCase.Application.Queries;
using CqrsShowCase.Query.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CqrsShowCase.Core.DTOs;
using CqrsShowCase.UserInterface.QueryApi.DTOs;

namespace CqrsShowCase.UserInterface.QueryApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostQueryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostQueryController> _logger;

    public PostQueryController(IMediator mediator, ILogger<PostQueryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _mediator.Send(new FindAllPostsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<IActionResult> GetPostByIdAsync(Guid postId)
    {
        try
        {
            var posts = await _mediator.Send(new FindPostByIdQuery { Id = postId });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while retrieving post by id!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<IActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _mediator.Send(new FindPostsByAuthorQuery { Author = author });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while retrieving posts by author!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withComments")]
    public async Task<IActionResult> GetPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _mediator.Send(new FindPostsWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while retrieving posts with comments!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<IActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _mediator.Send(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while retrieving posts with likes!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        if (posts == null || !posts.Any())
            return NoContent();

        var count = posts.Count;
        return Ok(new PostQueryResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
        });
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
        _logger.LogError(ex, safeErrorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        });
    }
}
