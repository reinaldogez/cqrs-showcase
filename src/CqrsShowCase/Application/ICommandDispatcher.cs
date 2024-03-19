using CqrsShowCase.Command.Domain;

namespace CqrsShowCase.Application;

public interface ICommandDispatcher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}