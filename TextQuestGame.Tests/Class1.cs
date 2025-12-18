using NUnit.Framework;
using System.IO;
using TextQuestGame;
using TextQuestGame.Model;

namespace TextQuestGame.Tests
{
    [TestFixture]
    public class GameServiceTests
    {
        [Test]
        public void SaveAndLoad_Works()
        {
            var service = new GameService();
            service.SaveGame("test.json");

            var service2 = new GameService();
            service2.LoadGame("test.json");

            Assert.That(service2.GetCurrentScene(), Is.Not.Null);
        }
    }
}
