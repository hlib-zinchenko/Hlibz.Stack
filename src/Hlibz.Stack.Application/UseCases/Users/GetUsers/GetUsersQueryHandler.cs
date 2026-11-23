using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.ReadRepositories;

namespace Hlibz.Stack.Application.UseCases.Users.GetUsers;

internal sealed class GetUsersQueryHandler(IUserReadRepository users) : IGetUsersQueryHandler
{
    public async Task<Result<PagedData<UserDto>>> HandleAsync(
        GetUsersQuery useCase,
        CancellationToken cancellationToken = default) =>
        Result<PagedData<UserDto>>.Success(await users.GetPageAsync(
            useCase.Search,
            SortableDto.For<UserDto>().Parse(useCase.Sort),
            new PagingParams(useCase.Page, useCase.PerPage),
            cancellationToken));
}
