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
            user.Wins = 3;
            user.Draws = 1;
            user.Losses = 1;
            user.GamesPlayed = 5;

            Assert.IsNotNull(user);
            Assert.AreEqual("newusername", user.Username);
            Assert.AreEqual("newusername", user.Username);
            Assert.AreEqual(3, user.Wins);
            Assert.AreEqual(1, user.Draws);
            Assert.AreEqual(1, user.Losses);
            Assert.AreEqual(5, user.GamesPlayed);
        }

        [TestMethod]
        public void User_Default_Attributes()
        {
            User user = new User("username", "password");

            Assert.IsNotNull(user);
            Assert.AreEqual(20, user.Coins);
            Assert.AreEqual(0, user.GamesPlayed);
            Assert.AreEqual(100, user.Elo);
        }

    }
}
