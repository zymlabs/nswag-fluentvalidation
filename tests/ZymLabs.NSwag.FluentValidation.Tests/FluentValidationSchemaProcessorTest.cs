using System;
using FluentValidation;
using FluentValidation.Validators;
using Moq;
using NJsonSchema;
using NJsonSchema.Generation;
using Xunit;

namespace ZymLabs.NSwag.FluentValidation.Tests
{
    public class MockValidationTarget
    {
        public string StringLength { get; set; }
        public int Int { get; set; }
        public int IntExclusive { get; set; }
    }

    public class MockValidationTargetValidator : AbstractValidator<MockValidationTarget>
    {
        public MockValidationTargetValidator()
        {
            RuleFor(x => x.StringLength).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Int).GreaterThanOrEqualTo(2).LessThanOrEqualTo(11);
            RuleFor(x => x.IntExclusive).GreaterThan(5).LessThan(10);
        }
    }
    
    public class FluentValidationSchemaProcessorTest
    {
        [Fact]
        public void ProcessModifiesSchemaToValidations()
        {
            // Arrange
            var testValidator = new MockValidationTargetValidator();
            
            var validatorFactoryMock = new Mock<IValidatorFactory>();
            validatorFactoryMock.Setup(x => x.GetValidator(It.IsAny<Type>())).Returns(testValidator);

            var validatorFactory = validatorFactoryMock.Object;
            
            var fluentValidationSchemaProcessor= new FluentValidationSchemaProcessor(validatorFactory);
            
            var jsonSchemaGeneratorSettings = new JsonSchemaGeneratorSettings();
            jsonSchemaGeneratorSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
            
            // Act
            var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);
            
            // Assert
            Assert.Equal(1, schema.Properties["StringLength"].MinLength);
            Assert.Equal(500, schema.Properties["StringLength"].MaxLength);
            Assert.Equal(2, schema.Properties["Int"].Minimum);
            Assert.Equal(11, schema.Properties["Int"].Maximum);
            Assert.Equal(5, schema.Properties["IntExclusive"].ExclusiveMinimum);
            Assert.Equal(10, schema.Properties["IntExclusive"].ExclusiveMaximum);
        }
    }
}
