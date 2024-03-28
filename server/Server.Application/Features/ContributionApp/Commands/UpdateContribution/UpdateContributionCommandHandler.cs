﻿using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrappers;
using Server.Contracts.Contributions;
using Server.Domain.Common.Constants;
using Server.Domain.Common.Errors;
using File = Server.Domain.Entity.Content.File;


namespace Server.Application.Features.ContributionApp.Commands.UpdateContribution
{
    public class UpdateContributionCommandHandler : IRequestHandler<UpdateContributionCommand,ErrorOr<IResponseWrapper>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMediaService _mediaService;
        public UpdateContributionCommandHandler(IUnitOfWork unitOfWork,IMapper mapper, IDateTimeProvider dateTimeProvider, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _mediaService = mediaService;
        }

        public async Task<ErrorOr<IResponseWrapper>> Handle(UpdateContributionCommand request, CancellationToken cancellationToken)
        {
            var itemFromDb  =  await _unitOfWork.ContributionRepository.GetByIdAsync(request.ContributionId);
            if(itemFromDb is null)
            {
                return Errors.Contribution.NotFound;
                
            }

            if (!itemFromDb.IsConfirmed)
            {
                return Errors.Contribution.NotConfirmed;
            }
            if (itemFromDb.DateDeleted.HasValue)
            {
                return Errors.Contribution.Deleted;
            }
            _mapper.Map(request,itemFromDb);
            itemFromDb.DateEdited = _dateTimeProvider.UtcNow;
            await _unitOfWork.CompleteAsync();
            if (request.Thumbnail is not null || request.Files.Count > 0)
            {
                // remove old files
                var existingFiles = await _unitOfWork.FileRepository.GetByContribution(itemFromDb);
                if (existingFiles.Count == 0)
                {
                    return Errors.Contribution.NoFilesFound;
                }
                var removeFilePaths = new List<string>();
                foreach (var existingFile in existingFiles)
                {
                    _unitOfWork.FileRepository.Remove(existingFile);
                    removeFilePaths.Add(existingFile.Path);

                }

                await _mediaService.RemoveFile(removeFilePaths);

                // create new files
                var thumbnailList = new List<IFormFile>
                {
                    request.Thumbnail
                };

                var thumbnailInfo = await _mediaService.UploadFiles(thumbnailList, FileType.Thumbnail);
                var fileInfo = await _mediaService.UploadFiles(request.Files, FileType.File);

                foreach (var info in fileInfo.Concat(thumbnailInfo))
                {
                    _unitOfWork.FileRepository.Add(new File
                    {
                        ContributionId = itemFromDb.Id,
                        Path = info.Path,
                        Type = info.Type,
                        Name = info.Name,
                        DateEdited = _dateTimeProvider.UtcNow
                    });
                }
                await _unitOfWork.CompleteAsync();
            }
            // add email service later


            return new ResponseWrapper
            {
                IsSuccessfull = true,
                Messages = new List<string>
                {
                    "Update contribution successfully!"
                }
            };
            
        }
    }
}
