using Domain.Entities;
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
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}