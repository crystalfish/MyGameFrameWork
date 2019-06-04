using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using COMMON;
public class Manager : MonoBehaviour {


    public Transform ig;
    // Use this for initialization
    void Start() {
        CreateCache();

    }
    //public void
    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
        TableManager.Instance.TableLoad();
    }

    public void Init()
    {
        //秒步进器例子
        //GameCache.Instance.sSecondTimer.SetTimer(OnSeconds);
        //GameCache.Instance.sSecondTimer.RemoveTimer(OnSeconds);

    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "打开登录界面"))
        {

            //COMMON.UIEffectManager.Instance.CreateEffect(eUIEffectType.BulletEmpty,ig , new Vector3(0,0,-1f));
        }
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

    public void OnSeconds(){
            
    }
}
