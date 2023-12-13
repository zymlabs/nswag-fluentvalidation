using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// Extensions for some swagger specific work.
    /// </summary>
    internal static class ValidationExtensions
    {
        /// <summary>
        /// Contains <see cref="IValidationRule"/> and additional info.
        /// </summary>
        public readonly struct ValidationRuleContext
        {
            /// <summary>
            /// PropertyRule.
            /// </summary>
            public readonly IValidationRule ValidationRule;

            /// <summary>
            /// Flag indication whether the <see cref="IValidationRule"/> is the CollectionRule.
            /// </summary>
            public readonly bool IsCollectionRule;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationRuleContext"/> struct.
            /// </summary>
            /// <param name="validationRule">PropertyRule.</param>
            /// <param name="isCollectionRule">Is a CollectionPropertyRule.</param>
            public ValidationRuleContext(IValidationRule validationRule, bool isCollectionRule)
            {
                ValidationRule = validationRule;
                IsCollectionRule = isCollectionRule;
            }
        }
        
        /// <summary>
        /// Is supported swagger numeric type.
        /// </summary>
        public static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;

        /// <summary>
        /// Returns not null enumeration.
        /// </summary>
        public static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue>? collection)
        {
            return collection ?? Array.Empty<TValue>();
        }
        
        /// <summary>
        /// Returns validation rules by property name ignoring name case.
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="name">Property name.</param>
        /// <returns>enumeration.</returns>
        public static IEnumerable<ValidationRuleContext> GetValidationRulesForMemberIgnoreCase(this IValidator validator, string name)
        {
            return (validator as IEnumerable<IValidationRule>)
                   .NotNull()
                   .GetPropertyRules()
                   .Where(validationRuleContext => HasNoCondition(validationRuleContext.ValidationRule) && StringExtensions.EqualsIgnoreAll(validationRuleContext.ValidationRule.PropertyName, name));
        }
        
        /// <summary>
        /// Returns property validators by property name ignoring name case.
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="name">Property name.</param>
        /// <returns>enumeration.</returns>
        public static IEnumerable<IPropertyValidator> GetValidatorsForMemberIgnoreCase(this IValidator validator, string name)
        {
            return GetValidationRulesForMemberIgnoreCase(validator, name)
                    .SelectMany(
                        validationRuleContext =>
                        {
                            return validationRuleContext.ValidationRule.Components.Select(c => c.Validator);
                        });
        }
        
        /// <summary>
        /// Returns all IValidationRules that are PropertyRule.
        /// If rule is CollectionPropertyRule then isCollectionRule set to true.
        /// </summary>
        internal static IEnumerable<ValidationRuleContext> GetPropertyRules(
            this IEnumerable<IValidationRule> validationRules)
        {
            foreach (var validationRule in validationRules)
            {
                var isCollectionRule = validationRule.GetType() == typeof(ICollectionRule<,>);
                yield return new ValidationRuleContext(validationRule, isCollectionRule);
            }
        }
        
        // /// <summary>
        // /// Returns a <see cref="bool"/> indicating if the <paramref name="propertyValidator"/> is conditional.
        // /// </summary>
        // internal static bool HasNoCondition(this IPropertyValidator propertyValidator)
        // {
        //     return propertyValidator?.Options?.Condition == null && propertyValidator?.Options?.AsyncCondition == null;
        // }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating if the <paramref name="propertyRule"/> is conditional.
        /// </summary>
        internal static bool HasNoCondition(this IValidationRule propertyRule)
        {
            return !propertyRule.HasCondition && !propertyRule.HasAsyncCondition;
        }
    }
}