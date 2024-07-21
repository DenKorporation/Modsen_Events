using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder
            .Property(e => e.Date)
            .IsRequired()
            .HasColumnType("timestamp without time zone");
        
        builder
            .Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasMany(e => e.Users)
            .WithMany(u => u.Events)
            .UsingEntity<EventUser>();
    }
}