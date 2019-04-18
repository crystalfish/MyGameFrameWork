using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace COMMON
{
    public interface ISingleton
    {
        void CleanInstance();

        bool Suspended { get; }

        void SuspendThis(bool b);
    }

    public static class SimpleSingletonList
    {
        public readonly static List<ISingleton> sList = new List<ISingleton>();
        /// <summary>
        /// 简单粗暴地清除所有的静态实例
        /// </summary>
        public static void CleanUp()
        {
            if (sList.Count > 0)
            {
                for (int i = 0, n = sList.Count; i < n; ++i)
                {
                    var it = sList[i];
                    if (it.Suspended)
                    {
                        continue;
                    }
                    if (it is IDisposable)
                    {
                        var d = it as IDisposable;
                        d.Dispose();
                    }
                    it.CleanInstance();
                }//for
                sList.Clear();
            }//if
        }//CleanUp
    }
    /// <summary>
    /// 单例的对象
    /// </summary>
    public class SimpleSingleton<T> : ISingleton where T : class, new()
    {
        /// <summary>
        /// 阻止外部直接实例化
        /// </summary>
        protected SimpleSingleton()
        {
            //SimpleSingletonList.sList.Add(this);
        }//SimpleSingleton
        /// <summary>
        /// 静态实例的引用
        /// </summary>
        protected static T sInstance;
        /// <summary>
        /// 获取静态对象实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == sInstance)
                    sInstance = new T();
                return sInstance;
            }//get
        }//Instance
        /// <summary>
        /// 获取静态实例，如果为null不创建新的实例
        /// </summary>
        /// <returns></returns>
        public static T TryGetInstance()
        {
            return sInstance;
        }

        public bool Suspended
        {
            get
            {
                return mSuspended;
            }
        }
        /// <summary>
        /// 标记是否阻止清理
        /// </summary>
        private bool mSuspended;

        public static bool HasInstance()
        {
            return sInstance != null;
        }//HasInstance

        public void CleanInstance()
        {
            sInstance = null;
        }//CleanInstance

        /// <summary>
        /// 阻止被粗暴地清理静态单例
        /// </summary>
        /// <param name="b"></param>
        public void SuspendThis(bool b)
        {
            mSuspended = b;
        }
    }//end class
}

