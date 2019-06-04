using System.Collections.Generic;

namespace COMMON
{
    /// <summary>
    /// 当需要传递n个参数来Dispatch事件的时候可以先缓存起来
    /// 然后在收事件的地方从这里获取，如果下面的变量不够用可以自己扩展
    /// 这个在同一个事件周期安全，因为主线程是单线程处理的。
    /// 
    /// author:joi
    /// </summary>
    public static class EventArgumentCache
    {
        public static int sInt;

        public static int sInt2;

    }

    /// <summary>
    /// 事件的参数
    /// </summary>
    public struct EventArguments
    {
        /// <summary>
        /// 派发事件的时候传递的参数
        /// </summary>
        public readonly object param;
        /// <summary>
        /// 事件的派发器
        /// </summary>
        public readonly Notifier sender;
        /// <summary>
        /// 事件的类型
        /// </summary>
        public readonly string type;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="param_"></param>
        /// <param name="sender_"></param>
        /// <param name="type_"></param>
        internal EventArguments(object param_,
            Notifier sender_,
            string type_)
        {
            param = param_;
            sender = sender_;
            type = type_;
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="handler"></param>
        public void Remove(Notifier.Eventhandler handler)
        {
            sender.Remove(type, handler);
        }

        public static readonly EventArguments Default = new EventArguments(null, null, null);
    }
    /// <summary>
    /// 事件派发器，用来监听或者移除或者派发事件
    /// 这个类并非线程安全的，只有在同一个线程中安全
    /// </summary>
    public class Notifier
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public Notifier()
        {

        }//Notifier




        /// <summary>
        /// 标记是否处于托管状态
        /// </summary>
        private bool mManaged;

        /// <summary>
        /// 委托的原型
        /// </summary>
        /// <param name="ea"></param>
        public delegate void Eventhandler(ref EventArguments ea);

        /// <summary>
        /// 事件的实现类
        /// </summary>
        private class EventImpl
        {
            public event Eventhandler mDispatcher;

            /// <summary>
            /// 派发事件
            /// </summary>
            /// <param name="data"></param>
            /// <param name="sender"></param>
            /// <param name="type"></param>
            public void Execute(object data, Notifier sender, string type)
            {
                if (null != mDispatcher)
                {
                    var ea = new EventArguments(data, sender, type);
                    mDispatcher.Invoke(ref ea);
                }
            }
        }

