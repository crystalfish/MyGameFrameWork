using UnityEngine;
using System.Collections.Generic;
using System;

namespace COMMON
{
    /// <summary>
    /// 避免装箱和拆箱
    /// </summary>
    public class CommonActionParamValue
    {
        /// <summary>
        /// 数值
        /// </summary>
        public CommonActionParam value;

        /// <summary>
        /// 将数据转化成int
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator CommonActionParam(CommonActionParamValue i)
        {
            return i.value;
        }

    }//end class

    public class CommonActionParamValuePool : XPool<CommonActionParamValue>
    {
        public CommonActionParamValue GetNode(int val)
        {
            var iv = Get();
            iv.value = val;
            return iv;
        }
    }
}//end namespace

