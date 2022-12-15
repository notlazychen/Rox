using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.Autolink;

/* 协议格式:
 */
public class AutolinkPackage
{
    public string Key { get; set; }

    public int Id { get; set; }
    public long CreateAt { get; set; }
    public string RpcTiger { get; set; }
    public object Body { get; set; }

}
