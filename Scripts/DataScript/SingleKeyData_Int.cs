using UnityEngine;
using System.Collections;

namespace MyData
{
    public abstract class SingleKeyData_Int : BData
    {
        public int nId;

        public override object GetKey()
        {
            return nId;
        }
    }

}
