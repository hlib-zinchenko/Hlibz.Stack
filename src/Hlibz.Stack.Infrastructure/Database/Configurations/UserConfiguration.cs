using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hlibz.Stack.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Email).HasMaxLength(User.EmailMaxLength);
        builder.Property(user => user.Name).HasMaxLength(User.NameMaxLength);
        builder.Property(user => user.PasswordHash).HasMaxLength(User.PasswordHashMaxLength);
        builder.Property(user => user.Role).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(user => user.Email).IsUnique();
    }
}
