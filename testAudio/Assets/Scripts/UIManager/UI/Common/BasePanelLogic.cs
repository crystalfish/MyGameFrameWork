using System;

namespace COMMON
{
    public class BasePanelLogic : System.IDisposable
    {
        public BasePanel mView;

        public BasePanelLogic()
        {
            InitEvent();
        }

        /// <summary>
        /// 激活面板
        /// </summary>
        public virtual void Active()
        { }

        /// <summary>
        /// 注册事件响应函数
        /// </summary>
        public virtual void InitEvent()
        { }


        public virtual void Update()
        { }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public virtual void ShutDown(bool bOnClose = true)
        {
            if(null != mView)
            {
                mView.HidePanel(bOnClose);
            }
        }

        /// <summary>
        /// 摧毁面板
        /// </summary>
        public virtual void OnDestroy()
        {
            if(null != mView)
            {
                mView.HidePanel();
                mView.OnDestroy();
            }
            mView = null;
        }
        /// <summary>
        /// 标记是否已经清理了资源
        /// </summary>
        private bool mDisposed = false;
        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            if(!mDisposed)
            {
                mDisposed = true;
            }
            else
            {
                return;
            }

            mView = mView.TryDispose();
        }//Dispose
    }//BasePanelLogic
}//namespace COMMON