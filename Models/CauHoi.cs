using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class CauHoi
{
    public int MaCau { get; set; }

    public int? MaDoan { get; set; }

    public string NoiDung { get; set; } = null!;

    public virtual ICollection<LuaChon> LuaChons { get; set; } = new List<LuaChon>();

    public virtual DoanVan? MaDoanNavigation { get; set; }

    public virtual ICollection<DeThiMau> MaDes { get; set; } = new List<DeThiMau>();
}
