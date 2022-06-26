using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class PlayerTest : MusicTestBase
    {
        private TestPlayer getTestPlayer()
        {
            TestPlayer testPlayer = new();
            return testPlayer;
        }

        [TestMethod]
        public void CreatePlayerTest()
        {
            Player player = getTestPlayer();
            Assert.IsTrue(player.Initialized);
        }
    }
}
