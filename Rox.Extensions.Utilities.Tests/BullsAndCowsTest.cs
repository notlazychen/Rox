using Rox.Extensions.BullsAndCows;

namespace Rox.Extensions.Utilities.Tests
{
    public class BullsAndCowsTest
    {
        [Fact]
        public void TestGenerateGame()
        {
            for (int i = 0; i < 100; i++)
            {
                var game = new Game();
                var cnt2 = game.RightAnswer.Distinct().Count();
                Assert.Equal(4, game.RightAnswer.Length);
                Assert.Equal(4, cnt2);
            }
        }


        [Fact]
        public void TestCheckMethod()
        {
            var tests = new []
            {
                new []{ "5234", "5346", "1A2B"},
                new []{ "5543", "5255", "1A1B"},
                new []{ "1234", "1234", "4A0B"},
                new []{ "1234", "1235", "3A0B"},
                new []{ "1234", "4432", "1A2B"},
            };
            
            foreach(var test in tests)
            {
                var result = Game.Check(test[1], test[0]);
                Assert.Equal(result.ToString(), test[2]);
            }
        }
    }
}