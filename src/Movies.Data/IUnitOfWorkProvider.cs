using System.Data;

namespace Movies.Data;

public interface IUnitOfWorkProvider
{
    IUnitOfWork Create(
        bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task<IUnitOfWork> CreateAsync(
        bool transactional = false, 
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
        CancellationToken cancellationToken = default);
}
