using Todos.Core.DataAccess;
using Todos.Core.Dtos;
using Todos.Core.Entities;
using Todos.Core.Exceptions;

namespace Todos.Core.UseCases.Todos;

public class FindTodoByIdUseCase : IFindTodoByIdUseCase
{
    private readonly IUserDataAccess userDataAccess;
    private readonly ITodoDataAccess todoDataAccess;

    public FindTodoByIdUseCase(IUserDataAccess userDataAccess, ITodoDataAccess todoDataAccess)
    {
        this.userDataAccess = userDataAccess;
        this.todoDataAccess = todoDataAccess;
    }

    public TodoDto Execute(string? todoId, string? authUserId)
    {
        var validTodoId = this.ValidateTodoId(todoId);
        var validUserId = this.ValidateUserId(authUserId);
        this.CheckUserExists(validUserId);
        var todoDb = this.FindTodoById(validTodoId);
        this.CheckResourceOwnership(todoDb, validUserId);
        var todo = this.MapTodoDbToTodo(todoDb);
        return todo;
    }

    private string ValidateTodoId(string? todoId)
    {
        Todo.ValidateId(todoId);
        return todoId!;
    }

    private string ValidateUserId(string? authUserId)
    {
        User.ValidateId(authUserId);
        return authUserId!;
    }

    private void CheckUserExists(string authUserId)
    {
        var user = this.userDataAccess.FindById(authUserId);
        if (user == null) {
            throw new UserNotFoundException();
        }
    }

    private TodoDbDto FindTodoById(string todoId)
    {
        var todo = this.todoDataAccess.FindById(todoId);
        if (todo == null) {
            throw new TodoNotFoundException();
        }
        return todo;
    }

    private void CheckResourceOwnership(TodoDbDto todoDb, string authUserId)
    {
        if (todoDb.UserId != authUserId) {
            throw new NotResourceOwnerException();
        }
    }

    private TodoDto MapTodoDbToTodo(TodoDbDto todoDb) =>
        new TodoDto() {
            Id = todoDb.Id,
            Name = todoDb.Name,
            Description = todoDb.Description,
            IsDone = todoDb.IsDone,
            UserId = todoDb.UserId,
            TaskId = todoDb.TaskId
        };

}
