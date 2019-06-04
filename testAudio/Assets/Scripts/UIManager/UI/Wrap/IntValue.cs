using UnityEngine;
using System.Collections.Generic;
using System;

namespace COMMON
{
    /// <summary>
    /// 避免装箱和拆箱
    /// </summary>
    public class IntValue : IEquatable<IntValue>
    {

        public struct Cmp : IEqualityComparer<IntValue>
        {
            public bool Equals(IntValue x, IntValue y)
            {
                return x.value == y.value;
            }

            public int GetHashCode(IntValue obj)
            {
                return obj.value;
            }
        }
        /// <summary>
        /// 构造字典的时候必须传递这个匹配器
        /// </summary>
        public static readonly Cmp DefaultCmp = new Cmp();
        /// <summary>
        /// 数值
        /// </summary>
        public int value;
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntValue other)
        {
            return this.value == other.value;
        }
        /// <summary>
        /// 将数据转化成int
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator int(IntValue i)
        {
            return i.value;
        }

    }//end class

    public class IntValuePool : XPool<IntValue>
    {
        public IntValue GetNode(int val)
        {
            var iv = Get();
            iv.value = val;
            return iv;
        }
    }
}//end namespace

