using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.BullsAndCows;

public struct Result
{
    public int A { get; set; }
    public int B { get; set; }

    public override string ToString()
    {
        return $"{A}A{B}B";
    }
}
