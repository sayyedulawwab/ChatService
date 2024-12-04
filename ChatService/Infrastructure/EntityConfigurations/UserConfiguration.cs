using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ChatService.Domain.Users;

namespace ChatService.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
              .ValueGeneratedOnAdd();

        builder.Property(user => user.Name)
               .HasMaxLength(200);

        builder.Property(user => user.Email)
               .HasMaxLength(200);

        builder.Property(user => user.PasswordHash);

        builder.Property(user => user.CreatedOn)
               .HasDefaultValueSql("GetUtcDate()");
    }
}