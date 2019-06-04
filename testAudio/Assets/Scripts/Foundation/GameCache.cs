using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using COMMON;
public class GameCache : IDisposable {

    ~GameCache()
    {
        Dispose();
    }
    /// <summary>
    /// 初始化流程
    /// </summary>
    public void SetUp()
    {



        if (!sInited)
        {
            sInited = true;
        }//if
        else
        {
            return;
        }//else

        gGlobalTimer = new XTimer();
        sSecondTimer = new SecondsTimer();
    }

    public void Clear()
    {

        if (sInited)
        {
            sInited = false;
        }
        else
        {
            return;
        }
       
    }
    /// <summary>
    /// 标记是否已经初始化了
    /// </summary>
    private bool sInited = false;

    /// <summary>
    /// 标记是否已经销毁了资源了
    /// </summary>
    private bool mDisposed = false;
    /// <summary>
    /// 销毁资源
    /// </summary>
    public void Dispose()
    {
        if (!mDisposed)
        {
            mDisposed = true;

            //这个很重要，因为不Dispose子线程结束不了   !!!!!!!!


            Clear();
            //清除Instance引用
            CleanInstance();


        }//if
    }//Dispose


    public XTimer gGlobalTimer;

    public SecondsTimer sSecondTimer;

    #region Singleton接口
#if dev
        public static ECache Instance { get; private set; }
#else
    public static GameCache Instance;
#endif

    public static void CreateInstance()
    {
        if (null == Instance)
        {
            Instance = new GameCache();
        }
    }

    public static bool HasInstance()
    {
        return Instance != null;
    }//HasInstance

    public void CleanInstance()
    {
        Instance = null;
    }//CleanInstance

    #endregion

    //对象池
    public readonly XPool<MAudio> audioPool
            = new XPool<MAudio>();
    public readonly XPool<MAudioObject> AudioObjectPool = new XPool<MAudioObject>();




}
