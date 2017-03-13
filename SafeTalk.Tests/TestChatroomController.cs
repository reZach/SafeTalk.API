using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeTalk.API.Controllers;
using SafeTalk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace SafeTalk.Tests
{
    [TestClass]
    [DeploymentItem("sharedAppSettings.config")]
    [DeploymentItem("secretConnectionStrings.config")]
    class TestChatroomController
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
    }
}
