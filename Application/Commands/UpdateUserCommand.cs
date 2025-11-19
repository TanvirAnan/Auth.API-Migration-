using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Domain.ValueObjects;

namespace Application.Commands;

public record UpdateUserCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password) : ICommand<UpdateUserResult>;

public record UpdateUserResult(UserId Id);
