using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMON
{
    /// <summary>
    /// 界面资源路径,新界面必填位置
    /// </summary>
    public class UIPanelPrefabConfig 
    {
        public static string LoginView = "UIPanel/panelloginview";
    }

    /// <summary>
    /// UI窗口名字,新界面必填位置
    /// Author : joi
    /// </summary>
    public class UI_PANEL_NAME
    {
        /// <summary>
        /// 登录界面
        /// </summary>
        public const string PANEL_LOGIN_VIEW = "PanelLoginView";


    }


    /// <summary>
    /// 界面唯一ID ,新界面必填位置
    /// </summary>
    public class E_PanelID
    {
        public const int NULL = 0;

        /// <summary>
        /// 登录面板
        /// </summary>
        public const int ACCOUNT_VIEW = 1;

        /// <summary>
        /// 登录界面
        /// </summary>
        public const int LOGIN_VIEW = 2;


    }//end class
}

