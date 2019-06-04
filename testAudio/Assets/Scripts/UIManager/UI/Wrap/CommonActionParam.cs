#if DEBUG
#define CommonActionParam_Type_Detection
#endif

using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;


namespace COMMON
{
    /// <summary>
    /// 作为通用的参数配置
    /// </summary>
    public struct CommonActionParam : IEquatable<CommonActionParam>, IComparable<CommonActionParam>
    {
        public string DebugInfo()
        {
            string str = "CommonActionParam :";
#if CommonActionParam_Type_Detection
            str = type.ToString() + ",";
            switch (type)
            {
                case CommonActionParam_Type.None:
                    break;
                case CommonActionParam_Type.Object:
                    break;
                case CommonActionParam_Type.Int:
                    int i = param;
                    str += i;
                    break;
             
                case CommonActionParam_Type.Bool:
                    bool b = param;
                    str += b;
                    break;
            
            }
#endif
            return str;
        }

#if CommonActionParam_Type_Detection
        public enum CommonActionParam_Type
        {
            None,
            Object,
            Int,
            Bool,
        }

        private CommonActionParam_Type type;
#endif

        private CommonActionParam2 param;

        private object o;
        public object O
        {
            get
            {
#if CommonActionParam_Type_Detection
                if (type != CommonActionParam_Type.Object)
                {
                    Logger.LogError("TypeError Parse Object-----Type " + type);
                }
#endif
                return o;
            }
            set
            {
                o = value;

#if CommonActionParam_Type_Detection
                type = CommonActionParam_Type.Object;
#endif

            }
        }

#if UNITY_EDITOR
        public CommonActionParam_Type GetEnumType()
        {
            return type;
        }
#endif

        public int CompareTo(CommonActionParam other)
        {
            if (Equals(other))
            {
                return 0;
            }
            else
            {
                return GetHashCode() - other.GetHashCode();
            }
        }
        public override int GetHashCode()
        {
            if (o != null)
            {
                //为避免GetHashCode可能产生的不同步，直接返回0
                return 0;

                //return o.GetHashCode();
            }
            else
            {
                return param.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CommonActionParam)
            {
                var other = (CommonActionParam)obj;
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(CommonActionParam other)
        {
            if (O == null && other.O == null)
            {
                return param.Equals(other.param);
            }
            else
            {
                return O == other.O;
            }
        }

       


        /// <summary>
        /// 将数据转化成int
        /// </summary>
        public static implicit operator int(CommonActionParam p)
        {
#if CommonActionParam_Type_Detection
            if (p.type != CommonActionParam_Type.Int)
            {
                Logger.LogError("TypeError Parse int-----Type " + p.type);
            }
#endif
            return p.param;
        }

        /// <summary>
        /// 将数据转化成bool
        /// </summary>
        public static implicit operator bool(CommonActionParam p)
        {
#if CommonActionParam_Type_Detection
            if (p.type != CommonActionParam_Type.Bool)
            {
                Logger.LogError("TypeError Parse bool-----Type " + p.type);
            }
#endif
            return p.param;
        }

     


        /// <summary>
        /// 将int变量转化为数据
        /// </summary>
        public static implicit operator CommonActionParam(int i)
        {
            var ret = new CommonActionParam();
            ret.param = i;

#if CommonActionParam_Type_Detection
            ret.type = CommonActionParam_Type.Int;
#endif

            return ret;
        }

       

        /// <summary>
        /// 将bool变量转化为数据
        /// </summary>
        public static implicit operator CommonActionParam(bool b)
        {
            var ret = new CommonActionParam();
            ret.param = b;

#if CommonActionParam_Type_Detection
            ret.type = CommonActionParam_Type.Bool;
#endif

            return ret;
        }

      

     
    }//end class CommonActionParam

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct CommonActionParam2 : IEquatable<CommonActionParam2>, IComparable<CommonActionParam2>
    {
        #region 属性
        [FieldOffset(0)]
        private int i;
        public int I
        {
            get
            {
                return i;
            }
            set
            {
                i = value;
            }
        }

       

        [FieldOffset(0)]
        private bool b;
        public bool B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
            }
        }

      

        #endregion


        public int CompareTo(CommonActionParam2 other)
        {
            if (Equals(other))
            {
                return 0;
            }
            else
            {
                return GetHashCode() - other.GetHashCode();
            }
        }
        public override int GetHashCode()
        {
            //TODO 
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is CommonActionParam2)
            {
                var other = (CommonActionParam2)obj;
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(CommonActionParam2 other)
        {
            return true;
        }

   
        /// <summary>
        /// 将数据转化成int
        /// </summary>
        public static implicit operator int(CommonActionParam2 p)
        {
            return p.i;
        }

        /// <summary>
        /// 将数据转化成bool
        /// </summary>
        public static implicit operator bool(CommonActionParam2 p)
        {
            return p.b;
        }

 

        /// <summary>
        /// 将int变量转化为数据
        /// </summary>
        public static implicit operator CommonActionParam2(int i)
        {
            var ret = new CommonActionParam2();
            ret.i = i;
            return ret;
        }

      

        /// <summary>
        /// 将bool变量转化为数据
        /// </summary>
        public static implicit operator CommonActionParam2(bool b)
        {
            var ret = new CommonActionParam2();
            ret.b = b;
            return ret;
        }

    }

}//end namespace


