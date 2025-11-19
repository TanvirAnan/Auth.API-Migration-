using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Domain.Entites;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            
            if (await _userRepository.ExistsByUserNameAsync(request.UserName))
            {
                throw new BadRequestException($"Username '{request.UserName}' already exists.");
            }

            var user = new User(
                UserId.New(),
                request.FirstName.Trim(),
                request.LastName.Trim(),
                request.UserName.Trim(),
                new Email(request.Email),
                request.Password 
            );

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User created with id {UserId}", user.Id);

            return new CreateUserResult(user);
        }
    }
}
