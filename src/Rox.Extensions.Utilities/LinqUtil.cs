using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions;

public static class LinqUtil
{
    public static bool ContainsDuplicate<T>(this IEnumerable<T> source)
    {
        int index = 0;
        foreach(var item in source)
        {
            index++;
            foreach (var item2 in source.Skip(index))
            {
                if(item.Equals(item2))
                {
                    return true;
                }
            }
        }
        return false;
    }
}