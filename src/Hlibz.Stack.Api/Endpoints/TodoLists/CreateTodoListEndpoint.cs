using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Creates an empty todo list for the signed-in user.</summary>
internal sealed class CreateTodoListEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPost(string.Empty, HandleAsync).WithSummary("Creates a todo list.");

    private static Task<Result<TodoListDetailDto, CreateTodoListError>> HandleAsync(
        CreateTodoListCommand command,
        [FromServices] ICreateTodoListCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(command, cancellationToken);
}
