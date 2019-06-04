using UnityEngine;
using System.Collections;
using System;

namespace COMMON
{
    public class UILogicController : System.IDisposable
    {

        public PanelLoginLogic mLogin;
        public UILogicController()
        {

        }
        public void Init()
        {
            mLogin = new PanelLoginLogic();                 //登录
            
        }
        private bool mDisposed = false;
        public virtual void Dispose()
        {
            if (mDisposed)
            {
                return;
            }
            else
            {
                mDisposed = true;
            }
            var list = new System.Collections.Generic.List<BasePanelLogic>()
            {
                mLogin,
            };

            for (int i = 0, n = list.Count; i < n; ++i)
            {
                var it = list[i];
                if (null == it.mView)
                {
                    it.Dispose();
                    continue;
                }
                var go = it.mView.GetGameObject();
                it.Dispose();
                if (null != go)
                {
                    UnityEngine.Object.Destroy(go);
                }
            }
            list.Clear();
            mLogin = mLogin.TryDispose();
          

        }//Dispose
    }
}


