using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using System.Web.Http;
using SafeTalk.Models;
using SafeTalk.API.Controllers;
using System.Collections.Generic;

namespace SafeTalk.Tests
{
    [TestClass]
    [DeploymentItem("sharedAppSettings.config")]
    [DeploymentItem("secretConnectionStrings.config")]
    public class TestChatroomController
    {
        [TestMethod]
        public void Post_ShouldReturnNewChatroom()
        {
            var controller = new ChatroomController();

            Random digits = new Random();
            string append = digits.Next(1000).ToString().PadLeft(4, '0');
            var name = "test" + append;
            var fakeChatroom = new Chatroom();
            IHttpActionResult response = controller.Post(name);
            var contentResult = response as OkNegotiatedContentResult<Chatroom>;

            Assert.AreEqual(contentResult.Content.GetType(), fakeChatroom.GetType());
        }

        [TestMethod]
        public void Sequence_ShouldPostThenRetrieveChatroom()
        {
            // to-do
        }

        [TestMethod]
        public void Get_ShouldReturnListOfChatrooms()
        {
            var controller = new ChatroomController();

            var compareTo = new List<Chatroom>();
            IHttpActionResult response = controller.Get();
            var contentResult = response as OkNegotiatedContentResult<List<Chatroom>>;

            Assert.AreEqual(contentResult.Content.GetType(), compareTo.GetType());
        }
    }
}
