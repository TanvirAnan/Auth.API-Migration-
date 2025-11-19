using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Identify user by username+password
            var existing = await _userRepository.GetAsync(request.UserName, request.Password);
            if (existing is null)
            {
                throw new NotFoundException("User", request.UserName);
            }

            existing.FirstName = request.FirstName.Trim();
            existing.LastName = request.LastName.Trim();
            existing.Email = new Email(request.Email);

            _userRepository.Update(existing);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} updated", existing.Id);

            return new UpdateUserResult(existing.Id);
        }
    }
}
