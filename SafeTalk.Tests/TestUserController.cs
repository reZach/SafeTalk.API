using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SafeTalk.API.Controllers;
using SafeTalk.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;

namespace SafeTalk.Tests
{
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api
    // http://stackoverflow.com/questions/42741455/c-sharp-injecting-configurationmanager-within-unit-tests/42741692

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

        [TestMethod]
        public void Sequence_ShouldPostThenRetrieveUser()
        {
            var controller = new UserController();

            IHttpActionResult response = controller.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;
            var user = contentResult.Content;
            IHttpActionResult response2 = controller.Get(user.Guid);
            var contentResult2 = response2 as OkNegotiatedContentResult<User>;

            Assert.AreEqual(contentResult2.Content.Guid, user.Guid);
        }

        [TestMethod]
        public void Post_ShouldAddUserToCache()
        {
            var controller = new UserController();

            IHttpActionResult response = controller.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            var previousCount = contentResult.Content.Count;

            IHttpActionResult response2 = controller.Post();
            IHttpActionResult response3 = controller.Get();
            var contentResult3 = response3 as OkNegotiatedContentResult<List<User>>;
            var newCount = contentResult3.Content.Count;

            Assert.AreEqual(previousCount + 1, newCount);
        }

        [TestMethod]        
        public void Get_ShouldReturnListOfUsers()
        {
            var controller = new UserController();

            var compareTo = new List<User>();
            IHttpActionResult response = controller.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;

            Assert.AreEqual(contentResult.Content.GetType(), compareTo.GetType());
        }               

        [TestMethod]
        public void Put_FailsToUpdateBecauseUserDoesntExist()
        {
            var controller = new UserController();

            var notInCacheUser = new User();
            IHttpActionResult response = controller.Put(notInCacheUser);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Put_SucceedsForExistingUser()
        {
            var controller = new UserController();

            IHttpActionResult response = controller.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            User user = contentResult.Content[0];
            IHttpActionResult response2 = controller.Put(user);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<User>));
        }

        [TestMethod]
        public void Put_SetsNewRandomNameForUser()
        {
            var controller = new UserController();

            IHttpActionResult response = controller.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            User user = contentResult.Content[0];
            string oldName = user.Name;
            IHttpActionResult response2 = controller.Put(user, true);
            var contentResult2 = response2 as OkNegotiatedContentResult<User>;

            Assert.AreNotEqual(oldName, contentResult2.Content.Name);
        }

        [TestMethod]
        public void Delete_FailsForNonExistingUser()
        {
            var controller = new UserController();

            User user = new User();
            IHttpActionResult response = controller.Delete(user);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_SucceedsForExistingUser()
        {
            var controller = new UserController();

            IHttpActionResult response = controller.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;
            var user = contentResult.Content;
            IHttpActionResult response2 = controller.Delete(user);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<User>));
        }
    }
}
