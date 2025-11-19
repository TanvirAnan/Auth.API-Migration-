using BuildingBlocks.CQRS;
using Application.Constants;
using Domain.Entites;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands;

public record CreateUserCommand(string FirstName,
 string LastName,
 string UserName,
 string Email,
 string Password) : ICommand<CreateUserResult>;

public record CreateUserResult(User User);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
 public CreateUserCommandValidator()
 {
 RuleFor(v => v.FirstName)
 .NotEmpty()
 .MaximumLength(UserConsts.NameMaxLength);

 RuleFor(v => v.LastName)
 .NotEmpty()
 .MaximumLength(UserConsts.NameMaxLength);

 RuleFor(v => v.UserName)
 .NotEmpty()
 .MinimumLength(UserConsts.UsernameMinLength)
 .MaximumLength(UserConsts.UsernameMaxLength);

 RuleFor(v => v.Email)
 .NotEmpty()
 .MaximumLength(UserConsts.EmailMaxLength)
 .EmailAddress();

 RuleFor(v => v.Password)
 .NotEmpty()
 .MinimumLength(UserConsts.PasswordMinLength)
 .MaximumLength(UserConsts.PasswordMaxLength);
 }
}