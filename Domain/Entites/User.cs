using Domain.Common;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entites
{
    public class User : BaseEntity<UserId>
    {
        // Parameterless constructor required by EF Core (calls base with default id; will be overridden when seeding or saving)
        public User() : base(default) { }

        // Explicit constructor to enforce setting required properties
        public User(UserId id, string firstName, string lastName, string userName, Email email, string password) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            Password = password;
        }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public Email Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
