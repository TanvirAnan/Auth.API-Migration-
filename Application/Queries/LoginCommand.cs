using Application.Constants;
using BuildingBlocks.CQRS;
using Domain.Entites;
using FluentValidation;
using MediatR;


namespace Application.Queries;

public record LoginQuery(string UserName, string Password) : IQuery<LoginResult>;
public record LoginResult(User User, string Token);


public class GetUserQueryValidator : AbstractValidator<LoginQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(v => v.UserName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(UserConsts.UsernameMaxLength)
            .MinimumLength(UserConsts.UsernameMinLength);

        RuleFor(v => v.Password)
            .NotEmpty()
            .NotNull()
            .MaximumLength(UserConsts.PasswordMaxLength)
            .MinimumLength(UserConsts.PasswordMinLength);
    }
}
