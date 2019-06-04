using UnityEngine;
using System.Collections;

/// <summary>
/// UI面板事件
/// </summary>
namespace COMMON
{
    public class PanelEvents
    {
        public static Notifier notifier = new Notifier();

        /// <summary>
        /// 打开账号面板
        /// </summary>
        public const string PANEL_OPEN_ACCOUNT_VIEW = "PANEL_OPEN_LOGIN";


        /// <summary>
        /// 打开登录面板
        /// </summary>
        public const string PANEL_OPEN_LOGIN_VIEW = "PANEL_OPEN_LOGIN_VIEW";


        
        /// <summary>
        /// 打开主面板
        /// </summary>
        public const string PANEL_MAIN_OPEN = "PANEL_MAIN_OPEN";


    
        #region 打开/关闭面板事件
    

        /// <summary>
        /// 关闭等待面板
        /// </summary>
        public const string PANEL_CLOSE_WAITING = "PANEL_CLOSE_WAITING";


        /// <summary>
        /// 打开一个系统界面
        /// </summary>
        public const string OPEN_UI_PANEL = "OPEN_UI_PANEL";

        
        /// <summary>
        /// 界面关闭通知
        /// </summary>
        public const string PANEL_CLOSE_NOTIFY = "PANEL_CLOSE_NOTIFY";

        /// <summary>
        /// 界面打开通知
        /// </summary>
        public const string PANEL_OPEN_NOTIFY = "PANEL_OPEN_NOTIFY";

      

        #endregion
       
    }
}

