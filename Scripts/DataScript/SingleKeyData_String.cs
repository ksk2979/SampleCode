using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyData
{
    public class SingleKeyData_String : BData
    {
        public string nId;

        public override object GetKey()
        {
            return nId;
        }
    }
}
