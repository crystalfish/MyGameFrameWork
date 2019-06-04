
using System.Collections.Generic;
using System;

namespace COMMON
{
    /// <summary>
    /// 更新的对象
    /// </summary>
    public interface IUpdateItem
    {
        bool OnUpdate(float t);
    }
    /// <summary>
    /// 逻辑层的更新逻辑
    /// </summary>
    public interface IUpdateItemLogic
    {
        void OnUpdate(float t);
    }



    /// <summary>
    /// 用来驱动超时处理的数据结构
    /// </summary>
    public class TimeoutInfo
    {
#if dev
        /// <summary>
        /// 不允许XTimer以外的地方修改!!!
        /// </summary>
        public int Seed { get; private set; }
#else
        public int Seed ;
#endif
        public void SetSeed(int seed)
        {
            Seed = seed;
        }//SetSeed


        public XTimer mTimer;
        
        public float mElapsedTime { get; private set; }

        public void SetElapsedTime(float val)
        {
            mElapsedTime = val;
        }
        /// <summary>
        /// 总共的时间 
        /// </summary>
        public float mTotalTime { get; private set; }

        public void SetTotalTime(float t)
        {
            mTotalTime = t;
        }
        /// <summary>
        /// 被回调的方法
        /// </summary>
        public Action<TimeoutInfo> mCallback { get; private set; }

        public void SetCallback(Action<TimeoutInfo> fn)
        {
            mCallback = fn;
        }
        /// <summary>
        /// 可以寄存一个整数
        /// </summary>
        public int intValue { get; set; }
        /// <summary>
        /// 可以寄存一个浮点数
        /// </summary>
        public double doubleValue { get; set; }
        /// <summary>
        /// 可以寄存一个对象
        /// </summary>
        public object objValue { get; set; }
        /// <summary>
        /// 是否取消计时器
        /// </summary>
        public bool cancel { get; private set; }
        /// <summary>
        /// 取消计时器，不会被回调
        /// </summary>
        public TimeoutInfo Cancel()
        {
            cancel = true;
            return null;
        }

