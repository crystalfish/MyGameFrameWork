//using Common;
//using GameLoader.Utils;
//using GameLoader.Utils.Timer;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIClickHandler : MonoBehaviour, IPointerClickHandler,IPointerDownHandler,IPointerUpHandler, IDisposable
{
    public delegate bool PointerEvetCallBackFunc(GameObject target, PointerEventData eventData);
    // public string m_sound = AB.AUDIO_COMMON_CLICK;
    //可点击按钮，有有效和无效音效
    public event PointerEvetCallBackFunc onPointerClick;
    //点击返回按钮，有区别于点击其他按钮的音效
    public event PointerEvetCallBackFunc onPointerBtnBackClick;

    //点击面板，没有点击音效
    public event PointerEvetCallBackFunc onPointerPanelClick;

    //支持长按处理
    public bool bOpenLongPress = false; //是否打开长按功能
    public float durationThreshold = 0.8f; //长按的判断阈值
    private bool isPointerDown = false;
    private bool longPressTriggered = false; 
    private float timePressStarted;
    public event PointerEvetCallBackFunc onLongPressCallBack; //长按
    public event PointerEvetCallBackFunc onLongPressUpCallBack; //长按弹起

    private PointerEventData longPressEventData;



    public float m_iCDTime = 0.3f;
    public float m_iTimeStamp;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - m_iTimeStamp < m_iCDTime)
            return;
        m_iTimeStamp = Time.time;

        /*if (!string.IsNullOrEmpty(m_sound))
            AudioManager.PlayUISound(m_sound);*/
        if(longPressTriggered)
        {
            return;
        }
        if (onPointerClick != null)
        {
            onPointerClick(gameObject, eventData);
            //if (onPointerClick(gameObject, eventData))
            //{
            //    PLD.SimpleSingleton<PLD.MAudioManager>.Instance.Play(300058, PLD.GameLogic.Instance.transform);
            //}
            //else
            //{
            //    PLD.SimpleSingleton<PLD.MAudioManager>.Instance.Play(300057, PLD.GameLogic.Instance.transform);
            //}
        }

        //返回按钮专用，区别音效
        if (onPointerBtnBackClick != null)
        {
            //onPointerBtnBackClick(gameObject, eventData);
            //PLD.SimpleSingleton<PLD.MAudioManager>.Instance.Play(300058, PLD.GameLogic.Instance.transform);
        }

        if (null != onPointerPanelClick)
        {
            onPointerPanelClick(gameObject, eventData);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(bOpenLongPress)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
            longPressEventData = eventData;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (bOpenLongPress)
        {
            isPointerDown = false;
        }
        if(longPressTriggered)
        {
            if (onLongPressUpCallBack != null)
            {
                onLongPressUpCallBack(gameObject, eventData);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (bOpenLongPress)
        {
            isPointerDown = false;
        }
    }
    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            if (Time.time - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
                if (onLongPressCallBack != null)
                {
                    onLongPressCallBack(gameObject, longPressEventData);
                }
                
            }
        }
    }
    public void RemoveAllHandler(bool isDestroy = true)
    {
        if (mIsDisposed)
        {
            return;
        }
        m_iCDTime = 0.3f;
        onPointerBtnBackClick = null;
        onPointerPanelClick = null;
        onPointerClick = null;
        onPointerBtnBackClick = null;
        onPointerPanelClick = null;
        onLongPressCallBack = null;
        onLongPressUpCallBack = null;
  
        if (isDestroy)
        {
            DestroyImmediate(this);
        }
    }

    public UGUIClickHandler SetCDTime(float seconds)
    {
        m_iCDTime = (uint)(seconds * 1000);
        return this;
    }

    public static UGUIClickHandler Get(GameObject go)
    {
        if (null == go)
        {
            return null;
        }
        UGUIClickHandler listener = go.GetComponent<UGUIClickHandler>();
        if (listener == null)
        {
            listener = go.AddComponent<UGUIClickHandler>();
        }
            
        return listener;
    }

    public static UGUIClickHandler Get(GameObject go, string sound)
    {
        UGUIClickHandler listener = Get(go);
        //listener.m_sound = sound;
        return listener;
    }

    public static UGUIClickHandler Get(Transform tran, string sound)
    {
        UGUIClickHandler listener = Get(tran);
        //listener.m_sound = sound;
        return listener;
    }

    public static UGUIClickHandler Get(Transform tran)
    {
        return Get(tran.gameObject);
    }

    private bool mIsDisposed = false;
    public virtual void Dispose()
    {
        if (mIsDisposed)
        {
            return;
        }

        mIsDisposed = true;
        longPressEventData = null;
        longPressTriggered = false;
        RemoveAllHandler();
    }

    void OnDestroy()
    {
        Dispose();
    }
}