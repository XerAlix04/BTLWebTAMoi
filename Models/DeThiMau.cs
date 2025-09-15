using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class DeThiMau
{
    public int MaDe { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? Loai { get; set; }

    public virtual ICollection<KetQuaBaiThi> KetQuaBaiThis { get; set; } = new List<KetQuaBaiThi>();

    public virtual ICollection<CauHoi> MaCaus { get; set; } = new List<CauHoi>();
}
