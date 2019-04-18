#define TRACKING_MAX//追踪最大值

#if (!DEBUG)
#undef TRACKING_MAX//在非debug模式下关闭
#endif

using System.Collections.Generic;
using System;
using System.Collections;

namespace COMMON
{


//    /// <summary>
//    /// 这个类用追踪在战斗场景中的每个对象的阈值
//    /// </summary>
//    public class PoolTrackingInfo
//    {
//#if TRACKING_MAX
//        public enum E_Type
//        {
//            SKILL_ENTITY,
//            AUDIO,
//            ACTION
//        }

//        public E_Type mType;

//        public int Count;
//        public string mTypeName;

//        public XDictionary<string, int> mMap;// = new XDictionary<string, int>();

//        public List<SkillEntity> mSkills;

//        public List<AudioObject> mAudios;
//        /// <summary>
//        /// 打印信息
//        /// </summary>
//        /// <param name="builder"></param>
//        public void Print(System.Text.StringBuilder builder)
//        {
//            if(mType == E_Type.ACTION)
//            {
//                if(Count>0)
//                {
//                    builder.Append("Action:").Append(mTypeName)
//                   .Append("  max:").Append(Count).AppendLine();
//                }
//            }
//            else
//            {
//                Calculate();
//                var L = mMap.GetList();
//                if (mType == E_Type.AUDIO)
//                {
//                    builder.Append("Audio:").AppendLine();
//                }
//                else if(mType == E_Type.SKILL_ENTITY)
//                {
//                    builder.Append("SKillEntity:").AppendLine();
//                }
//                for (int i = 0, n= L.Count; i < n;++i)
//                {
//                    var kv = L[i];
//                    if (kv.Value == 0)
//                    {
//                        continue;
//                    }
//                    builder.Append("res:").Append(kv.Key).Append("  max:")
//                        .Append(kv.Value).AppendLine();
//                }//for
//            }//else
//        }//Print
//        /// <summary>
//        /// 计算各种特效和声音的数量
//        /// </summary>
//        private void Calculate()
//        {
//            if(mType == E_Type.AUDIO)
//            {
//                int N = 0;
//                for(int i=0,n=mAudios.Count;i<n;++i)
//                {
//                    var it = mAudios[i];
//                    var k = it.mRes;
//                    if(!k.Filled())
//                    {
//                        continue;
//                    }
//                    if(mMap.TryGetValue(k,out N))
//                    {
//                        mMap[k] = N + 1;
//                    }
//                    else
//                    {
//                        mMap.Add(k, 1);
//                    }
//                }//for
//            }//if
//            if(mType == E_Type.SKILL_ENTITY)
//            {
//                int N = 0;
//                for (int i=0,n=mSkills.Count;i<n;++i)
//                {
//                    var it = mSkills[i];
//                    var k = it.resName;
//                    if (mMap.TryGetValue(k, out N))
//                    {
//                        mMap[k] = N + 1;
//                    }
//                    else
//                    {
//                        mMap.Add(k, 1);
//                    }
//                }//for
//            }//if
//        }//Calculate

//        public PoolTrackingInfo(E_Type type,string typeName)
//        {
//            mList.Add(this);
//            mType = type;
//            mTypeName = typeName;
//            if(type == E_Type.AUDIO)
//            {
//                mMap = new XDictionary<string, int>();
//                mAudios = new List<AudioObject>();
//            }//if
//            else if(type == E_Type.SKILL_ENTITY)
//            {
//                mMap = new XDictionary<string, int>();
//                mSkills = new List<SkillEntity>();
//            }
//        }//PoolTrackingInfo

//        public void TrackSkill(SkillEntity obj)
//        {
//            var res = obj.resName;
//            mSkills.Add(obj);
//        }//Track
//        public void TrackAudio(AudioObject obj)
//        {
//            var res = obj.mRes;
//            mAudios.Add(obj);
//        }//Track

//        public void TrackAction(BaseAction act)
//        {
//            ++Count;
//        }

//        public void Track(System.Object obj)
//        {
//            if(mType == E_Type.SKILL_ENTITY)
//            {
//                TrackSkill(obj as SkillEntity);
//            }
//            else if(mType == E_Type.AUDIO)
//            {
//                TrackAudio(obj as AudioObject);
//            }
//            else if(mType == E_Type.ACTION)
//            {
//                TrackAction(obj as BaseAction);
//            }
//        }

//        private static List<PoolTrackingInfo> mList = new List<PoolTrackingInfo>();
//#endif//TRACKING_MAX

//#if DEBUG
//        /// <summary>
//        /// 打印信息到控制台
//        /// </summary>
//        public static void PrintMsg()
//        {
//            var builder = new System.Text.StringBuilder();
//            for(int i=0,n=mList.Count;i<n;++i)
//            {
//                mList[i].Print(builder);
//            }//for
//            Logger.Log(builder.ToString());
//        }//Print

//        public static void Clear()
//        {
//            for (int i = 0, n = mList.Count; i < n; ++i)
//            {
//                var it = mList[i];
//                it.Count = 0;
//                if(null != it.mMap)
//                {
//                    it.mMap.Clear();
//                }//if
//                if(null != it.mSkills)
//                {
//                    it.mSkills.Clear();
//                }
//                if(null != it.mAudios)
//                {
//                    it.mAudios.Clear();
//                }
//            }//for
//        }
//#endif
//    }//PoolTrackingInfo



