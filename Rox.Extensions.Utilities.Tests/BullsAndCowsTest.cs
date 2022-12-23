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


        [Fact]
        public void TestRobot()
        {
            //进行1000次猜谜游戏
            int totalTime = 0;
            for (int i = 0; i< 1000; i++)
            {
                var game = new Game();
                var robot = new Robot();
                robot.Start(game.Options);

                do
                {
                    var result = game.Guess(robot.GetNextAnswer());
                    robot.Return(result);
                    if(result.A == game.Options.Length)
                    {
                        break;
                    }
                } while (true);
                totalTime += game.Round;
            }
            Assert.True(totalTime < 1000 * 8);
        }
    }
}