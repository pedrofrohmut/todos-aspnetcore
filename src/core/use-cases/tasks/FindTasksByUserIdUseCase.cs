using Todos.Core.DataAccess;
using Todos.Core.Dtos;
using Todos.Core.Entities;
using Todos.Core.Exceptions;

namespace Todos.Core.UseCases.Tasks;

public class FindTasksByUserIdUseCase : IFindTasksByUserIdUseCase
{
    private readonly IUserDataAccess userDataAccess;
    private readonly ITaskDataAccess taskDataAccess;

    public FindTasksByUserIdUseCase(IUserDataAccess userDataAccess, ITaskDataAccess taskDataAccess)
    {
        this.userDataAccess = userDataAccess;
        this.taskDataAccess = taskDataAccess;
    }

    public List<TaskDto> Execute(string authUserId)
    {
        this.ValidateUserId(authUserId);
        this.CheckUserExists(authUserId);
        var tasksDb = this.FindTasksByUserId(authUserId);
        var tasks = this.MapTasksDbToTasks(tasksDb);
        return tasks;
    }

    private void ValidateUserId(string authUserId)
    {
        User.ValidateId(authUserId);
    }

    private void CheckUserExists(string authUserId)
    {
        var user = this.userDataAccess.FindUserById(authUserId);
        if (user == null) {
            throw new UserNotFoundException();
        }
    }

    private List<TaskDbDto> FindTasksByUserId(string authUserId) =>
        this.taskDataAccess.FindByUserId(authUserId).ToList();

    private List<TaskDto> MapTasksDbToTasks(List<TaskDbDto> tasksDb) =>
        tasksDb
            .Select(task => new TaskDto() {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                UserId = task.UserId
            })
            .ToList();
}