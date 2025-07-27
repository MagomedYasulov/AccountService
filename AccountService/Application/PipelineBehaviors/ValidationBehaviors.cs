using FluentValidation;
using MediatR;

namespace AccountService.Application.PipelineBehaviors;

public class ValidationBehaviors<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(validator => validator.Validate(context))
            .SelectMany(vr => vr.Errors)
            .Where(e => e != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return next(cancellationToken);
    }
}