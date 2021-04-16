namespace ZymLabs.NSwag.FluentValidation.Tests
{
    public class MockValidationTarget
    {
        public string PropertyWithNoRules { get; set; } = "";

        public string Length { get; set; } = "";
        public string NotNull { get; set; } = "";
        public string NotEmpty { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string EmailAddressNet4 { get; set; } = "";

        public string RegexField { get; set; } = "";

        public int ValueInRange { get; set; }
        public int ValueInRangeExclusive { get; set; }

        public float ValueInRangeFloat { get; set; }
        public double ValueInRangeDouble { get; set; }

        public string IncludeField { get; set; } = "";

        public MockValidationTargetChild NotNullChild { get; set; } = new MockValidationTargetChild();
        public MockValidationTargetChild NotEmptyChild { get; set; } = new MockValidationTargetChild();
        
        public MockValidationTargetChildEnum NotNullChildEnum { get; set; }
        public MockValidationTargetChildEnum NotEmptyChildEnum { get; set; }

    }
}