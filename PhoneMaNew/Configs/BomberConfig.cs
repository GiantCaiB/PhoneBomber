using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneMaNew.Configs
{
    public class BomberConfig
    {
        public ICollection<string> OwnedPhoneNumbers { get; set; }
        public int TotalSorties { get; set; }
        public string BombContent { get; set; }
    }
}
