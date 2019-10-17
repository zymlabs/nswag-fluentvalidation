using FluentValidation;

namespace ZymLabs.NSwag.FluentValidation.Tests
{
    public class MockValidationTargetIncludeValidator : AbstractValidator<MockValidationTarget>
    {
        public MockValidationTargetIncludeValidator()
        {
            RuleFor(sample => sample.IncludeField).NotNull();
        }
    }
}