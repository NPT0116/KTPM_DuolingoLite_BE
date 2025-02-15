using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.User;
using SharedKernel;
using System;

namespace Application.Features.User.Commands.Register;

public class UserRegisterCommandHandler : ICommandHandler<UserRegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;

    public UserRegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<Guid>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.UseEmailExistsAsync(request.UserRegisterDto.Email))
        {
            return Result.Failure<Guid>(UserError.EmailNotUnique) as Result<Guid>;
        }
        if (await _identityService.UserNameExistsAsync(request.UserRegisterDto.UserName))
        {
            return Result.Failure<Guid>(UserError.UserNameNotUnique);
        }
        var result = await _identityService.CreateUserAsync(
            request.UserRegisterDto.FirstName, 
            request.UserRegisterDto.LastName, 
            request.UserRegisterDto.Email, 
            request.UserRegisterDto.UserName, 
            request.UserRegisterDto.Password);

        if (result.Result.IsFailure)
        {
            return Result.Failure<Guid>(result.Result.Error);
        }

        return result.UserId;
    }
}
