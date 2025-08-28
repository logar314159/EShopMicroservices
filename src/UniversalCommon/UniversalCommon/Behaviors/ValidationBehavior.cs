using FluentValidation;
using MediatR;
using UniversalCommon.CQRS;

namespace UniversalCommon.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validator)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationContext = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validator.Select(v => { return v.ValidateAsync(validationContext, cancellationToken); })
            );

            var failures = validationResults
                .Where(v => v.Errors.Any())
                .SelectMany(v => v.Errors).ToList();

            if (failures.Any()) {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
