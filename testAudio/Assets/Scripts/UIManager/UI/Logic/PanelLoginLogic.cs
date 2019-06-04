using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace COMMON
{
    public class PanelLoginLogic :BasePanelLogic
    {
        /// <summary>
        /// 标记是否已经清理资源了
        /// </summary>
        private bool mDisposed = false;

        private Notifier.Eventhandler mOnOpen;
        /// <summary>
        /// 可以注册相对应的网络事件
        /// </summary>
        public override void InitEvent()
        {
            base.InitEvent();
            if (null == mOnOpen)
            {

                mOnOpen = OnOpen;
                PanelEvents.notifier.Regist(PanelEvents.PANEL_OPEN_LOGIN_VIEW, mOnOpen);

            }
        }

        private void OnOpen(ref EventArguments e)
        {
           

            Active();
        }
        
       
        private void RemoveEvents()
        {
            if (null != mOnOpen)
            {
                PanelEvents.notifier.Remove(PanelEvents.PANEL_OPEN_LOGIN_VIEW, mOnOpen);
               
                mOnOpen = null;
            }
        }
        /// <summary>
        /// 逻辑循环的部分
        /// </summary>
        public override void Update()
        {
            base.Update();
        }
        public override void Active()
        {
            base.Active();
            mView = UIPanelManager.ShowPanel(UI_PANEL_NAME.PANEL_LOGIN_VIEW);
            mView.mLogic = this;

        }

          public override void Dispose()
          {
            if (!mDisposed)
            {
                mDisposed = true;
            }
            else
            {
                return;
            }
            base.Dispose();
            RemoveEvents();
           }//Dispose

    }
}