        /// <summary>
        /// type和实现对象的映射表
        /// </summary>
        private Dictionary<string, EventImpl> mMap = new Dictionary<string, EventImpl>();
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void Regist(string type, Eventhandler handler)
        {
            Remove(type, handler);
            if (!mMap.ContainsKey(type))
            {
                mMap[type] = new EventImpl();
            }

            var impl = mMap[type];
            impl.mDispatcher += handler;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void Remove(string type, Eventhandler handler)
        {
            if (mMap.ContainsKey(type))
            {
                var impl = mMap[type];
                impl.mDispatcher -= handler;
            }
        }

        /// <summary>
        /// 根据事件类型来移除全部事件监听器
        /// </summary>
        /// <param name="type"></param>
        public void RemoveAll(string type)
        {
            if (mMap.ContainsKey(type))
            {
                mMap.Remove(type);
            }
        }

        /// <summary>
        /// 移除所有的事件监听
        /// </summary>
        public void Clear()
        {
            mMap.Clear();
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchCmd(string type, object data = default(object))
        {
            try
            {
#if DEBUG
                if(data!=null)
                {
                    var t = data.GetType();
                    if(t == typeof(int) || t == typeof(float))
                    {
                        Logger.LogError("不允许在参数使用int或者float");
                    }
                }
#endif
                if (mMap.ContainsKey(type))
                {
                    var impl = mMap[type];
                    impl.Execute(data, this, type);
                }
            }
            catch (System.Exception e)
            {
#if DEBUG
                Logger.LogError(e.ToString());
#endif
            }
        }
        /// <summary>
        /// 派发事件，提供了非装箱的重载
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchCmdInt(string type,int data)
        {
            var i = mIntPool.Get();
            i.value = data;
            DispatchCmd(type, i);
            mIntPool.Put(i);
        }

        public void DispatchCmdLong(string type, long data)
        {
            var i = mLongPool.Get();
            i.value = data;
            DispatchCmd(type, i);
            mLongPool.Put(i);
        }


        /// <summary>
        /// 派发事件，提供了非装箱的重载
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchCmdFloat(string type,float data)
        {
            var f = mFloatPool.Get();
            f.value = data;
            DispatchCmd(type, f);
            mFloatPool.Put(f);
        }

        private XPool<IntValue> mIntPool = new XPool<IntValue>();

        private XPool<FloatValue> mFloatPool = new XPool<FloatValue>();

        private XPool<LongValue> mLongPool = new XPool<LongValue>();


        /// <summary>
        /// 派发事件并且移除事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchAndRemove(string type, object data = default(object))
        {
            DispatchCmd(type, data);
            RemoveAll(type);
        }
        /// <summary>
        /// 派发事件并移除事件,提供了非装箱的重载
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchAndRemove(string type, int data)
        {
            var i = mIntPool.Get();
            i.value = data;
            DispatchAndRemove(type, i);
            mIntPool.Put(i);
        }
        /// <summary>
        /// 派发事件并移除事件,提供了非装箱的重载
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchAndRemove(string type, float data)
        {
            var f = mFloatPool.Get();
            f.value = data;
            DispatchAndRemove(type, f);
            mFloatPool.Put(f);
        }
    }//end class


    /// <summary>
    /// 实现了这个接口的类可以通过FastNotifier来注册事件
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// 派发事件后会收到这个回调，可以通过e.type来区分事件类型
        /// </summary>
        /// <param name="e"></param>
        void OnMessage(ref EventArgumentsF e);
    }//IObserver



    /// <summary>
    /// 事件的参数
    /// </summary>
    public struct EventArgumentsF
    {
        /// <summary>
        /// 派发事件的时候传递的参数
        /// </summary>
        public readonly object param;
        /// <summary>
        /// 事件的派发器
        /// </summary>
        public readonly FastNotifier sender;
        /// <summary>
        /// 事件的类型
        /// </summary>
        public readonly string type;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="param_"></param>
        /// <param name="sender_"></param>
        /// <param name="type_"></param>
        internal EventArgumentsF(object param_,
            FastNotifier sender_,
            string type_)
        {
            param = param_;
            sender = sender_;
            type = type_;
        }
        public static readonly EventArgumentsF Default
            = new EventArgumentsF(null, null, null);
    }
    /// <summary>
    /// 新实现的事件管理器，可以提高性能
    /// 以及针对事件移除的情景做了更好的优化方案
    /// 需要用到事件监听的类需要实现IObserver接口
    /// 这个类只能用于同一个线程，并非跨线程安全的
    /// 
    /// 相对于Notifer的改良:
    /// 1.注册和移除事件不用产生临时的delegate对象，
    /// 减少gc alloc
    /// 2.针对同一个模块的事件处理，集中在同一个OnMessage函数
    /// 中处理通常而言可以让代码更加直观和方便理解
    /// 3.提供了针对移除同一个Observer的所有监听事件的接口
    /// 可以在模块的Dispose中做统一移除处理
    /// 
    /// 谭仲添
    /// </summary>
    public class FastNotifier
    {
        enum E_Operation
        {
            ADD,
            REMOVE
        }//E_Operation

        /// <summary>
        /// 存储了所有的事件和事件监听对象的映射表
        /// </summary>
        private XDictionary<string, XHashSet<IObserver>> mMap
            = new XDictionary<string, XHashSet<IObserver>>();

