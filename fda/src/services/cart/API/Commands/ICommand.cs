namespace Cart.API.Commands
{
    /// <summary>
    /// Base interface for all commands
    /// </summary>
    public interface ICommand<TResult>
    {
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Base interface for commands without return value
    /// </summary>
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}