        /// <summary>
        /// 停止计时器，并重新开始
        /// </summary>
        public void ReStart()
        {
            Cancel();

            mTimer.SetTimeout(mCallback, mTotalTime);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public TimeoutInfo()
        {

        }

        /// <summary>
        /// 强制完成，会被回调
        /// </summary>
        public void ForceFinished()
        {
            //mElapsedTime = mTotalTime;

            cancel = true;
            if (null != mCallback)
            {
                mCallback(this);
            }
            mCallback = null;
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        private TimeoutInfo Reset()
        {
            //mElapsedTime = 0;
            mElapsedTime = 0;
            mTotalTime = 0;
            intValue = 0;
            doubleValue = 0;
            objValue = null;
            mCallback = null;
            cancel = false;
            return this;
        }

        public class Pool
        {
            private List<TimeoutInfo> mImpl = new List<TimeoutInfo>();

            private int mSeed = 1;

            private bool[] mSet = new bool[1024];

            public TimeoutInfo Get()
            {
                TimeoutInfo tmp = null;
                var n = mImpl.Count;
                if(n>0)
                {
                    tmp = mImpl[n - 1];
                    mImpl.RemoveAt(n - 1);
                }
                else
                {
                    tmp = new TimeoutInfo();
                }
                tmp.SetSeed(mSeed++);
                return tmp;
            }
            public void Put(TimeoutInfo ti)
            {
                
                var seed = ti.Seed;
                if(seed >= mSet.Length)
                {
                    Array.Resize(ref mSet, mSet.Length * 2);
                    mSet[seed] = true;
                }
                else
                {
                    if (mSet[seed])
                    {
                        return;
                    }
                    else
                    {
                        mSet[seed] = true;
                    }
                }
                ti.Reset();
                mImpl.Add(ti);
            }//Put
        }//Pool

    }//end TimeoutInfo


    /// <summary>
    /// 专用的计时器
    /// </summary>
    public class XTimer
    {
        /// <summary>
        /// 用来缩放时间
        /// </summary>
        public float TimeScale = 1f;

        private XFragmentList<Func<float, bool>> mUpdateItems
            = new XFragmentList<Func<float, bool>>();

        private XFragmentList<TimeoutInfo> mTimeoutlist
            = new XFragmentList<TimeoutInfo>();

        class Timeoutlist_Comparer : IComparer<double>
        {
            public static Timeoutlist_Comparer Get()
            {
                if (null == sIns)
                {
                    sIns = new Timeoutlist_Comparer();
                }
                return sIns;
            }

            private static Timeoutlist_Comparer sIns;

            private Timeoutlist_Comparer() { }
            public int Compare(double x, double y)
            {
                if (x < y)
                    return 1;
                else
                    return -1;
            }//Compare
        }//Timeoutlist_Comparer

        private TimeoutInfo.Pool mTimeoutInfoPool = new TimeoutInfo.Pool();

        /// <summary>
        /// 驱动计时器更新
        /// </summary>
        /// <param name="t"></param>
        public void Tick(float t)
        {
            var rt = t * TimeScale;
#region 更新Update列表

            var L = mUpdateItems;
            for (int i = 0; i < L.Count; ++i)
            {
                var fn = L[i];
                if (null == fn) { continue; }
                var finished = fn(rt);
                if (finished)
                {
                    mUpdateItems.RemoveAt(i);
                }
            }
            if(L.DeleteCount>10)
            {
                L.Flush();
            }
            
#endregion

#region 更新倒计时列表

            for (int i = 0;i< mTimeoutlist.Count; ++i)
            {
                var ti = mTimeoutlist[i];
                if(null == ti) { continue; }
                if (ti.cancel)
                {
                    mTimeoutlist.RemoveAt(i);
                    mTimeoutInfoPool.Put(ti);
                }
                else
                {
                    ti.SetElapsedTime(ti.mElapsedTime + rt);
                    if (ti.mElapsedTime >= ti.mTotalTime)
                    {
                        if (null != ti.mCallback)
                        {
                            ti.mCallback(ti);
                        }
                        mTimeoutlist.RemoveAt(i);
                        mTimeoutInfoPool.Put(ti);
                    }
                }
            }
            if(mTimeoutlist.DeleteCount>10)
            {
                mTimeoutlist.Flush();
            }
#endregion
        }

        private float mCachedTime;
        /// <summary>
        /// 添加更新处理方法
        /// </summary>
        /// <param name="fn"></param>
        public void AddUpdateHandler(Func<float, bool> fn)
        {
            mUpdateItems.Add(fn);
        }
        /// <summary>
        /// 移除更新处理方法
        /// </summary>
        /// <param name="fn"></param>
        public void RemoveUpdateHandler(Func<float, bool> fn)
        {
            mUpdateItems.Remove(fn);
        }

        public bool HaveUpdateHandler(Func<float, bool> fn)
        {
            return mUpdateItems.Contains(fn);
        }

        private void RemoveAllUpdateHander()
        {
            mUpdateItems.Clear();
        }


        private void RemoveAllTimeOutHadner()
        {
            mTimeoutlist.Clear();
        }

        public void RemoveAllHander()
        {
            RemoveAllUpdateHander();
            RemoveAllTimeOutHadner();
        }

        /// <summary>
        /// 最小的时间间隔
        /// </summary>
        readonly float MIN_DELTA = (1f / 60f);

        private void SetTimeout(ref TimeoutInfo ti, Action<TimeoutInfo> fn, float delay)
        {
            ti.mTimer = this;
            ti.SetTotalTime(delay);
            ti.SetCallback(fn);
            ti.SetElapsedTime(0F);
            if (delay <= MIN_DELTA)
            {
                fn(ti);
                mTimeoutInfoPool.Put(ti);
            }
            else
            {
                mTimeoutlist.Add(ti);
            }
        }

        /// <summary>
        /// 设置超时处理
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="delay"></param>
        public TimeoutInfo SetTimeout(Action<TimeoutInfo> fn,
            float delay)
        {
            var ti = mTimeoutInfoPool.Get();
            SetTimeout(ref ti, fn, delay);
            return ti;
        }
        /// <summary>
        /// 设置超时处理
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="delay"></param>
        public TimeoutInfo SetTimeoutObj(Action<TimeoutInfo> fn,
            float delay, object vo)
        {
            var ti = mTimeoutInfoPool.Get();
            ti.objValue = vo;
            SetTimeout(ref ti, fn, delay);
            return ti;
        }
        /// <summary>
        /// 设置超时处理
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="delay"></param>
        /// <param name="vo"></param>
        public void SetTimeout(Action<TimeoutInfo> fn,
            float delay, int vo)
        {
            var ti = mTimeoutInfoPool.Get();
            ti.intValue = vo;
            SetTimeout(ref ti, fn, delay);
        }
        /// <summary>
        /// 设置超时处理
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="delay"></param>
        /// <param name="vo"></param>
        public void SetTimeout(Action<TimeoutInfo> fn,
            float delay, float vo)
        {
            var ti = mTimeoutInfoPool.Get();
            ti.doubleValue = vo;
            SetTimeout(ref ti, fn, delay);
        }
        /// <summary>
        /// 设置超时处理
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="delay"></param>
        /// <param name="vo"></param>
        public void SetTimeout(Action<TimeoutInfo> fn,
            float delay, object vo)
        {
            var ti = mTimeoutInfoPool.Get();
            ti.objValue = vo;
            SetTimeout(ref ti, fn, delay);
        }
        /// <summary>
        /// 移除倒计时，这个函数的，开销非常非常大，慎用
        /// </summary>
        /// <param name="fn"></param>
        public void RemoveTimeout(Action<TimeoutInfo> fn)
        {
            for(int i=0,n=mTimeoutlist.Count;i<n;++i)
            {
                var it = mTimeoutlist[i];
                if(null == it) { continue; }
                if(it.mCallback == fn)
                {
                    it.Cancel();
                }
            }//for

        }//RemoveTimeout
    }//end class XTimer


}//end namespace


