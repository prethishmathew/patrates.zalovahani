using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace patrates.zalohovani.Model
{
    public class Devices
    {
        public string Name { get; set; }
        public List<CloudWPFItem> cloudfiles { get; set; }
    }
}
