using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Queries.GetTopContributors;
using Server.Application.Features.PublicContributionApp.Commands.CreateFavorite;
using Server.Application.Features.PublicContributionApp.Commands.CreateReadLater;
using Server.Application.Features.PublicContributionApp.Commands.LikeContribution;
using Server.Application.Features.PublicContributionApp.Queries.DownAllFile;
using Server.Application.Features.PublicContributionApp.Queries.DownSingleFile;
using Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionPaging;
using Server.Application.Features.PublicContributionApp.Queries.GetDetailPublicContributionBySlug;
using Server.Application.Features.PublicContributionApp.Queries.GetTop4Contributions;
using Server.Application.Features.PublicContributionApp.Queries.GetTopContribution;
using Server.Contracts.PublicContributions;
using Server.Contracts.PublicContributions.Favorite;
using Server.Contracts.PublicContributions.Like;
using Server.Contracts.PublicContributions.ReadLater;

namespace Server.Api.Controllers.ClientApi;

public class PublicContributionController : ClientApiController
{
    private readonly IMapper _mapper;
    public PublicContributionController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet]
    [Route("paging")]
    public async Task<IActionResult> GetAllPublicContribution([FromQuery] GetAllPublicContributionPagingRequest request)
    {
        var query = _mapper.Map<GetAllPublicContributionPagingQuery>(request);
        var result = await _mediatorSender.Send(query);
        return result.Match(success => Ok(success), errors => Problem(errors));

    }

    [HttpGet]
    [Route("top-4")]
    public async Task<IActionResult> GetTop4Contributions()
    {
        var query = new GetTop4ContributionQuery();
        var result = await _mediatorSender.Send(query);
        return result.Match(success => Ok(success), errors => Problem(errors));
    }

    [HttpPost]
    [Route("toggle-like/{ContributionId}")]
    public async Task<IActionResult> LikeContribution([FromRoute] LikeContributionRequest request)
    {
        var command = _mapper.Map<LikeContributionCommand>(request);
        command.UserId = User.GetUserId();
        var result = await _mediatorSender.Send(command);
        return result.Match(success => Ok(success), errors => Problem(errors));
    }

    [HttpPost]
    [Route("{Slug}")]
    public async Task<IActionResult> GetDetailContribution([FromRoute] GetDetailPublicContributionBySlugRequest request)
    {
        var query = _mapper.Map<GetDetailPublicContributionBySlugQuery>(request);
        var result = await _mediatorSender.Send(query);
        return result.Match(success => Ok(success), errors => Problem(errors));


    }
    [HttpGet("download-files/{ContributionId}")]
    public async Task<IActionResult> DownloadFiles([FromRoute] DownloadAllFileRequest request)
    {
        var query = _mapper.Map<DownloadAllFileQuery>(request);
        var result = await _mediatorSender.Send(query);
        return result.Match(result => Ok(result), errors => Problem(errors));
    }

    [HttpGet("download-file")]
    public async Task<IActionResult> DownloadFile(DownSingleFileRequest request)
    {
        var query = _mapper.Map<DownSingleFileQuery>(request);
        var result = await _mediatorSender.Send(query);
        return result.Match(result => Ok(result), errors => Problem(errors));
    }
    [HttpGet]
    [Route("featured-contribution")]
    public async Task<IActionResult> GetTopContribution()
    {
        var query = new GetFeaturedContributionQuery();
        var result = await _mediatorSender.Send(query);
        return result.Match(success => Ok(success), errors => Problem(errors));
    }

    [HttpGet]
    [Route("top-contributors")]
    public async Task<IActionResult> GetTopContributors()
    {
        var query = new GetTopContributorsQuery();
        var result = await _mediatorSender.Send(query);
        return result.Match(success => Ok(success), errors => Problem(errors));
    }
    //[HttpPost]
    //[Route("view/{ContributionId}")]
    //public async Task<IActionResult> ViewContribution([FromRoute] ViewContributionRequest request)
    //{
    //    var command = _mapper.Map<ViewContributionCommand>(request);
    //    var result = await _mediatorSender.Send(command);
    //    return result.Match(success => Ok(success), errors => Problem(errors));
    //}
    [HttpPost("toggle-read-later/{ContributionId}")]
    [Authorize]
    public async Task<IActionResult> AddReadLater([FromRoute] ReadLaterRequest request)
    {
        var command = _mapper.Map<CreateReadLaterCommand>(request);
        command.UserId = User.GetUserId();
        var result = await _mediatorSender.Send(command);
        return result.Match(result => Ok(result), errors => Problem(errors));
    }

    [HttpPost("toggle-favorite/{ContributionId}")]
    [Authorize]
    public async Task<IActionResult> AddFavorite([FromRoute] FavoriteRequest request)
    {
        var command = _mapper.Map<CreateFavoriteCommand>(request);
        command.UserId = User.GetUserId();
        var result = await _mediatorSender.Send(command);
        return result.Match(result => Ok(result), errors => Problem(errors));
    }
}