    public interface IPool
    {
        object GetX();
        void PutX(object item);

        void Clear();
        ///// <summary>
        ///// 自动清零
        ///// </summary>
        //void AutoCleanup();
        /// <summary>
        /// 在AutoCleanup的时候是否自动被清理
        /// </summary>
        bool RemoveWhenAutoCleanup { get; }
        /// <summary>
        /// 设置在被清理的时候自动Cleanup
        /// </summary>
        IPool SetRemoveWhenAutoCleanup();
        /// <summary>
        /// 
        /// </summary>
        void MakeReserveItems(int count);
    }

    public class PoolManager : List<WeakReference>
    {
        public readonly static PoolManager sInstance = new PoolManager();

        public void CleanCached()
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                var it = this[i];
                //将弱引用转换为强引用才可以使用，相当于lock
                IPool pool = it.Target as IPool;
                if (null!=pool)
                {
                    //void
                }
                else
                {
                    RemoveAt(i);
                    continue;
                }
                pool.Clear();
                if (pool.RemoveWhenAutoCleanup)
                {
                    RemoveAt(i);
                }//if
            }//for
        }//CleanCached
        /// <summary>
        /// 添加到自动移除列表中
        /// </summary>
        /// <param name="pool"></param>
        public static void AddToAutoCleanUpList(IPool pool)
        {
            sInstance.Add(new WeakReference(pool));
        }//AddToAutoCleanUpList
    }//PoolManager


    /// <summary>
    /// 对象池
    /// </summary>
    public class XPool<T> : IEnumerable<T>, IDisposable,IPool
        where T : class, new()
    {
        /// <summary>
        /// 在AutoCleanup的时候是否自动被清理
        /// </summary>
        bool IPool.RemoveWhenAutoCleanup { get { return mRemoveWhenAutoCleanup; } }
        /// <summary>
        /// 设置在被清理的时候自动Cleanup
        /// </summary>
        IPool IPool.SetRemoveWhenAutoCleanup()
        {
            mRemoveWhenAutoCleanup = true;
            return this;
        }
        /// <summary>
        /// 设置成在AutoCleanUp的时候从PoolManager中移除以
        /// 消除强引用
        /// </summary>
        /// <returns></returns>
        public XPool<T> SetRemoveWhenAutoCleanup()
        {
            mRemoveWhenAutoCleanup = true;
            return this;
        }//SetRemoveWhenAutoCleanup

        /// <summary>
        /// 标记是否在AutoCleanup的时候从管理器里面移除
        /// </summary>
        private bool mRemoveWhenAutoCleanup;

        private HashSet<T> mSet;
        /// <summary>
        /// 默认的初始化容量
        /// </summary>
        private const int DEFAULT_SIZE = 4;
        /// <summary>
        /// 存放数据的容器
        /// </summary>
        public List<T> mImpl;
        /// <summary>
        /// 标记是否已经销毁过了
        /// </summary>
        private bool mDisposed = false;

        ///// <summary>
        ///// 切换战斗场景的时候被自动清零
        ///// </summary>
        //void IPool.AutoCleanup()
        //{
        //    PoolManager.AddToAutoCleanUpList(this);
        //}
        ///// <summary>
        ///// 切换战斗场景的时候被自动清零
        ///// </summary>
        ///// <returns></returns>
        //public XPool<T> AutoCleanup()
        //{
        //    PoolManager.AddToAutoCleanUpList(this);
        //    return this;
        //}

        /// <summary>
        /// 销毁资源
        /// </summary>
        public void Dispose()
        {
            if (!mDisposed)
            {
                mDisposed = true;
            }
            Clear();
            mCreator = null;//释放资源
            mPutHandler = null;//释放强引用
            mGetHandler = null;//释放强引用
        }//Dispose
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="capacity"></param>
        public XPool(int capacity) 
        {
            Init(null, capacity);
        }
        /// <summary>
        /// 根据构造器来构造
        /// </summary>
        /// <param name="fnCreator"></param>
        public XPool(Func<T> fnCreator)
        {
            Init(fnCreator, DEFAULT_SIZE);
        }
        /// <summary>
        /// 不带任何参数的构造方法
        /// </summary>
        public XPool()
        {
            Init(null, DEFAULT_SIZE);
        }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="fnCreator"></param>
        /// <param name="capacity"></param>
        private void Init(Func<T> fnCreator, int capacity)
        {
            mImpl = new List<T>(capacity);
            mSet = new HashSet<T>();
            mCreator = fnCreator;
            mIsIDisposable = typeof(T).IsAssignableFrom(typeof(IDisposable));
            mIsIReleaseable = typeof(T).IsAssignableFrom(typeof(IReleaseable));

//#if TRACKING_MAX
//            var t = typeof(T);
//            if(t.IsSubclassOf(typeof(BaseAction)))
//            {
//                mTrackingInfo = new PoolTrackingInfo(PoolTrackingInfo.E_Type.ACTION, t.Name);
//            }//if
//            else if(t==typeof(SkillEntity))
//            {
//                mTrackingInfo = new PoolTrackingInfo(PoolTrackingInfo.E_Type.SKILL_ENTITY, t.Name);
//            }//else if
//            else if(t==typeof(AudioObject))
//            {
//                mTrackingInfo = new PoolTrackingInfo(PoolTrackingInfo.E_Type.AUDIO, t.Name);
//            }//else
//#endif//TRACKING_MAX
        }

//#if TRACKING_MAX
//        PoolTrackingInfo mTrackingInfo;
//#endif//TRACKING_MAX


        /// <summary>
        /// 根据构造器和初始容量来构造对象
        /// </summary>
        /// <param name="fnCreator"></param>
        /// <param name="capacity"></param>
        public XPool(Func<T> fnCreator, int capacity)
        {
            Init(fnCreator, capacity);
        }
        /// <summary>f
        /// 用来构造对象的delegate
        /// </summary>
        public Func<T> mCreator = null;
        /// <summary>
        /// 获取的时候的处理器
        /// </summary>
        public Action<T> mGetHandler = null;
        /// <summary>
        /// 放回去的时候的处理器
        /// </summary>
        public Action<T> mPutHandler = null;
        /// <summary>
        /// 类型是否为IReleaseable
        /// </summary>
        private bool mIsIReleaseable = false;
        /// <summary>
        /// 类型是否为IDisposable
        /// </summary>
        private bool mIsIDisposable = false;
        /// <summary>
        /// 清除全部的缓存对象
        /// </summary>
        public void Clear()
        {
            mSet.Clear();
            if(mIsIReleaseable)
            {
                for(int i=0,n=mImpl.Count;i<n;++i)
                {
                    var it = mImpl[i];
                    var r = it as IReleaseable;
                    if (null != r)
                    {
                        r.Release();
                    }//if
                }
            }//
            else if(mIsIDisposable)
            {
                for (int i = 0, n = mImpl.Count; i < n; ++i)
                {
                    var it = mImpl[i];
                    var r = it as IDisposable;
                    if (null != r)
                    {
                        r.Dispose();
                    }//if
                }
            }//else
            mImpl.Clear();
        }//Clear
        /// <summary>
        /// 只做引用清理工作
        /// </summary>
        public void ClearReffOnly()
        {
            if(null != mImpl)
            {
                mImpl.Clear();
            }
            if(null!= mSet)
            {
                mSet.Clear();
            }
        }//ClearReffOnly
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return mImpl.GetEnumerator();
        }
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mImpl.GetEnumerator();
        }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T ret = default(T);
            if(0 == mImpl.Count)
            {
                if(null != mCreator)
                {
                    ret = mCreator();
                }
                else
                {
                    ret = new T();
                }
//#if TRACKING_MAX
//                if(null != mTrackingInfo)
//                {
//                    mTrackingInfo.Track(ret);
//                }
//#endif//TRACKING_MAX
            }
            else
            {
                var n = mImpl.Count - 1;
                ret = mImpl[n];
                mImpl.RemoveAt(n);
                if(ret != null && mSet.Contains(ret))
                {
                    mSet.Remove(ret);
                }
            }
            if(null != mGetHandler)
            {
                mGetHandler(ret);
            }
            return ret;
        }
        /// <summary>
        /// 将对象放回对象池
        /// </summary>
        /// <param name="item"></param>
        public void Put(T item)
        {
            if(mSet.Contains(item))
            {
                return;
            }
            mSet.Add(item);
            if(null != mPutHandler)
            {
                mPutHandler(item);
            }
            mImpl.Add(item);
        }

        /// <summary>
        /// 对泛化做兼容处理
        /// </summary>
        /// <param name="item"></param>
        public void PutX(object item)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if(!(item is T))
            {
                Logger.LogError("type not matched! class:{0}",item.GetType().FullName);
            }
#endif//platform
            Put(item as T);
        }
        /// <summary>
        /// 对泛化做兼容处理
        /// </summary>
        /// <returns></returns>
        public object GetX()
        {
            return Get();
        }
        /// <summary>
        /// 创建若干个对象
        /// </summary>
        /// <param name="capacity"></param>
        public XPool<T> MakeReserve(int capacity)
        {
            capacity = capacity - mImpl.Count;
            for(var i = 0; i < capacity;++i)
            {
                T it = null; ;
                if(null == mCreator)
                {
                    it = new T();
                }
                else
                {
                    it = mCreator();
                }
                mImpl.Add(it);
            }
            return this;
        }

        void IPool.MakeReserveItems(int count)
        {
            MakeReserve(count);
        }//MakeReserve
    }//end class
}//end namespace

