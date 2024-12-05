using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ChatService.Domain.Rooms;

namespace ChatService.Infrastructure.EntityConfigurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(room => room.Id);

        builder.Property(room => room.Id)
              .ValueGeneratedOnAdd();

        builder.Property(room => room.Name)
               .HasMaxLength(200);

        builder.Property(room => room.PasswordHash);

        builder.Property(category => category.CreatedOn)
               .HasDefaultValueSql("GetUtcDate()");
    }
}