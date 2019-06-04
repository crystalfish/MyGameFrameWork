using System.Collections.Generic;
using System;
using UnityEngine;

namespace COMMON
{
    /// <summary>
    /// 避免装箱
    /// </summary>
    public class FloatValue : IEquatable<FloatValue>
    {
        /// <summary>
        /// 定制的匹配器可以提升性能
        /// </summary>
        public struct Cmp : IEqualityComparer<FloatValue>
        {
            public bool Equals(FloatValue x, FloatValue y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(FloatValue obj)
            {
                return (int)obj.value;
            }
        }
        /// <summary>
        /// 如果用这个对象作为Dictionary的key，则必须使用这个匹配器
        /// </summary>
        public static readonly Cmp DefaultCmp = new Cmp();
        /// <summary>
        /// 数值
        /// </summary>
        public float value;
        /// <summary>
        /// 浮点数比较不能直接用==
        /// </summary>
        private const float kEpsilon = Vector2.kEpsilon;
        /// <summary>
        /// 匹配值是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FloatValue other)
        {
            float delta = this.value - other.value;
            if(delta > 0f)
            {
                return delta <= kEpsilon;
            }
            else
            {
                return -delta <= kEpsilon;
            }
        }
        /// <summary>
        /// 将数据转化成float
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator float(FloatValue f)
        {
            return f.value;
        }
    }//end class
}//end namespace

