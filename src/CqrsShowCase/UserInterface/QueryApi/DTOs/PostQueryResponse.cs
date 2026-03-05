using CqrsShowCase.Core.DTOs;
using CqrsShowCase.Query.Domain.Entities;

namespace CqrsShowCase.UserInterface.QueryApi.DTOs;

public class PostQueryResponse : BaseResponse
{
    public List<PostEntity> Posts { get; set; }
}
