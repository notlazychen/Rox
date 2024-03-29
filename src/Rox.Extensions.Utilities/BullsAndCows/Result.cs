﻿using System;
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

    public static Result Parse(string result)
    {
        int a = int.Parse(result[0].ToString());
        int b = int.Parse(result[2].ToString());
        return new Result() { A= a, B = b }; 
    }
}
