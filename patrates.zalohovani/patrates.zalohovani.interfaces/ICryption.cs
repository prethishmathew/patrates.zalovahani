using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.interfaces
{
    public interface ICryption
    {
        string encrypt(string data);
        string decrypt(string data);
        string key { set;  }
    }
}
