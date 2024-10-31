using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void User_Initialization_ParameterizedConstructor()
        {
            User user = new User("username", "password");

            Assert.IsNotNull(user);
            Assert.AreEqual("username", user.Username);
            Assert.AreEqual("password", user.Password);
        }

        [TestMethod]
        public void User_Getter_Setter()
        {
            User user = new User("username", "password");

            user.Username = "newusername";
            user.Password = "newpassword";

            Assert.IsNotNull(user);
            Assert.AreEqual("newusername", user.Username);
            Assert.AreEqual("newpassword", user.Password);
        }

        [TestMethod]
        public void User_Default_Attributes()
        {
            User user = new User("username", "password");

            Assert.IsNotNull(user);
            Assert.AreEqual(20, user.Coins);
            Assert.AreEqual(100, user.Elo);
        }

    }
}
