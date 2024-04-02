﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Common.Filters;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Commands.ApproveContributions;
using Server.Application.Features.ContributionApp.Commands.RejectContribution;
using Server.Application.Features.ContributionApp.Commands.UpdateContribution;
using Server.Application.Features.ContributionApp.Queries.GetActivityLog;
using Server.Application.Features.ContributionApp.Queries.GetAllContributionsPaging;
using Server.Application.Features.ContributionApp.Queries.GetRejectReason;
using Server.Contracts.Contributions;
using Server.Domain.Common.Constants;

namespace Server.Api.Controllers.CoordinatorApi
{
    public class ContributionsController : CoordinatorApiController
    {
        private readonly IMapper _mapper;
        public ContributionsController(ISender _mediator, IMapper mapper) : base(_mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Permissions.Contributions.View)]
        public async Task<IActionResult> GetFacultyContribution([FromQuery] GetFacultyContributionRequest request)
        {
            var query = _mapper.Map<GetAllContributionsPagingQuery>(request);
            query.FacultyName = User.GetFacultyName();
            var result = await _mediatorSender.Send(query);
            return result.Match(result => Ok(result), errors => Problem(errors));
        }

        [HttpPost("approve")]
        [Authorize(Permissions.Contributions.Approve)]
        public async Task<IActionResult> ApproveContribution(ApproveContributionsRequest request)
        {
            var command = _mapper.Map<ApproveContributionsCommand>(request);
            command.UserId = User.GetUserId();
            var result = await _mediatorSender.Send(command);
            return result.Match(result => Ok(result), errors => Problem(errors));
        }

        [HttpPost("reject")]
        [Authorize(Permissions.Contributions.Approve)]
        public async Task<IActionResult> RejectContribution(RejectContributionRequest request)
        {
            var command = _mapper.Map<RejectContributionCommand>(request);
            command.UserId = User.GetUserId();
            var result = await _mediatorSender.Send(command);
            return result.Match(result => Ok(result), errors => Problem(errors));
        }

        [HttpGet("reject-reason/{Id}")]
        [Authorize(Permissions.Contributions.Approve)]
        public async Task<IActionResult> GetRejectReason([FromRoute] GetRejectReasonRequest request)
        {
            var query = _mapper.Map<GetRejectReasonQuery>(request);
            var result = await _mediatorSender.Send(query);
            return result.Match(result => Ok(result), errors => Problem(errors));

        }
        [HttpGet("activity-logs/{ContributionId}")]
        [Authorize(Permissions.Contributions.Approve)]
        public async Task<IActionResult> GetActivityLogs([FromRoute] GetActivityLogRequest request)
        {
            var query = _mapper.Map<GetActivityLogQuery>(request);
            var result = await _mediatorSender.Send(query);
            return result.Match(result => Ok(result), errors => Problem(errors));

        }
        //[HttpPut]
        //[FileValidationFilter(5 * 1024 * 1024)]
        //[Authorize(Permissions.Contributions.Edit)]
        //public async Task<IActionResult> UpdateContribution([FromForm] UpdateContributionRequest updateContributionRequest)
        //{
        //    var command = _mapper.Map<UpdateContributionCommand>(updateContributionRequest);
        //    command.UserId = User.GetUserId();
        //    command.FacultyId = User.GetFacultyId();
        //    command.Slug = updateContributionRequest.Title.Slugify();
        //    var result = await _mediatorSender.Send(command);
        //    return result.Match(result => Ok(result), errors => Problem(errors));
        //}
    }
}