        #region 添加和移除的缓存列表
        /// <summary>
        /// 操作队列
        /// </summary>
        private List<E_Operation> mOperationList = new List<E_Operation>();
        /// <summary>
        /// 操作对象队列
        /// </summary>
        private List<IObserver> mObjectList = new List<IObserver>();
        /// <summary>
        /// 消息类型的队列
        /// </summary>
        private List<string> mMsgTypeList = new List<string>();
        #endregion
        /// <summary>
        /// 清除所有的对象
        /// </summary>
        public void Clear()
        {
            mAllocator.ReleaseItems(mMap);
            mMap.Clear();
            mOperationList.Clear();
            mObjectList.Clear();
            mMsgTypeList.Clear();
            mAllocator.ReleaseItems(mReflection);
            mReflection.Clear();
        }//Clear

        private Allocator mAllocator = new Allocator();

        /// <summary>
        /// 定制的Allocator
        /// </summary>
        public class Allocator
        {
            private List<XHashSet<IObserver>> mL1 = new List<XHashSet<IObserver>>();
            public XHashSet<IObserver> GetObserverSet()
            {
                XHashSet<IObserver> ret = null;
                var n = mL1.Count - 1;
                if(n>=0)
                {
                    ret = mL1[n];
                    mL1.RemoveAt(n);
                }
                else
                {
                    return new XHashSet<IObserver>();
                }
                return ret;
            }//GetObserverSet

            private List<XHashSet<string>> mL2 = new List<XHashSet<string>>();

            public XHashSet<string> GetStringSet()
            {
                XHashSet<string> ret = null;
                var n = mL2.Count - 1;
                if (n >= 0)
                {
                    ret = mL2[n];
                    mL2.RemoveAt(n);
                }
                else
                {
                    return new XHashSet<string>();
                }
                return ret;
            }


            public void ReleaseItems(XDictionary<string, XHashSet<IObserver>> M)
            {
                var L = M.GetList();
                for(int i = 0,n=L.Count;i<n;++i)
                {
                    var it = L[i].Value;
                    it.Clear();
                    mL1.Add(it);
                }//for
            }//ReleaseItems

            public void ReleaseItems(XDictionary<IObserver, XHashSet<string>> M)
            {
                var L = M.GetList();
                for (int i = 0, n = L.Count; i < n; ++i)
                {
                    var it = L[i].Value;
                    it.Clear();
                    mL2.Add(it);
                }//for
            }//ReleaseItems

        }//Allocator

        /// <summary>
        /// 为了避免装箱强制约束只能传递非值类型的参数
        /// 如果要传递值类型的数据，请自定义一个Cache对象
        /// 并且声明为全局可取，派发事件之前存在那个地方
        /// 相应的对象从对应的地方取
        /// 派发完以后消除引用
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgType"></param>
        /// <param name="param"></param>
        public void DispatchCmd<T>(string msgType,T param)where T:class
        {
            if (mDirty)
            {
                Flush();
            }//if

            var HS = default(XHashSet<IObserver>);
            if(mMap.TryGetValue(msgType,out HS))
            {
                var L = HS.GetList();
                var e = new EventArgumentsF(param, this, msgType);
                for(int i=0;i< L.Count; ++i)
                {
                    var it = L[i];
                    it.OnMessage(ref e);
                }//for
            }//if

            if (mDirty)
            {
                Flush();
            }//if
        }//DispatchCmd
        /// <summary>
        /// 无参数的事件调用
        /// </summary>
        /// <param name="msgType"></param>
        public void DispatchCmd(string msgType)
        {
            if (mDirty)
            {
                Flush();
            }//if

            var HS = default(XHashSet<IObserver>);
            if (mMap.TryGetValue(msgType, out HS))
            {
                var L = HS.GetList();
                var e = new EventArgumentsF(null, this, msgType);
                for (int i = 0; i < L.Count; ++i)
                {
                    var it = L[i];
                    it.OnMessage(ref e);
                }//for
            }//if

            if (mDirty)
            {
                Flush();
            }//if
        }

        private XPool<CommonActionParamValue> mParamValPool = new XPool<CommonActionParamValue>();

        public void DispatchCmdParam(string msgType, CommonActionParam param)
        {
            var p = mParamValPool.Get();
            p.value = param;
            DispatchCmd(msgType, p);
            mParamValPool.Put(p);
        }

