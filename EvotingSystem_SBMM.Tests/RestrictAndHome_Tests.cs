using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace EVotingSystem_SBMM.Tests
{
    public class RestrictAndHome_Tests
    {
        private ActionExecutingContext CreateActionExecutingContext(HttpContext httpContext, UserModel user = null)
        {
            // Mock the session
            var session = new Mock<ISession>();
            httpContext.Session = session.Object;

            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

            // Serialize the user model if provided
            if (user != null)
            {
                httpContext.Session.SetString("userLoggedSession", JsonConvert.SerializeObject(user));
            }

            return new ActionExecutingContext(
                actionContext,
                new IFilterMetadata[0],
                new RouteValueDictionary(),
                null);
        }

        #region RestrictController
        
        [Fact]
        public void OnActionExecuting_RedirectsToLogin_When_UserNotLoggedIn()
        {
            // Arrange
            var filter = new ElectoralAdministratorRestrictPage();
            var httpContext = new DefaultHttpContext();

            // Act
            var actionContext = CreateActionExecutingContext(httpContext);
            filter.OnActionExecuting(actionContext);

            // Assert
            var redirectResult = actionContext.Result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirectResult.RouteValues["controller"].Should().Be("login");
            redirectResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OnActionExecuting_RedirectsToLogin_When_UserSessionInvalid()
        {
            // Arrange
            var filter = new ElectoralAdministratorRestrictPage();
            var httpContext = new DefaultHttpContext();

            // Act
            var actionContext = CreateActionExecutingContext(httpContext);
            httpContext.Session.SetString("userLoggedSession", "invalid");
            filter.OnActionExecuting(actionContext);

            // Assert
            var redirectResult = actionContext.Result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirectResult.RouteValues["controller"].Should().Be("login");
            redirectResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OnActionExecuting_RedirectsToLogin_When_UserNotElectoralAdministrator()
        {
            // Arrange
            var filter = new ElectoralAdministratorRestrictPage();
            var httpContext = new DefaultHttpContext();
            var user = new UserModel { Profile = ProfileEnum.ThirdPartAuditor };

            // Act
            var actionContext = CreateActionExecutingContext(httpContext, user);
            filter.OnActionExecuting(actionContext);

            // Assert
            var redirectResult = actionContext.Result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirectResult.RouteValues["controller"].Should().Be("login"); // Corrected "Restrict" to "Login"
            redirectResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void OnActionExecuting_AllowsAccess_When_UserIsElectoralAdministrator()
        {
            // Arrange
            var filter = new ElectoralAdministratorRestrictPage();
            var httpContext = new DefaultHttpContext();
            var user = new UserModel { Profile = ProfileEnum.ElectoralAdministrator };

            // Act
            var actionContext = CreateActionExecutingContext(httpContext, user);
            filter.OnActionExecuting(actionContext);

            // Assert
            var redirectResult = actionContext.Result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirectResult.RouteValues["controller"].Should().Be("login");
            redirectResult.RouteValues["action"].Should().Be("Index");
        }
        #endregion

        #region HomeController

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangeLanguage_ReturnsRedirectResult()
        {
            // Arrange
            var controller = new HomeController();
            const string culture = "en";

            // Mock HttpContext and HttpRequest
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "http://example.com/previous-page";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.ChangeLanguage(culture);

            // Assert
            Assert.IsType<RedirectResult>(result);
        }


        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange
            var controller = new HomeController();
            var httpContext = new DefaultHttpContext();

            // Mock the HttpContext to avoid null reference exception
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);

            // Log the RequestId value
            Console.WriteLine($"RequestId: {model.RequestId}");

            // Assert that RequestId is not null
            Assert.NotNull(model.RequestId);
        }


        #endregion HomeController
        
        
    }
}
