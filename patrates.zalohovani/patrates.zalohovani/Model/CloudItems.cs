using patrates.zalohovani.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.Model
{
    public class CloudItems
    {
        public cloudTypes cloudtype { get; set; }
        public List<Devices> devices { get; set; } 
    }
}
