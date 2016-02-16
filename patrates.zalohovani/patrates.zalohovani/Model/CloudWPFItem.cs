using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace patrates.zalohovani.Model
{
     public class CloudWPFItem
    {
         public bool isSelected { get; set; }
         public IcloudFileInfo cloudItems { get; set; }
    }
}
