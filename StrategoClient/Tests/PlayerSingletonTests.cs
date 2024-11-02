using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.Model;
using StrategoApp.LogInService;

namespace Tests
{
    [TestClass]
    public class PlayerSingletonTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            PlayerSingleton.Instance.LogOut();
        }

        //[TestMethod]
        //public void LogIn_ShouldSetPlayer()
        //{
        //    var testPlayer = new PlayerDTO
        //    {
        //        Id = 1,
        //        Name = "TestUser",
        //        PicturePath = "path/to/picture.png"
        //    };

        //    PlayerSingleton.Instance.LogIn(testPlayer);

        //    Assert.AreEqual("TestUser", PlayerSingleton.Instance.Player.Name);
        //}

        //[TestMethod]
        //public void LogOut_ShouldClearPlayer()
        //{
        //    var testPlayer = new PlayerDTO
        //    {
        //        Id = 1,
        //        Name = "TestUser",
        //        PicturePath = "path/to/picture.png"
        //    };
        //    PlayerSingleton.Instance.LogIn(testPlayer);

        //    PlayerSingleton.Instance.LogOut();

        //    Assert.IsNull(PlayerSingleton.Instance.Player);
        //}

        //[TestMethod]
        //public void Test_IsLoggedIn_WhenPlayerIsLoggedIn_ShouldReturnTrue()
        //{
        //    var testPlayer = new PlayerDTO
        //    {
        //        Id = 1,
        //        Name = "TestUser",
        //        PicturePath = "path/to/picture.png"
        //    };
        //    PlayerSingleton.Instance.LogIn(testPlayer);

        //    var isLoggedIn = PlayerSingleton.Instance.IsLoggedIn();

        //    Assert.IsTrue(isLoggedIn);
        //}

        [TestMethod]
        public void Test_IsLoggedIn_WhenNoPlayerIsLoggedIn_ShouldReturnFalse()
        {
            var isLoggedIn = PlayerSingleton.Instance.IsLoggedIn();

            Assert.IsFalse(isLoggedIn);
        }
    }
}
