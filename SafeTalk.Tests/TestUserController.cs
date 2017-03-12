using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SafeTalk.API.Controllers;
using SafeTalk.Models;
using System.Configuration;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;

namespace SafeTalk.Tests
{
    [TestClass]
    [DeploymentItem("sharedAppSettings.config")]
    [DeploymentItem("secretConnectionStrings.config")]
    public class TestUserController
    {
        [TestMethod]        
        public void Post_ShouldReturnNewUser()
        {
            var controller = new UserController();

            var fakeUser = new User();
            IHttpActionResult response = controller.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;

            Assert.AreEqual(contentResult.Content.GetType(), fakeUser.GetType());
        }



        private User GetUser()
        {
            var user = new User();
            return user;
        }
    }
}
