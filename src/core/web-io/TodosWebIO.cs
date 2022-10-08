using Todos.Core.Dtos;
using Todos.Core.Exceptions;
using Todos.Core.UseCases.Todos;

namespace Todos.Core.WebIO; 

public class TodosWebIO
{
    public WebResponseDto Create(ICreateTodoUseCase createTodoUseCase, WebRequestDto request)
    {
        try {
            createTodoUseCase.Execute((CreateTodoDto) request.Body!, request.AuthUserId!);
            return new WebResponseDto() { Message = "Todo Created", Status = 201 };
        } catch (Exception e) {
            if (e is InvalidUserException ||
                e is InvalidTaskException ||
                e is InvalidTodoException ||
                e is UserNotFoundException ||
                e is TaskNotFoundException)
            {
                return new WebResponseDto() { Message = e.Message, Status = 400 };
            }
            throw;
        }
    }


    public WebResponseDto FindByTaskId(IFindTodosByTaskIdUseCase findTodosByTaskIdUseCase,
                                       WebRequestDto request)
    {
        try {
            var todos = findTodosByTaskIdUseCase.Execute(request.Param!, request.AuthUserId!);
            if (todos.Count == 0) {
                return new WebResponseDto() { Message = "", Status = 204 };
            }
            return new WebResponseDto() { Body = todos, Status = 200 };
        } catch (Exception e) {
            if (e is InvalidUserException ||
                e is InvalidTaskException ||
                e is InvalidTodoException ||
                e is UserNotFoundException ||
                e is TaskNotFoundException)
            {
                return new WebResponseDto() { Message = e.Message, Status = 400 };
            }
            throw;
        }
    }
}
