using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrappers;
using Server.Domain.Common.Constants;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using Server.Application.Common.Interfaces.Services;
using Server.Contracts.Common;

namespace Server.Application.Features.Identity.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, ErrorOr<IResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(UserManager<AppUser> userManager,
                                    RoleManager<AppRole> roleManager,
                                    IMapper mapper,
                                    IEmailService emailService,
                                    IUnitOfWork unitOfWork, IMediaService mediaService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
        _mediaService = mediaService;
        _emailService = emailService;
    }

    public async Task<ErrorOr<IResponseWrapper>> Handle(CreateUserCommand request,
                                                    CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Errors.User.DuplicateEmail;
        }

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role is null) 
        {
            return Errors.Roles.NotFound;
        }

        var facultyFromDb =
            await _unitOfWork
            .FacultyRepository
            .GetByIdAsync(request.FacultyId);

        if (facultyFromDb is null)
        {
            return Errors.Faculty.CannotFound;
        }
        var newUser = new AppUser();

        _mapper.Map(request, newUser);
        newUser.Id = Guid.NewGuid();
        newUser.FacultyId = facultyFromDb.Id;
        string randomPassword = RandomString(8);
        newUser.PasswordHash = new PasswordHasher<AppUser>().HashPassword(newUser, randomPassword);

        // avatar
        if (request.Avatar is not null)
        {
            var avatarList = new List<IFormFile>();
            avatarList.Add(request.Avatar);
            var avatarInfo = await _mediaService.UploadFileCloudinary(avatarList, FileType.Avatar,newUser.Id);
            foreach (var info in avatarInfo)
            {
                newUser.Avatar = info.Path;
                newUser.AvatarPublicId = info.PublicId;
            }
        }

        var result = await _userManager.CreateAsync(newUser);

        if (!result.Succeeded)
        {
            return result.GetIdentityResultErrorDescriptions();
        }

        result = await _userManager.AddToRoleAsync(newUser, role.Name!);

        if (!result.Succeeded)
        {
            return result.GetIdentityResultErrorDescriptions();
        }

        // send mail
        _emailService.SendEmail(new MailRequest 
        {
            ToEmail = request.Email,
            Subject = "University Provide Password",
            Body = $"Email: <h1>{request.Email}</h1>, Password: <h1>{randomPassword}</h1>"
        });

        return new ResponseWrapper
        {
            IsSuccessfull = true,
            Messages = new List<string>
                {
                    "Create new user successfully!"
                }
        };
    }

    private static Random random = new Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}