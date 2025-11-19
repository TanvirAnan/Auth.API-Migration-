using BuildingBlocks.Exceptions;

namespace Application.Exceptions;



public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid Id) : base("Product", Id)
    {
    }
}
