using EasyPay.Common.Errors;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => ToCamelCase(failureGroup.Key), failureGroup => failureGroup.ToArray());

                var validationError = new ValidationError(errors);
                return CreateFailureResponse(validationError);
            }

            return await next();
        }

        private static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
            {
                return str;
            }
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        private static TResponse CreateFailureResponse(Error error)
        {
            var responseType = typeof(TResponse);
            var failureMethod = responseType.GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                new[] { typeof(Error) });

            if (failureMethod == null)
            {
                throw new InvalidOperationException($"Could not find a static 'Failure' method on type {responseType.Name} that accepts an Error object.");
            }

            return (TResponse)failureMethod.Invoke(null, new object[] { error });
        }
    }
}