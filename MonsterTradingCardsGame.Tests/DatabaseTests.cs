using MonsterTradingCardsGame.GameClasses;
using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGame.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        Database.Database db;

        public DatabaseTests()
        { 
            db = new Database.Database();
        }

        [TestMethod]
        public void User_Does_Not_Exist()
        {
            string username = "DoesNotExist";

            Assert.IsFalse(db.CheckUserExists(username));
        }

        [TestMethod]
        public void Register_User()
        {
            string username = "TestUser";
            string password = "TestPassword";

            db.RegisterUser(username, password);

            Assert.IsTrue(db.CheckUserExists(username));
        }

        [TestMethod]
        public void Login_User()
        {
            string username = "TestUser";
            string password = "TestPassword";

            Assert.IsNotNull(db.LoginUser(username, password));
        }

        [TestMethod]
        public void Login_User_Wrong_Password()
        {
            string username = "TestUser";
            string password = "WrongPassword";

            Assert.IsTrue(string.IsNullOrEmpty(db.LoginUser(username, password)));
        }

        [TestMethod]
        public void User_Is_Admin()
        {
            string username = "admin";

            Assert.IsTrue(db.CheckUserIsAdmin(username));
        }

        [TestMethod]
        public void User_Is_Not_Admin()
        {
            string username = "TestUser";

            Assert.IsFalse(db.CheckUserIsAdmin(username));
        }

        [TestMethod]
        public void Get_Username_From_Token()
        {
            string username = "TestUser";
            string password = "TestPassword";

            db.RegisterUser(username, password);
            string token = db.LoginUser(username, password);

            Assert.AreEqual(username, db.GetUsernameFromToken(token));
        }

        [TestMethod]
        public void Get_Username_From_Wrong_Token()
        {
            string username = "TestUser";
            string token = "WrongToken";

            Assert.AreNotEqual(username, db.GetUsernameFromToken(token));
            Assert.IsTrue(string.IsNullOrEmpty(db.GetUsernameFromToken(token)));
        }

        [TestMethod]
        public void Get_User()
        {
            string username = "TestUser";
            string password = "TestPassword";

            db.RegisterUser(username, password);

            User? user = db.GetUser(username);
            Assert.AreEqual(username, user?.Username);
            Assert.IsTrue(string.IsNullOrEmpty(user?.Name));
            Assert.IsTrue(string.IsNullOrEmpty(user?.Bio));
            Assert.IsTrue(string.IsNullOrEmpty(user?.Image));
            Assert.AreEqual(100, user?.Elo);
            Assert.AreEqual(0, user?.Wins);
            Assert.AreEqual(0, user?.Draws);
            Assert.AreEqual(0, user?.Losses);
            Assert.AreEqual(0, user?.GamesPlayed);
        }

        [TestMethod]
        public void Edit_User_Profile()
        {
            string username = "TestUser";
            string password = "TestPassword";

            db.RegisterUser(username, password);
            string newName = "TestName";
            string newBio = "TestBio";
            string newImage = "TestImage";
            db.EditProfile(username, newName, newBio, newImage);

            User? user = db.GetUser(username);
            Assert.AreEqual(newName, user?.Name);
            Assert.AreEqual(newBio, user?.Bio);
            Assert.AreEqual(newImage, user?.Image);
            Assert.AreEqual(100, user?.Elo);
            Assert.AreEqual(0, user?.Wins);
            Assert.AreEqual(0, user?.Draws);
            Assert.AreEqual(0, user?.Losses);
            Assert.AreEqual(0, user?.GamesPlayed);
        }

        [TestMethod]
        public void Update_User_Stats()
        {
            string username = "TestUser";
            string password = "TestPassword";

            db.RegisterUser(username, password);
            int newElo = 99;
            int newWins = 3;
            int newDraws = 1;
            int newLosses = 2;
            int newGamesPlayed = 6;
            db.UpdateUserStats(username, newElo, newWins, newDraws, newLosses, newGamesPlayed);

            User? user = db.GetUser(username);
            Assert.IsTrue(string.IsNullOrEmpty(user?.Name));
            Assert.IsTrue(string.IsNullOrEmpty(user?.Bio));
            Assert.IsTrue(string.IsNullOrEmpty(user?.Image));
            Assert.AreEqual(newElo, user?.Elo);
            Assert.AreEqual(newWins, user?.Wins);
            Assert.AreEqual(newDraws, user?.Draws);
            Assert.AreEqual(newLosses, user?.Losses);
            Assert.AreEqual(newGamesPlayed, user?.GamesPlayed);
        }

    }
}