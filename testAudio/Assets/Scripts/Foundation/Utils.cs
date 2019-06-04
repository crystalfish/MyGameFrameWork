using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMON
{
    /// <summary>
    /// 对字符串的拓展
    /// </summary>
    public static class StringExt
    {
        public static byte HashString(this string str)
        {
            if (str.IsNullOrEmpty()) { return 0; }

            byte rt = 0;

            for (int i = 0, n = str.Length; i < n; ++i)
            {
                char c = str[i];
                rt ^= (byte)c;
            }
            return rt;
        }

        /// <summary>
        /// 拆分字符串成Int数组此方法产生GC 只能在Edtior使用
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static List<int> SplitToIntList(this string str, char sign)
        {
            List<int> ret = new List<int>();
            if (str.Filled())
            {
                var vec = str.Split(sign);
                int tmp;
                for (int i = 0, n = vec.Length; i < n; ++i)
                {
                    if (int.TryParse(vec[i], out tmp))
                    {
                        ret.Add(tmp);
                    }
                    else
                    {
                        COMMON.Logger.LogError("转换int列表失败: {0}.", str);
                    }
                }
            }
            return ret;
        }
        //static public bool IsNullOrEmpty(this string str)
        //{
        //    return string.IsNullOrEmpty(str);
        //}
        private static int[] sTmpList = new int[64];
        private static int sTop = 0;

        public static void SplitToIntList(this string str, char sign, List<int> L)
        {
            L.Clear();
            sTop = 0;
            for (int i = 0, n = str.Length; i < n; ++i)
            {
                var c = str[i];
                if (c != sign)
                {
                    sTmpList[sTop] = c;
                    ++sTop;
                }//if
                else
                {
                    if (sTop > 0)
                    {
                        L.Add(GetInt());
                    }
                }//else
            }//for

            if (sTop > 0)
            {
                L.Add(GetInt());
            }
        }//SplitToIntList

        private static int GetInt()
        {
            int ret = 0;
            for (int i = 0, n = sTop; i < n; ++i)
            {
                ret *= 10;
                ret += sTmpList[i];
            }//for
            sTop = 0;
            return ret;
        }

        /// <summary>
        /// 拆分字符串成Int数组此方法产生GC 只能在Edtior使用
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static List<int> SplitToIntList(this string str)
        {
            return SplitToIntList(str, ',');
        }

        /// <summary>
        /// 获取某个字符串标志以后的文本内容，不包含sign的内容
        /// 如果  "aaaBCD".StringAfter("aaa")得到"BCD"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string StringAfter(this string str, string sign)
        {
            string ret = null;
            var i = str.IndexOf(sign);
            if (i < 0)
            {
                ret = str;
            }
            else
            {
                ret = str.Substring(i + sign.Length);
            }
            return ret;
        }
        /// <summary>
        /// 获取某个字符串标志以后的文本内容，包含sign的内容
        /// 如果 "abcd".StringFrom("b")得到"bcd"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string StringFrom(this string str, string sign)
        {
            string ret = null;
            var i = str.IndexOf(sign);
            if (i < 0)
            {
                ret = str;
            }
            else
            {
                ret = str.Substring(i);
            }
            return ret;
        }
        /// <summary>
        /// 得到某个字符串以前的文本内容，不包含sign的内容
        /// 如果 "ABCD".BeforeAt("D")得到"ABC"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string BeforeAt(this string str, string sign)
        {
            string ret = null;
            var i = str.LastIndexOf(sign);
            if (i < 0)
            {
                ret = str;
            }
            else
            {
                ret = str.Substring(0, i);
            }
            return ret;
        }
        /// <summary>
        /// 得到某个字符串以前的文本内容，包含sign的内容
        /// 如果 "ABCDE".BeforTo("C")得到"ABC"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string BeforeTo(this string str, string sign)
        {
            string ret = null;
            var i = str.LastIndexOf(sign);
            if (i < 0)
            {
                ret = str;
            }
            else
            {
                ret = str.Substring(0, i + sign.Length);
            }
            return ret;
        }

        /// <summary>
        /// 得到某个字符串以前的文本内容，不包含sign的内容
        /// 如果 "ABCDE".SubStrAt("C")得到"DE"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string SubStrAt(this string str, string sign)
        {
            string ret = null;
            var i = str.LastIndexOf(sign) + 1;
            if (i < 0)
            {
                ret = str;
            }
            else
            {
                ret = str.Substring(i, str.Length - i);
            }
            return ret;
        }

        /// <summary>
        /// 返回两个字符之间的字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startSign"></param>
        /// <param name="endSign"></param>
        /// <returns></returns>
        public static string BtweenSign(this string str, string startSign, string endSign)
        {
            return str.Replace('\\', '/').StringAfter(startSign).BeforeAt(endSign);
        }
        /// <summary>
        /// 判断字符串是否已经填充了
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Filled(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool Filled2(this string str)
        {
            return Filled(str) && str != "0";
        }

        public static bool Exist(this System.Object obj)
        {
            return null != obj;
        }

        public static T SafeRelease<T>(this T obj) where T : class, System.IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
            }
            return null;
        }




    }//end class StringExt
    public static class DisposeExt
    {
        public static T TryDispose<T>(this T t) where T : class
        {
            if (null == t)
            {
                return null;
            }
            if (t is System.IDisposable)
            {
                var d = t as System.IDisposable;
                d.Dispose();
            }
            return null;
        }
    }

}


