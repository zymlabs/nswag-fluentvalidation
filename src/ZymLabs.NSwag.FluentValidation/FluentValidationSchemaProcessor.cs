using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NJsonSchema;
using NJsonSchema.Generation;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// Swagger <see cref="ISchemaProcessor"/> that uses FluentValidation validators instead System.ComponentModel based attributes.
    /// </summary>
    public class FluentValidationSchemaProcessor : ISchemaProcessor
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly ILogger _logger;
        private readonly IReadOnlyList<FluentValidationRule> _rules;
        
        /// <summary>
        /// Creates new instance of <see cref="FluentValidationSchemaProcessor"/>
        /// </summary>
        /// <param name="validatorFactory">Validator factory.</param>
        /// <param name="rules">External FluentValidation rules. Rule with the same name replaces default rule.</param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/> for logging. Can be null.</param>
        public FluentValidationSchemaProcessor(IValidatorFactory validatorFactory, IEnumerable<FluentValidationRule> rules = null, ILoggerFactory loggerFactory = null)
        {
            _validatorFactory = validatorFactory;
            _logger = loggerFactory?.CreateLogger(typeof(FluentValidationSchemaProcessor)) ?? NullLogger.Instance;
            _rules = CreateDefaultRules();
            if (rules != null)
            {
                var ruleMap = _rules.ToDictionary(rule => rule.Name, rule => rule);
                foreach (var rule in rules)
                {
                    // Add or replace rule
                    ruleMap[rule.Name] = rule;
                }
                _rules = ruleMap.Values.ToList();
            }
        }
        
        /// <inheritdoc />
        public void Process(SchemaProcessorContext context)
        {
            if (!context.Schema.IsObject || context.Schema.Properties.Count == 0)
            {
                // Ignore other 
                // Ignore objects with no properties
                return;
            }

            IValidator validator = null;

            try
            {       
                validator = _validatorFactory.GetValidator(context.Type);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, $"GetValidator for type '{context.Type}' fails.");
            }
            
            if (validator == null)
            {
                return;
            }
                
            _logger.LogDebug($"Applying FluentValidation rules to swagger schema for type '{context.Type}'.");
            
            ApplyRulesToSchema(context, validator);

            try
            {
                AddRulesFromIncludedValidators(context, validator);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, $"Applying IncludeRules for type '{context.Type}' fails.");
            }
        }
        
        private void ApplyRulesToSchema(SchemaProcessorContext context, IValidator validator)
        {
             _logger.LogDebug($"Applying FluentValidation rules to swagger schema for type '{context.Type}'.");

            var schema = context.Schema;
            foreach (var key in schema?.Properties?.Keys ?? Array.Empty<string>())
            {
                var validators = validator.GetValidatorsForMemberIgnoreCase(key);

                foreach (var propertyValidator in validators)
                {
                    foreach (var rule in _rules)
                    {
                        if (rule.Matches(propertyValidator))
                        {
                            try
                            {
                                rule.Apply(new RuleContext(context, key, propertyValidator));
                                
                                _logger.LogDebug($"Rule '{rule.Name}' applied for property '{context.Type.Name}.{key}'");
                            }
                            catch (Exception e)
                            {
                                _logger.LogWarning(0, e, $"Error on apply rule '{rule.Name}' for property '{context.Type.Name}.{key}'.");
                            }
                        }
                    }
                }
            }
        }
        
        private void AddRulesFromIncludedValidators(SchemaProcessorContext context, IValidator validator)
        {
            // Note: IValidatorDescriptor doesn't return IncludeRules so we need to get validators manually.
            var includeRules = (validator as IEnumerable<IValidationRule>)
                .NotNull()
                .OfType<PropertyRule>()
                .Where(includeRule =>
                    includeRule.Condition == null && includeRule.AsyncCondition == null
                                                  && includeRule.GetType().IsGenericType 
                                                  && includeRule.GetType().GetGenericTypeDefinition() == typeof(IncludeRule<>)
                )
                .ToList();
            
            var childAdapters = includeRules  
                // 2nd filter 
                .SelectMany(includeRule => includeRule.Validators)
                .Where(x => x.GetType().IsGenericType 
                    && x.GetType().GetGenericTypeDefinition() == typeof(ChildValidatorAdaptor<,>))
                .ToList();

            foreach (var adapter in childAdapters)
            {
                var propertyValidatorContext = new PropertyValidatorContext(new ValidationContext<object>(null), null, string.Empty);
                if (adapter.GetType().GetGenericTypeDefinition() == typeof(ChildValidatorAdaptor<,>))
                {
                    var adapterType = adapter.GetType();

                    var adapterMethod = adapterType
                        .GetMethod("GetValidator");

                    if (adapterMethod != null)
                    {
                        IValidator includeValidator = adapterMethod
                            .Invoke(adapter, new object[] { propertyValidatorContext }) as IValidator;

                        ApplyRulesToSchema(context, includeValidator);
                        AddRulesFromIncludedValidators(context, includeValidator);
                    }

                }
            }
        }
        
        /// <summary>
        /// Creates default rules.
        /// Can be overriden by name.
        /// </summary>
        private static FluentValidationRule[] CreateDefaultRules()
        {
            return new[]
            {
                new FluentValidationRule("Required")
                {
                    Matches = propertyValidator => propertyValidator is INotNullValidator || propertyValidator is INotEmptyValidator,
                    Apply = context =>
                    {
                        var schema = context.SchemaProcessorContext.Schema;
                        
                        if (schema == null)
                            return;
                        
                        if (!schema.RequiredProperties.Contains(context.PropertyKey))
                            schema.RequiredProperties.Add(context.PropertyKey);
                    }
                },
                new FluentValidationRule("NotNull")
                {
                    Matches = propertyValidator => propertyValidator is INotNullValidator,
                    Apply = context =>
                    {
                        var schema = context.SchemaProcessorContext.Schema;

                        schema.Properties[context.PropertyKey].IsNullableRaw = false;

                        if (schema.Properties[context.PropertyKey].Type.HasFlag(JsonObjectType.Null))
                        {
                            schema.Properties[context.PropertyKey].Type &= ~JsonObjectType.Null; // Remove nullable
                        }

                        var oneOfsWithReference = schema.Properties[context.PropertyKey].OneOf.Where(x => x.Reference != null).ToList();
                        if (oneOfsWithReference.Count == 1)
                        {
                            // Set the Reference directly instead and clear the OneOf collection
                            schema.Properties[context.PropertyKey].Reference = oneOfsWithReference.Single();
                            schema.Properties[context.PropertyKey].OneOf.Clear();
                        }
                    }
                },
                new FluentValidationRule("NotEmpty")
                {
                    Matches = propertyValidator => propertyValidator is INotEmptyValidator,
                    Apply = context =>
                    {
                        var schema = context.SchemaProcessorContext.Schema;

                        schema.Properties[context.PropertyKey].IsNullableRaw = false;
                        
                        if (schema.Properties[context.PropertyKey].Type.HasFlag(JsonObjectType.Null))
                        {
                            schema.Properties[context.PropertyKey].Type &= ~JsonObjectType.Null; // Remove nullable
                        }

                        var oneOfsWithReference = schema.Properties[context.PropertyKey].OneOf.Where(x => x.Reference != null).ToList();
                        if (oneOfsWithReference.Count == 1)
                        {
                            // Set the Reference directly instead and clear the OneOf collection
                            schema.Properties[context.PropertyKey].Reference = oneOfsWithReference.Single();
                            schema.Properties[context.PropertyKey].OneOf.Clear();
                        }

                        schema.Properties[context.PropertyKey].MinLength = 1;
                    }
                },
                new FluentValidationRule("Length")
                {
                    Matches = propertyValidator => propertyValidator is ILengthValidator,
                    Apply = context =>
                    {
                        var schema = context.SchemaProcessorContext.Schema;

                        var lengthValidator = (ILengthValidator)context.PropertyValidator;

                        if(lengthValidator.Max > 0)
                            schema.Properties[context.PropertyKey].MaxLength = lengthValidator.Max;

                        if (lengthValidator is MinimumLengthValidator
                            || lengthValidator is ExactLengthValidator
                            || schema.Properties[context.PropertyKey].MinLength == null)
                            schema.Properties[context.PropertyKey].MinLength = lengthValidator.Min;
                    }
                },
                new FluentValidationRule("Pattern")
                {
                    Matches = propertyValidator => propertyValidator is IRegularExpressionValidator,
                    Apply = context =>
                    {
                        var regularExpressionValidator = (IRegularExpressionValidator)context.PropertyValidator;
                        
                        var schema = context.SchemaProcessorContext.Schema;
                        schema.Properties[context.PropertyKey].Pattern = regularExpressionValidator.Expression;
                    }
                },
                new FluentValidationRule("Comparison")
                {
                    Matches = propertyValidator => propertyValidator is IComparisonValidator,
                    Apply = context =>
                    {
                        var comparisonValidator = (IComparisonValidator)context.PropertyValidator;
                        if (comparisonValidator.ValueToCompare.IsNumeric())
                        {
                            var valueToCompare = Convert.ToDecimal(comparisonValidator.ValueToCompare);
                            var schema = context.SchemaProcessorContext.Schema;
                            var schemaProperty = schema.Properties[context.PropertyKey];

                            if (comparisonValidator.Comparison == Comparison.GreaterThanOrEqual)
                            {
                                schemaProperty.Minimum = valueToCompare;
                            }
                            else if (comparisonValidator.Comparison == Comparison.GreaterThan)
                            {
                                schemaProperty.Minimum = valueToCompare;
                                schemaProperty.IsExclusiveMinimum = true;
                            }
                            else if (comparisonValidator.Comparison == Comparison.LessThanOrEqual)
                            {
                                schemaProperty.Maximum = valueToCompare;
                            }
                            else if (comparisonValidator.Comparison == Comparison.LessThan)
                            {
                                schemaProperty.Maximum = valueToCompare;
                                schemaProperty.IsExclusiveMaximum = true;
                            }
                        }
                    }
                },
                new FluentValidationRule("Between")
                {
                    Matches = propertyValidator => propertyValidator is IBetweenValidator,
                    Apply = context =>
                    {
                        var betweenValidator = (IBetweenValidator)context.PropertyValidator;
                        var schema = context.SchemaProcessorContext.Schema;
                        var schemaProperty = schema.Properties[context.PropertyKey];

                        if (betweenValidator.From.IsNumeric())
                        {
                            if (betweenValidator is ExclusiveBetweenValidator)
                            {
                                schemaProperty.ExclusiveMinimum = Convert.ToDecimal(betweenValidator.From);
                            }
                            else
                            {
                                schemaProperty.Minimum = Convert.ToDecimal(betweenValidator.From);
                            }
                        }

                        if (betweenValidator.To.IsNumeric())
                        {
                            if (betweenValidator is ExclusiveBetweenValidator)
                            {
                                schemaProperty.ExclusiveMaximum = Convert.ToDecimal(betweenValidator.To);
                            }
                            else
                            {
                                schemaProperty.Maximum = Convert.ToDecimal(betweenValidator.To);
                            }
                        }
                    }
                },
                new FluentValidationRule("AspNetCoreCompatibleEmail")
                {
                    Matches = propertyValidator => propertyValidator is AspNetCoreCompatibleEmailValidator,
                    Apply = context =>
                    {
                        var schema = context.SchemaProcessorContext.Schema;
                        schema.Properties[context.PropertyKey].Pattern = "^[^@]+@[^@]+$"; // [^@] All chars except @
                    }
                },
            };
        }
    }
}