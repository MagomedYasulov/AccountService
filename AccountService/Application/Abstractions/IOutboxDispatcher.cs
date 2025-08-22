namespace AccountService.Application.Abstractions;

public interface IOutboxDispatcher
{
    public Task Dispatch();
}