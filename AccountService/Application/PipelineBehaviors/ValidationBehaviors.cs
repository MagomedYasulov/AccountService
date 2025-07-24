using FluentValidation;
using MediatR;

namespace AccountService.Application.PipelineBehaviors
{
    public class ValidationBehaviors<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviors(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(validator => validator.Validate(context))
                .SelectMany(vr => vr.Errors)
                .Where(e => e != null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);

            return next(cancellationToken);
        }
    }
}
