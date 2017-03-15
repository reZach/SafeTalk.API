using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using System.Web.Http;
using SafeTalk.Models;
using SafeTalk.API.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeTalk.Tests
{
    [TestClass]
    [DeploymentItem("sharedAppSettings.config")]
    [DeploymentItem("secretConnectionStrings.config")]
    public class TestChatroomController
    {
        ChatroomController chatroomController { get; set; }

        [TestInitialize]
        public void Init()
        {
            chatroomController = new ChatroomController();
        }

        [TestMethod]
        public void Post_ShouldReturnNewChatroom()
        {
            var name = RandomName();
            var fakeChatroom = new Chatroom();
            IHttpActionResult response = chatroomController.Post(name);
            var contentResult = response as OkNegotiatedContentResult<Chatroom>;

            Assert.AreEqual(contentResult.Content.GetType(), fakeChatroom.GetType());
        }

        [TestMethod]
        public void Sequence_ShouldPostThenRetrieveChatroom()
        {
            var name = RandomName();
            IHttpActionResult response = chatroomController.Post(name);

            IHttpActionResult response2 = chatroomController.Get(name);
            var contentResult2 = response2 as OkNegotiatedContentResult<Chatroom>;

            Assert.AreEqual(name, contentResult2.Content.Name);
        }

        [TestMethod]
        public void Post_ShouldAddChatroomToCache()
        {
            IHttpActionResult response = chatroomController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<Chatroom>>;
            var previousCount = contentResult.Content.Count;

            IHttpActionResult response2 = chatroomController.Post(RandomName());
            IHttpActionResult response3 = chatroomController.Get();
            var contentResult3 = response3 as OkNegotiatedContentResult<List<Chatroom>>;
            var newCount = contentResult3.Content.Count;

            Assert.AreEqual(previousCount + 1, newCount);
        }

        [TestMethod]
        public void Get_ShouldReturnListOfChatrooms()
        {
            var compareTo = new List<Chatroom>();
            IHttpActionResult response = chatroomController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<Chatroom>>;

            Assert.AreEqual(contentResult.Content.GetType(), compareTo.GetType());
        }

        [TestMethod]
        public void Put_FailsToUpdateBecauseChatroomDoesntExist()
        {
            var notInCacheChatroom = new Chatroom();
            notInCacheChatroom.Name = RandomName();

            IHttpActionResult response = chatroomController.Put(notInCacheChatroom);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Put_SucceedsForExistingChatroom()
        {
            IHttpActionResult response = chatroomController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<Chatroom>>;
            Chatroom chatroom = contentResult.Content[0];
            IHttpActionResult response2 = chatroomController.Put(chatroom);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<Chatroom>));
        }

        [TestMethod]
        public void Delete_FailsForNonExistingChatroom()
        {
            Chatroom chatroom = new Chatroom();
            chatroom.Name = RandomName();
            IHttpActionResult response = chatroomController.Delete(chatroom);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_SucceedsForExistingChatroom()
        {
            var name = RandomName();
            IHttpActionResult response = chatroomController.Post(name);
            var contentResult = response as OkNegotiatedContentResult<Chatroom>;
            var chatroom = contentResult.Content;
            IHttpActionResult response2 = chatroomController.Delete(chatroom);

            Assert.IsInstanceOfType(response2, typeof(OkNegotiatedContentResult<Chatroom>));
        }

        [TestMethod]
        public void Delete_ShouldRemoveChatroomFromCache()
        {
            IHttpActionResult response = chatroomController.Get();
            var contentResult = response as OkNegotiatedContentResult<List<Chatroom>>;
            var previousCount = contentResult.Content.Count;

            var chatroom = contentResult.Content[0];
            IHttpActionResult response2 = chatroomController.Delete(chatroom);
            IHttpActionResult response3 = chatroomController.Get();
            var contentResult2 = response3 as OkNegotiatedContentResult<List<Chatroom>>;
            var newCount = contentResult2.Content.Count;

            Assert.AreEqual(previousCount - 1, newCount);
        }

        private string RandomName()
        {
            Random digits = new Random();
            string append = digits.Next(1000).ToString().PadLeft(4, '0');
            var name = "test" + append;

            return name;
        }
    }
}
