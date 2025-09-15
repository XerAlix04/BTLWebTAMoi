using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class LuaChon
{
    public int Ma { get; set; }

    public int? MaCau { get; set; }

    public string NoiDung { get; set; } = null!;

    public bool LaDapAn { get; set; }

    public virtual CauHoi? MaCauNavigation { get; set; }
}
