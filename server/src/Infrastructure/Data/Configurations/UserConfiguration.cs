using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>();
    }
}