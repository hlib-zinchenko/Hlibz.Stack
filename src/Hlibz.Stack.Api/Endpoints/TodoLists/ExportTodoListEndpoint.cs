using System.Text;

using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Exports a todo list as a Markdown checklist.</summary>
internal sealed class ExportTodoListEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    // A file, not the use case's value: the handler sends the result itself, so the use case's
    // errors are declared with ProducesErrorSet, and documented as GetTodoListEndpoint's are.
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet("{id}/export", HandleAsync)
            .WithSummary("Exports a todo list as a Markdown checklist.")
            .ProducesFile("text/markdown")
            .ProducesErrorSet<GetTodoListError>();

    private static async Task<IResult> HandleAsync(
        TodoListId id,
        [FromServices] IGetTodoListQueryHandler handler,
        CancellationToken cancellationToken)
    {
        Result<TodoListDetailDto, GetTodoListError> list =
            await handler.HandleAsync(new GetTodoListQuery(id), cancellationToken);
        return list.IsSuccess
            ? TypedResults.Text(Markdown(list.Value), "text/markdown", Encoding.UTF8)
            : list.ToHttpResult();
    }

    private static string Markdown(TodoListDetailDto list)
    {
        StringBuilder markdown = new StringBuilder("# ").Append(list.Title).Append('\n');
        if (list.Items.Count > 0)
        {
            markdown.Append('\n');
        }

        foreach (TodoItemDto item in list.Items)
        {
            markdown.Append(item.IsDone ? "- [x] " : "- [ ] ").Append(item.Title).Append('\n');
        }

        return markdown.ToString();
    }
}
