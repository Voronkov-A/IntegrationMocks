using System.Transactions;
using MediatR;

namespace IntegrationMocks.Sample.Locations.Application.Common;

public class UnitOfWorkPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest, ICommand
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkPipelineBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            },
            TransactionScopeAsyncFlowOption.Enabled);
        var response = await next();
        await _unitOfWork.Commit(cancellationToken);
        transaction.Complete();
        return response;
    }
}
