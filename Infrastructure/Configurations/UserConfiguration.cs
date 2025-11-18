using Application.Constants;
using Domain.Entites;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(UserConsts.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(UserConsts.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.UserName)
            .HasMaxLength(UserConsts.UsernameMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.UserName)
                    .IsUnique();

        builder.Property(x => x.Email)
            .HasConversion(
            e => e.Value,
            value => new Email(value))
                    .HasMaxLength(UserConsts.EmailMaxLength)
                    .IsRequired();

        builder.HasIndex(x => x.Email)
                    .IsUnique();

        builder.Property(x => x.Password)
            .HasMaxLength(UserConsts.PasswordMaxLength)
            .IsRequired();

  
    }
}