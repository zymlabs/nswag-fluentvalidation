using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// Extensions for some swagger specific work.
    /// </summary>
    internal static class ValidationExtensions
    {
        /// <summary>
        /// Is supported swagger numeric type.
        /// </summary>
        public static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;

        /// <summary>
        /// Returns not null enumeration.
        /// </summary>
        public static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue> collection)
        {
            return collection ?? Array.Empty<TValue>();
        }
        
        /// <summary>
        /// Returns validators by property name ignoring name case.
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="name">Property name.</param>
        /// <returns>enumeration or null.</returns>
        public static IEnumerable<IPropertyValidator> GetValidatorsForMemberIgnoreCase(this IValidator validator, string name)
        {
            return (validator as IEnumerable<IValidationRule>)
                .NotNull()
                .OfType<PropertyRule>()
                .Where(propertyRule => propertyRule.Condition == null && propertyRule.AsyncCondition == null && propertyRule.PropertyName?.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true)
                .SelectMany(propertyRule => propertyRule.Validators);
        }
    }
}