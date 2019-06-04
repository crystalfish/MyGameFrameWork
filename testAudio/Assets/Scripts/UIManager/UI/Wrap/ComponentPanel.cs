using System;
using UnityEngine;

namespace COMMON
{
    public class ComponentPanel : BasePanelAdapter
    {
        public Action CloseManuallyCallBack;    //通过返回按钮关闭时的回调
        public Action OnHideCallback;   //关闭面板时的回调
        public ComponentPanel(GameObject go)
        {
            m_go = go;
            if (go != null)
            {
                m_tran = go.GetComponent<RectTransform>();
            }
            NeedShowPrePanel = true;
            panelType = EPanelType.SubContent;
        }

        public override void OnHide()
        {
           if(null != OnHideCallback)
            {
                OnHideCallback();
            }
        }

        /// <summary>
        /// 通过返回按钮关闭时的特定回调
        /// </summary>
        public override void CloseManually()
        {
           if(null != CloseManuallyCallBack)
            {
                CloseManuallyCallBack();
            }
        }
    }
}
