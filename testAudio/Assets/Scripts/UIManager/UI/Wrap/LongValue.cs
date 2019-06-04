using UnityEngine;
using System.Collections.Generic;
using System;

namespace COMMON
{
    /// <summary>
    /// 避免装箱和拆箱
    /// </summary>
    public class LongValue : IEquatable<LongValue>
    {

        public struct Cmp : IEqualityComparer<LongValue>
        {
            public bool Equals(LongValue x, LongValue y)
            {
                return x.value == y.value;
            }

            public int GetHashCode(LongValue obj)
            {
                return (int)obj.value;
            }
        }
        /// <summary>
        /// 构造字典的时候必须传递这个匹配器
        /// </summary>
        public static readonly Cmp DefaultCmp = new Cmp();
        /// <summary>
        /// 数值
        /// </summary>
        public long value;
        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LongValue other)
        {
            return this.value == other.value;
        }
        /// <summary>
        /// 将数据转化成int
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator long(LongValue i)
        {
            return i.value;
        }

    }//end class


    public class LongValuePool:XPool<LongValue>
    {
        public LongValue GetNode(long val)
        {
            var ret = Get();
            ret.value = val;

            return ret;
        }
    }
}//end namespace