        /// <summary>
        /// 派发事件，提供了非装箱的重载
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void DispatchCmdInt(string msgType, int data)
        {
            var i = mIntValPool.Get();
            i.value = data;
            DispatchCmd(msgType, i);
            mIntValPool.Put(i);
        }
        private XPool<IntValue> mIntValPool = new XPool<IntValue>();

   
        

        /// <summary>
        /// 可以找到某个对象监听了哪些事件，以方便做清理移除
        /// 
        /// </summary>
        private XDictionary<IObserver, XHashSet<string>> mReflection
             = new XDictionary<IObserver, XHashSet<string>>();
        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="observer"></param>
        public void Regist(string msgType,IObserver observer)
        {
            mDirty = true;
            mObjectList.Add(observer);
            mOperationList.Add(E_Operation.ADD);
            mMsgTypeList.Add(msgType);
        }//Regist

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="observer"></param>
        public void Remove(string msgType,IObserver observer)
        {
            mDirty = true;
            mObjectList.Add(observer);
            mOperationList.Add(E_Operation.REMOVE);
            mMsgTypeList.Add(msgType);
        }//Remove

        /// <summary>
        /// 标记当前是否添加或者移除过元素
        /// </summary>
        private bool mDirty;

        /// <summary>
        /// 移除某个对象监听过的所有的事件
        /// 通常用于模块的结束的时候的处理
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveTarget(IObserver observer)
        {
            var strSet = default(XHashSet<string>);
            if(mReflection.TryGetValue(observer,out strSet))
            {
                var L = strSet.GetList();
                for(int i=0,n=L.Count;i<n;++i)
                {
                    Remove(L[i], observer);
                }//for
            }//if
        }//RemoveTarget

        /// <summary>
        /// 将添加和移除的事件加入处理，以为是队列的形式存在，因此可以保证逻辑上的一致
        /// </summary>
        public void Flush()
        {
            mDirty = false;
            var N = mOperationList.Count;
            for(int i=0;i<N;++i)
            {
                var msg = mMsgTypeList[i];
                var op = mOperationList[i];
                var handler = mObjectList[i];
                if(E_Operation.ADD==op)
                {
                    #region 添加处理逻辑
                    //标记这个observer添加了哪些事件
                    var strSet = default(XHashSet<string>);
                    if(!mReflection.TryGetValue(handler,out strSet))
                    {
                        strSet = mAllocator.GetStringSet(); //new XHashSet<string>();
                        mReflection.Add(handler, strSet);
                    }
                    if(strSet.Contains(msg))
                    {
                        //如果已经监听过这个事件的情况，continue
                        continue;
                    }
                    else
                    {
                        strSet.Add(msg);
                    }
                    var HS = default(XHashSet<IObserver>);
                    if(!mMap.TryGetValue(msg,out HS))
                    {
                        HS = mAllocator.GetObserverSet();//new XHashSet<IObserver>();
                        mMap.Add(msg, HS);
                    }//if
                    if(!HS.Contains(handler))
                    {
                        HS.Add(handler);
                    }//if
                    #endregion
                }//if--add
                else if(E_Operation.REMOVE==op)
                {
                    #region 删除处理逻辑
                    var strSet = default(XHashSet<string>);
                    if(!mReflection.TryGetValue(handler,out strSet))
                    {
                        //如果这里不包含了一定是没有监听过事件
                        continue;
                    }//if
                    else
                    {
                        if(!strSet.Contains(msg))
                        {
                            //如果没有监听过这个事件
                            continue;
                        }//if
                        else
                        {
                            strSet.Remove(msg);
                        }//else
                    }//else
                    var observerSet = default(XHashSet<IObserver>);
                    if(mMap.TryGetValue(msg,out observerSet))
                    {
                        if(observerSet.Contains(handler))
                        {
                            observerSet.Remove(handler);
                        }//if
                    }//if
                    #endregion
                }//else -remove
            }//for i-N

            //清理掉缓存队列
            mOperationList.Clear();
            mObjectList.Clear();
            mMsgTypeList.Clear();
        }//Flush
    }//FastNotifier

}//end namespace


