//joi

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace COMMON
{
    /// <summary>
    /// 逻辑管理基类
    /// </summary>
    public class MLogicManager : MBaseLogic
    {
        protected XDictionary<Type, MBaseLogic> _logicDict = new XDictionary<Type, MBaseLogic>();
        protected List<Type> _logicDictKeyList = new List<Type>();

        public MLogicManager()
        {

        }
        /// <summary>
        /// 创建逻辑集合
        /// </summary>
        /// <returns></returns>
        protected virtual List<Type> CreateLogics()
        {
            return new List<Type>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool AddLogic<T>() where T : MBaseLogic, new()
        {
            Type t = typeof(T);
            if (!_logicDict.ContainsKey(t))
            {
                _logicDict.Add(t, new T());
                _logicDictKeyList.Add(t);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取相对应的逻辑类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLogic<T>() where T : MBaseLogic, new()
        {
            Type t = typeof(T);
            var list = _logicDict.GetList();
            for (int i = 0, len = list.Count; i < len; i++)
            {
                var kvp = list[i];
                if (t == kvp.Key || kvp.Key.IsSubclassOf(t))
                    return kvp.Value as T;
            }
           
            return default(T);
        }

        /// <summary>
        /// 获取相对应的逻辑类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public MBaseLogic GetLogic(Type t)
        {
            var list = _logicDict.GetList();
            for (int i = 0, len = list.Count; i < len; i++)
            {
                var kvp = list[i];
                if (t == kvp.Key || kvp.Key.IsSubclassOf(t))
                    return kvp.Value ;
            }
            return null;
        }

        public override void DoUpdate()
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
                _logicDict[_logicDictKeyList[i]].DoUpdate();
        }

        public override void DoFixedUpdate()
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
                _logicDict[_logicDictKeyList[i]].DoFixedUpdate();
        }

        public override void DoLateUpdate()
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
                _logicDict[_logicDictKeyList[i]].DoLateUpdate();
        }

        public override void DoOnGUI()
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
                _logicDict[_logicDictKeyList[i]].DoOnGUI();
        }

#if UNITY_EDITOR
	public override void DoOnDrawGizmos ()
	{
		for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
			_logicDict [_logicDictKeyList [i]].DoOnDrawGizmos ();
	}
#endif
        public override void DoClean()
        {
            this.DoClean(new Type[0], true);
            base.DoClean();
        }
        /// <summary>
        /// 清除对象
        /// </summary>
        public override void DoDestroy()
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
                _logicDict[_logicDictKeyList[i]].DoDestroy();
            _logicDict.Clear();
            _logicDictKeyList.Clear();
        }
        /// <summary>
        /// 主要用于清除数据
        /// </summary>
        /// <param name="types"></param>
        /// <param name="except"></param>
        public virtual void DoClean(Type[] types, bool except = false)
        {
            for (int i = 0, len = _logicDictKeyList.Count; i < len; i++)
            {
                if (Array.IndexOf<Type>(types, _logicDictKeyList[i]) != -1)
                {
                    if (!except)
                        _logicDict[_logicDictKeyList[i]].DoClean();
                }
                else
                {
                    if (except)
                        _logicDict[_logicDictKeyList[i]].DoClean();
                }
            }
        }

    }
}
