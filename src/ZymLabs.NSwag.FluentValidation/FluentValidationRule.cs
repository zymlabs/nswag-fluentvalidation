using System;
using FluentValidation.Validators;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// FluentValidationRule.
    /// </summary>
    public class FluentValidationRule
    {
        /// <summary>
        /// Rule name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Predicate to match property validator.
        /// </summary>
        public Func<IPropertyValidator, bool> Matches { get; set; } = _ => false;

        /// <summary>
        /// Modify Swagger schema action.
        /// </summary>
        public Action<RuleContext> Apply { get; set; } = _ => { };

        /// <summary>
        /// Creates new instance of <see cref="FluentValidationRule"/>.
        /// </summary>
        /// <param name="name">Rule name.</param>
        public FluentValidationRule(string name)
        {
            Name = name;
        }
    }
}