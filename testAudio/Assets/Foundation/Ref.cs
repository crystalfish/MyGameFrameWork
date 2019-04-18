using UnityEngine;
using System.Collections.Generic;

namespace COMMON
{
    /// <summary>
    /// 可Release对象
    /// </summary>
    public interface IReleaseable
    {
        void Release();
    }
    public class Ref : System.IDisposable
    {
        /// <summary>
        /// 记录下所有持有该对象引用的对象
        /// </summary>
        protected HashSet<System.Object> mOwners = new HashSet<object>();

        /// <summary>
        /// 添加引用计数
        /// </summary>
        /// <param name="owner"></param>
        public void Retain(System.Object owner)
        {
            if(!mOwners.Contains(owner))
            {
                mOwners.Add(owner);
            }
        }
        /// <summary>
        /// 释放引用计数
        /// </summary>
        /// <param name="owner"></param>
        public void Release(System.Object owner)
        {
            if(mOwners.Contains(owner))
            {
                mOwners.Remove(owner);
            }
            if(0 == mOwners.Count)
            {
                Dispose();
            }
        }
        /// <summary>
        /// 彻底释放资源
        /// </summary>
        public virtual void Dispose()
        {

        }
    }//end class
}//end namespace
