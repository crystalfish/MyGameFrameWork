using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using COMMON;
public class Manager : MonoBehaviour {

    
    
    // Use this for initialization
    void Start() {
        CreateCache();

    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
        TableManager.Instance.TableLoad();
    }

    public void Init()
    {
    }

    public void CreateCache() {
        try
        {
            GameCache.CreateInstance();
            GameCache.Instance.SetUp();
        }
        catch (System.Exception ex)
        {
            //COMMON.Logger.LogError(ex.ToString());
        }
    }
    // Update is called once per frame
    void Update() {
        SimpleSingleton<MAudioManager>.Instance.DoUpdate();
    }

}
