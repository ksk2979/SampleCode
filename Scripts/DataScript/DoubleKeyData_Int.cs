using UnityEngine;
using System.Collections;

namespace MyData
{
    public abstract class DoubleKeyData_Int : BData
    {
        public int id1;
        public int id2;

        public override object GetKey()
        {
            return new DoubleKey(id1, id2);
        }

    }

}
