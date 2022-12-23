using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.BullsAndCows;

public class Game
{
    public GameOptions Options { get; private set; }
    public string RightAnswer { get; private set; }
    public int Round { get; private set; }

    public Game(GameOptions options = default)
    {
        Options = options ?? new GameOptions();
        RightAnswer = GenerateAnswer();
    }

    private string GenerateAnswer()
    {
        char[] ans = new char[Options.Length];
        List<int> nbs = Enumerable.Range(0, 10).ToList();
        for(int i = 0; i< ans.Length; i++)
        {
            int n = nbs[RandomUtil.GetRandomNum(0, nbs.Count)];
            if(i== 0 && n == 0 && !Options.AllowStartWithZero)
            {
                var tmps = nbs.ToList();
                tmps.Remove(0);
                n = tmps[RandomUtil.GetRandomNum(0, tmps.Count)];
            }
            var c = n.ToString();
            ans[i] = c[0];

            if (!Options.AllowDuplicate)
            {
                nbs.Remove(n);
            }
        }
        return new string(ans);
    }

    public Result Guess(string guessNumber)
    {
        Round++;
        return Check(guessNumber, RightAnswer);
    }

    public static Result Check(string guessNumber, string rightAnswer)
    {
        if(guessNumber.Length != rightAnswer.Length || !int.TryParse(guessNumber, out var n))
        {
            throw new ArgumentException($"must be a number, and not longer than {rightAnswer.Length}!");
        }
        int a = 0;
        int b = 0;
        var ans = rightAnswer.ToCharArray();
        for(int i = 0; i < guessNumber.Length; i++)
        {
            var gn = guessNumber[i];
            for(int j = 0; j < rightAnswer.Length; j++)
            {
                var an = ans[j];
                if (gn == an)
                {
                    if(i == j)
                    {
                        a ++;
                        ans[j] = ' ';
                    }
                    else
                    {
                        b ++;
                        ans[j] = ' ';
                    }
                    break;
                }
            }
        }

        return new Result { A = a, B = b };
    }
}
