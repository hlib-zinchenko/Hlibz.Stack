using Hlibz.Stack.Infrastructure.Database;

namespace Hlibz.Stack.Infrastructure.Repositories;

internal sealed class UserRepository(StackDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public void Add(User user) => db.Users.Add(user);
}
