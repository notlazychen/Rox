using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.BullsAndCows;

public class GameOptions
{
    /// <summary>
    /// 数字长度
    /// </summary>
    public int Length { get; private set; }
    /// <summary>
    /// 是否允许重复
    /// </summary>
    public bool AllowDuplicate { get; private set; }

    public GameOptions()
    {
        Length = 4;
        AllowDuplicate = false;
    }
}
