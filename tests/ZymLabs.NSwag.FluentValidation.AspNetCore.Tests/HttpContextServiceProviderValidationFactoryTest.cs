using System;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ZymLabs.NSwag.FluentValidation.AspNetCore.Tests
{
    public class HttpContextServiceProviderValidationFactoryTest
    {
        [Fact]
        public void CreateInstanceReturnsValidator()
        {
            // Arrange
            var mockValidator = new TestValidator();
            var validatorType = typeof(TestValidator);

            // Mock ServiceProvider
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(validatorType))
                .Returns(mockValidator);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope
                .Setup(x => x.ServiceProvider)
                .Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            
            // Mock HttpContextAccessor
            var httpContextAccessor =  new Mock<IHttpContextAccessor>();
            
            // Mock HttpContext
            var context = new DefaultHttpContext();
            context.RequestServices = serviceProvider.Object;
            
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            
            // // Mock HeaderConfiguration
            // var mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            // mockHeaderConfiguration
            //     .Setup(_ => _.GetTenantId(It.IsAny<IHttpContextAccessor>()))
            //     .Returns(fakeTenantId);
            
            var validationFactory = new HttpContextServiceProviderValidatorFactory(httpContextAccessor.Object);
            
            // Act
            var validator = validationFactory.CreateInstance(validatorType);

            // Assert
            Assert.IsType<TestValidator>(httpContextAccessor.Object.HttpContext.RequestServices.GetService(validatorType));
            Assert.IsType<TestValidator>(validator);
        }
        
        [Fact]
        public void CreateInstanceReturnsValidatorThrowsExceptionWhenNull()
        {
            // Arrange
            IValidator mockValidator = null;
            var validatorType = typeof(TestValidator);

            // Mock ServiceProvider
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(validatorType))
                .Returns(mockValidator);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope
                .Setup(x => x.ServiceProvider)
                .Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            
            // Mock HttpContextAccessor
            var httpContextAccessor =  new Mock<IHttpContextAccessor>();
            
            // Mock HttpContext
            var context = new DefaultHttpContext();
            context.RequestServices = serviceProvider.Object;
            
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            
            // // Mock HeaderConfiguration
            // var mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            // mockHeaderConfiguration
            //     .Setup(_ => _.GetTenantId(It.IsAny<IHttpContextAccessor>()))
            //     .Returns(fakeTenantId);
            
            var validationFactory = new HttpContextServiceProviderValidatorFactory(httpContextAccessor.Object);
            
            // Act

            // Assert
            Assert.Null(httpContextAccessor.Object.HttpContext?.RequestServices.GetService(validatorType));

            Assert.Throws<Exception>(() => validationFactory.CreateInstance(validatorType));
        }
    }
}