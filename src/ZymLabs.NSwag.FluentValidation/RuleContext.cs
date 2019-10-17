using FluentValidation.Validators;
using NJsonSchema.Generation;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// RuleContext.
    /// </summary>
    public class RuleContext
    {
        /// <summary>
        /// SchemaProcessorContext.
        /// </summary>
        public SchemaProcessorContext SchemaProcessorContext { get; }

        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyKey { get; }

        /// <summary>
        /// Property validator.
        /// </summary>
        public IPropertyValidator PropertyValidator { get; }

        /// <summary>
        /// Creates new instance of <see cref="RuleContext"/>.
        /// </summary>
        /// <param name="schemaProcessorContext">SchemaProcessorContext.</param>
        /// <param name="propertyKey">Property name.</param>
        /// <param name="propertyValidator">Property validator.</param>
        public RuleContext(SchemaProcessorContext schemaProcessorContext, string propertyKey, IPropertyValidator propertyValidator)
        {
            SchemaProcessorContext = schemaProcessorContext;
            PropertyKey = propertyKey;
            PropertyValidator = propertyValidator;
        }
    }
}