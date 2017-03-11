using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeTalk.API.Controllers;
using SafeTalk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeTalk.Tests
{
    [TestClass]
    public class TestUserController
    {
        [TestMethod]
        public void Post_ShouldReturnNewUser()
        {
            var controller = new UserController();

            var result = controller.Post() as User;
            Assert.AreNotEqual(result, null);
        }



        private User GetUser()
        {
            var user = new User();
            return user;
        }
    }
}
