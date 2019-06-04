using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace COMMON
{
    public class PanelLoginView : BasePanelAdapter
    {
        public PanelLoginView()
        {
            isRoot = true;
            panelType = EPanelType.Content;
            SetPanelPrefabPath(UIPanelPrefabConfig.LoginView);
        }

        public override int GetPanelID()
        {
            return E_PanelID.LOGIN_VIEW;
        }
        private GameObject lightButton;
        public override void Init()
        {
            base.Init();

            lightButton = this.m_go.FindChildObj("light");
            
        }
        private bool ShowLoginView(GameObject obj, PointerEventData data)
        {
            Debug.LogError("joi");
            return true;
        }
        /// <summary>
        /// 注册组件响应事件
        /// </summary>
        public override void InitEvent()
        {
            UGUIClickHandler.Get(lightButton).onPointerClick += ShowLoginView;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnClose()
        {
            base.OnClose();
        }
        public override void OnHide()
        {
            base.OnHide();
        }
        private bool mIsDisposed = false;

        public override void Dispose()
        {
            if (mIsDisposed)
            {
                return;
            }

            mIsDisposed = true;
            base.Dispose();
        }
    }
}

