using UnityEngine;
using System.Collections;

namespace MyData
{
    public struct DoubleKey
    {
        public readonly object key1;
        public readonly object key2;

        public DoubleKey(object key1_, object key2_)
        {
            key1 = key1_;
            key2 = key2_;
        }
    }

    public abstract class BData
    {
        public abstract object GetKey();

    }

}
