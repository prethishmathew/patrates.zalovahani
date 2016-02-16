using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.commons
{
    public class noencryption : ICryption

    {
        
        public string encrypt(string data)
        {
            return data;
        }

        public string decrypt(string data)
        {
            return data;
        }

        private string _key = "ZALASO4567IKDSDA";
        
        public string key {
            
            get { return _key; }
            set { _key = value; }
        }
    }
}
