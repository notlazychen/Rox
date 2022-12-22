using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.BullsAndCows;

public class Robot
{
    private List<string> _answers = new List<string>();
    private List<string> _suggests = new List<string>();
    private Dictionary<char, int> _scores = new Dictionary<char, int>();
    public void Start(GameOptions options)
    {
        int max = (int)Math.Pow(10, options.Length);
        _answers.Clear();
        _scores = Enumerable.Range(0, 10).ToDictionary(x => x.ToString()[0], x => 0);
        _suggests = Enumerable.Range(0, max)
            .Select(x => x.ToString("D" + options.Length))
            .Where(x => options.AllowDuplicate || !x.ContainsDuplicate())
            .ToList();
    }

    public string GetNextAnswer()
    {
        string answer;
        switch (_answers.Count)
        {
            case 0:
                answer = "1234";
                break;
            case 1:
                answer = "2379";
                break;
            default:
                do 
                {
                    answer = _suggests[RandomUtil.GetRandomNum(0, _suggests.Count)];
                }
                while(_answers.Contains(answer));
                break;
        }
        _answers.Add(answer);
        return answer;
    }

    public void Return(Result result)
    {
        string answer = _answers.Last();
        int rightNum = result.A + result.B;
        _suggests = _suggests
            .Where(suggest => 
            {
                if (result.A == 0)
                {
                    if (result.B == 0)
                    {
                        if(suggest.Any(x=> answer.Contains(x)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < suggest.Length; i++)
                        {
                            if (suggest[i] == answer[i])
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    var r2 = Game.Check(suggest, answer);
                    //在位的数字不能少于
                    if (r2.A < result.A)
                        return false;
                    var ab = r2.A + r2.B;
                    if (ab != rightNum)
                        return false;
                }
                return true;
            })
            .ToList();
        Console.WriteLine($"剩余备选项：{_suggests.Count}");
    }
}
