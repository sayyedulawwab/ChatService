using ChatService.Domain.Rooms;
using ChatService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.EntityConfigurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
              .ValueGeneratedOnAdd();

        builder.Property(user => user.UserId);

        builder.Property(user => user.RoomId);

        builder.Property(user => user.Role);

        builder.Property(user => user.JoinedOn)
               .HasDefaultValueSql("GetUtcDate()");

        builder.HasIndex(member => new { member.UserId, member.RoomId })
               .IsUnique();

        builder.HasOne<User>(member => member.User)
               .WithMany()
               .HasForeignKey(member => member.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Room>()
               .WithMany(room => room.Members)
               .HasForeignKey(member => member.RoomId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}