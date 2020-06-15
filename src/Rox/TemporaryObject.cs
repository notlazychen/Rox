using System;
using System.Collections.Generic;
using System.Text;

namespace Rox
{
    public class TemporaryObject<T>
    {
        public T Object { get; set; }
        public bool IsValid { get; set; }
    }
}
