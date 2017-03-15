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
        UserController userController { get; set; }

        [TestInitialize]
        public void Init()
        {
            userController = new UserController();
        }

        [TestMethod]        
        public void Post_ShouldReturnNewUser()
        {
            var fakeUser = new User();
            IHttpActionResult response = userController.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;

            Assert.AreEqual(contentResult.Content.GetType(), fakeUser.GetType());
        }

        [TestMethod]
        public void Sequence_ShouldPostThenRetrieveUser()
        {
            IHttpActionResult response = userController.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;
            var user = contentResult.Content;
            IHttpActionResult response2 = userController.Get(user.Guid);
            var contentResult2 = response2 as OkNegotiatedContentResult<User>;

            Assert.AreEqual(contentResult2.Content.Guid, user.Guid);
        }

        [TestMethod]
        public void Post_ShouldAddUserToCache()
        {
            IHttpActionResult response = userController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            var previousCount = contentResult.Content.Count;

            IHttpActionResult response2 = userController.Post();
            IHttpActionResult response3 = userController.Get();
            var contentResult3 = response3 as OkNegotiatedContentResult<List<User>>;
            var newCount = contentResult3.Content.Count;

            Assert.AreEqual(previousCount + 1, newCount);
        }

        [TestMethod]        
        public void Get_ShouldReturnListOfUsers()
        {
            var compareTo = new List<User>();
            IHttpActionResult response = userController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;

            Assert.AreEqual(contentResult.Content.GetType(), compareTo.GetType());
        }               

        [TestMethod]
        public void Put_FailsToUpdateBecauseUserDoesntExist()
        {
            var notInCacheUser = new User();
            IHttpActionResult response = userController.Put(notInCacheUser);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Put_SucceedsForExistingUser()
        {
            IHttpActionResult response = userController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            User user = contentResult.Content[0];
            IHttpActionResult response2 = userController.Put(user);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<User>));
        }

        [TestMethod]
        public void Put_SetsNewRandomNameForUser()
        {
            IHttpActionResult response = userController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            User user = contentResult.Content[0];
            string oldName = user.Name;
            IHttpActionResult response2 = userController.Put(user, true);
            var contentResult2 = response2 as OkNegotiatedContentResult<User>;

            Assert.AreNotEqual(oldName, contentResult2.Content.Name);
        }

        [TestMethod]
        public void Delete_FailsForNonExistingUser()
        {
            User user = new User();
            IHttpActionResult response = userController.Delete(user);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_SucceedsForExistingUser()
        {
            IHttpActionResult response = userController.Post();
            var contentResult = response as OkNegotiatedContentResult<User>;
            var user = contentResult.Content;
            IHttpActionResult response2 = userController.Delete(user);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<User>));
        }

        [TestMethod]
        public void Delete_ShouldRemoveUserFromCache()
        {
            IHttpActionResult response = userController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<User>>;
            var previousCount = contentResult.Content.Count;

            var user = contentResult.Content[0];
            IHttpActionResult response2 = userController.Delete(user);
            IHttpActionResult response3 = userController.Get();
            var contentResult2 = response3 as OkNegotiatedContentResult<List<User>>;
            var newCount = contentResult2.Content.Count;

            Assert.AreEqual(previousCount - 1, newCount);
        }
    }
}
