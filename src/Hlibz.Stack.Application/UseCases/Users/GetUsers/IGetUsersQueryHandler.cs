using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.GetUsers;

/// <summary>Lists every user.</summary>
public interface IGetUsersQueryHandler : IUseCaseHandler<GetUsersQuery, PagedData<UserDto>>;
