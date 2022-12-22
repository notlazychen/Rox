using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions;
public static class RandomUtil
{
    private readonly static Random rnd = new Random(DateTime.Now.Millisecond);

    public static int GetRandomNum(int min, int max)
    {
        return rnd.Next(min, max);
    }
}