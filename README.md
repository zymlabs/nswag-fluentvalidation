# ZymLabs.NSwag.FluentValidation

Use FluentValidation rules to define validation requirements for NSwag Swagger/OpenAPI schema.

This project has been split into two packages.

- ZymLabs.NSwag.FluentValidation
  - Provides the core logic, can be included in an application project.
- ZymLabs.NSwag.FluentValidation.AspNetCore
  - Provides the AspNetCore HTTP helpers such as the `HttpContextServiceProviderValidatorFactory` for supporting HttpContext scoped services when using FluentValidation with DB Context validators.

## Statuses

[![License](https://img.shields.io/github/license/zymlabs/nswag-fluentvalidation.svg)](https://raw.githubusercontent.com/zymlabs/nswag-fluentvalidation/master/LICENSE)
[![CI/CD](https://github.com/zymlabs/nswag-fluentvalidation/actions/workflows/ci.yml/badge.svg)](https://github.com/zymlabs/nswag-fluentvalidation/actions/workflows/ci.yml)

## Usage

### 1. Reference packages in your web project:

```console
dotnet add package ZymLabs.NSwag.FluentValidation.AspNetCore
```

Or if you want to use the core package for your application without the AspNetCore MVC dependencies.

```console
dotnet add package ZymLabs.NSwag.FluentValidation
```

### 2. Change Startup.cs

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // HttpContextServiceProviderValidatorFactory requires access to HttpContext
    services.AddHttpContextAccessor();

    services
        .AddControllers()

        // Adds fluent validators to Asp.net
        .AddFluentValidation(c =>
        {
            c.RegisterValidatorsFromAssemblyContaining<Startup>();

            // Optionally set validator factory if you have problems with scope resolve inside validators.
            c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
        })

    services.AddOpenApiDocument((settings, serviceProvider) =>
    {
        var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();

        // Add the fluent validations schema processor
        settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
    });

    // Add the FluentValidationSchemaProcessor as a scoped service
    serviceCollection.AddScoped<FluentValidationSchemaProcessor>(provider =>
    {
        var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
        var loggerFactory = provider.GetService<ILoggerFactory>();

        return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
    });
}
```

## Version compatibility

ZymLabs.NSwag.FluentValidation | Swashbuckle.AspNetCore | FluentValidation
---------|----------|---------
[0.1.0, 0.4.0) | [13.0.0, 14.0.0) | >=7.2.0
[0.4.0, 0.5.0) | [13.0.0, 14.0.0) | >=10.0.0

## Supported validators

* INotNullValidator (NotNull)
* INotEmptyValidator (NotEmpty)
* ILengthValidator (Length, MinimumLength, MaximumLength, ExactLength)
* IRegularExpressionValidator (Email, Matches)
* IComparisonValidator (GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual)
* IBetweenValidator (InclusiveBetween, ExclusiveBetween)

## Extensibility

You can register FluentValidationRule in ServiceCollection.

User defined rule name replaces default rule with the same.
Full list of default rules can be get by `FluentValidationRules.CreateDefaultRules()`

List or default rules:

* Required
* NotEmpty
* Length
* Pattern
* Comparison
* Between

Example of rule:

```csharp
new FluentValidationRule("Pattern")
{
    Matches = propertyValidator => propertyValidator is IRegularExpressionValidator,
    Apply = context =>
    {
        var regularExpressionValidator = (IRegularExpressionValidator)context.PropertyValidator;
        context.Schema.Properties[context.PropertyKey].Pattern = regularExpressionValidator.Expression;
    }
},
```

## Samples

### Swagger Sample model and validator

```csharp
public class Sample
{
    public string PropertyWithNoRules { get; set; }

    public string NotNull { get; set; }
    public string NotEmpty { get; set; }
    public string EmailAddress { get; set; }
    public string RegexField { get; set; }

    public int ValueInRange { get; set; }
    public int ValueInRangeExclusive { get; set; }

    public float ValueInRangeFloat { get; set; }
    public double ValueInRangeDouble { get; set; }
}

public class SampleValidator : AbstractValidator<Sample>
{
    public SampleValidator()
    {
        RuleFor(sample => sample.NotNull).NotNull();
        RuleFor(sample => sample.NotEmpty).NotEmpty();
        RuleFor(sample => sample.EmailAddress).EmailAddress();
        RuleFor(sample => sample.RegexField).Matches(@"(\d{4})-(\d{2})-(\d{2})");

        RuleFor(sample => sample.ValueInRange).GreaterThanOrEqualTo(5).LessThanOrEqualTo(10);
        RuleFor(sample => sample.ValueInRangeExclusive).GreaterThan(5).LessThan(10);

        RuleFor(sample => sample.ValueInRangeFloat).InclusiveBetween(1.1f, 5.3f);
        RuleFor(sample => sample.ValueInRangeDouble).ExclusiveBetween(2.2, 7.5f);
    }
}
```

### Swagger Sample model screenshot

![SwaggerSample](docs/images/swagger_sample.png "SwaggerSample")

### Validator with Include

```csharp
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(customer => customer.Surname).NotEmpty();
        RuleFor(customer => customer.Forename).NotEmpty().WithMessage("Please specify a first name");

        Include(new CustomerAddressValidator());
    }
}

internal class CustomerAddressValidator : AbstractValidator<Customer>
{
    public CustomerAddressValidator()
    {
        RuleFor(customer => customer.Address).Length(20, 250);
    }
}
```

## Upgrading

- To 0.6.0

    Update the dependency injection to specify the constructor to use as there is now ambiguity with the other constructor.

    ```csharp
    System.InvalidOperationException
        Unable to activate type 'ZymLabs.NSwag.FluentValidation.FluentValidationSchemaProcessor'. The following constructors are ambiguous:

        Void .ctor(System.IServiceProvider, System.Collections.Generic.IEnumerable`1[ZymLabs.NSwag.FluentValidation.FluentValidationRule], Microsoft.Extensions.Logging.ILoggerFactory)

        Void .ctor(FluentValidation.IValidatorFactory, System.Collections.Generic.IEnumerable`1[ZymLabs.NSwag.FluentValidation.FluentValidationRule], Microsoft.Extensions.Logging.ILoggerFactory)

        at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory.CreateConstructorCallSite(ResultCache lifetime, Type serviceType, Type implementationType, CallSiteChain callSiteChain)
    ```

    From:

    ```csharp
    serviceCollection.AddScoped<FluentValidationSchemaProcessor>();
    ```

    To:

    ```csharp
    serviceCollection.AddScoped<FluentValidationSchemaProcessor>(provider =>
    {
        var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
        var loggerFactory = provider.GetService<ILoggerFactory>();

        return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
    });
    ```

## Credits

This project is a port of [MicroElements.Swashbuckle.FluentValidation](https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation) to NSwag.
The initial version of this project was based on
[Mujahid Daud Khan](https://stackoverflow.com/users/1735196/mujahid-daud-khan) answer on StackOverflow:
https://stackoverflow.com/questions/44638195/fluent-validation-with-swagger-in-asp-net-core/49477995#49477995
