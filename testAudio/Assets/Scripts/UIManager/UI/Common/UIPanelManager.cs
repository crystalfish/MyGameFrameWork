using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace COMMON
{
    /// <summary>
    /// 封装界面弹窗逻辑的工具类
    /// </summary>
    public static class UIPanelManager
    {
        private static XDictionary<string, BasePanel> m_panelDic = new XDictionary<string, BasePanel>();
        private static XDictionary<string, BasePanel> m_TopPanelDic = new XDictionary<string, BasePanel>();
        /// <summary>
        /// 面板堆栈，功能面板和弹框面板需要加入堆栈，相应回退功能
        /// </summary>
        private static Stack<BasePanel> mStackPanel = new Stack<BasePanel>();

        private static BasePanel m_contentPanel;    //当前正在显示的主Panel
        
        //记录面板ID及对应的事件
        private static Dictionary<int, string> mDicPanelIDAndEvents = new Dictionary<int, string>();

        public static bool bInLoginState = false; //当前正处于登录状态

        private static void InitData()
        {
            m_panelDic = new XDictionary<string, BasePanel>();
            m_TopPanelDic = new XDictionary<string, BasePanel>();
            /// <summary>
            /// 面板堆栈，功能面板和弹框面板需要加入堆栈，相应回退功能
            /// </summary>
            mStackPanel = new Stack<BasePanel>();
            mComponentStack = new Stack<GameObject>();

            m_contentPanel = null;    //当前正在显示的主Panel
          
            //记录面板ID及对应的事件
            mDicPanelIDAndEvents = new Dictionary<int, string>();
            bInLoginState = false;
        }

        private static void DisposeMap(XDictionary<string, BasePanel> M)
        {
            var L = M.GetList();
            for (int i = 0, n = L.Count; i < n; ++i)
            {
                var panel = L[i].Value;
                var go = panel.GetGameObject();
                bool ddol = panel.DontDestroyOnLoad;
                if (null != go && !ddol)
                {
                    UnityEngine.Object.Destroy(go);
                }
                if (!ddol)
                {
                    panel.Dispose();
                }

            }
            L.Clear();
        }

        public static void Init()
        {
            InitPanelIDData();
        }


        #region 提示UI相关调用方法

        #endregion

        #region Panel相关操作方法

        /// <summary>
        /// 通过id打开对应的面板
        /// </summary>
        /// <param name="id"></param>
        public static void OpenPanelByID(int id,
            /*StateEnum.OperationViewOpenPanelParam param = null,*/
            object param = null,Func<bool> ConditionCheckCallback=null)
        {
            if (mDicPanelIDAndEvents.ContainsKey(id) &&(ConditionCheckCallback==null ||
                (ConditionCheckCallback !=null && ConditionCheckCallback())))
            {
                PanelEvents.notifier.DispatchCmd(mDicPanelIDAndEvents[id], param);
            }
        }

        public static void OpenPanelByID(int id, int parm)
        {
            if (mDicPanelIDAndEvents.ContainsKey(id))
            {
                PanelEvents.notifier.DispatchCmdInt(mDicPanelIDAndEvents[id], parm);
            }
        }

        /// <summary>
        /// 检测一个界面是否已经打开
        /// </summary>
        public static bool IsOpen(string panelName)
        {
            var list = m_panelDic.GetList();
            for (int i = 0; i < list.Count; i++)
            {
                var panel = list[i].Value;
                if (panel != null && panel.IsActiveInHierarchy() &&
                    panel.IsOpen())
                {
                    if (IsMatchByName(panel, panelName))
                    {
                        return true;
                    }
                }
            }

            list = m_TopPanelDic.GetList();
            for (int i = 0; i < list.Count; i++)
            {
                var panel = list[i].Value;
                if (panel != null && panel.IsActiveInHierarchy() &&
                    panel.IsOpen())
                {
                    if (IsMatchByName(panel, panelName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 返回指定界面名字是否跟目标名字一致
        /// </summary>
        public static bool IsMatchByName(BasePanel panel, string targetName)
        {
            string name = panel.GetPanelName();
            if (name.Equals(targetName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 展示某一界面
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static BasePanel ShowPanel(string panelName, object[] param = null)
        {
            BasePanel panel = null;
            if (!m_panelDic.TryGetValue(panelName, out panel))
            {
                panel = CreatePanelByName(panelName);
                m_panelDic.Add(panelName, panel);
                panel.PanelClassName = panelName;
            }
            ///如果当前面板有顶部栏附属面板，这时打开面板时没有关闭当前的面板
            ///而该面板比顶部栏面板层级高，则会出现顶部面板没有显示出来的情况
            ///加下面的语句只是为了减少重复打开面板，但不会导致报错
            //if (panel.IsOpen())
            //{
            //    panel.UpdatePanelSenderOrder();
            //    return panel;
            //}

            //m_contentPanel = panel;
            PushStack(panel, panelName, param);

            panel.SetParams(param);

            GameObject rootCanvas = UICanvasManager.Instance.Canvas;
            if (null != rootCanvas)
            {
                panel.ShowPanel(rootCanvas.transform);
            }

            //派发打开界面的事件
            PanelEvents.notifier.DispatchCmd(PanelEvents.OPEN_UI_PANEL, panel);

            return panel;
        }


        public static int GetFirstPanelId()
        {

            if (mStackPanel.Count > 0)
            {
                BasePanel firstContentPanel = mStackPanel.Peek();

                return firstContentPanel.GetPanelID();
            }
            return -1;
        }

        private static Stack<GameObject> mComponentStack = new Stack<GameObject>();

        /// <summary>
        /// 打开面板内容子面板
        /// </summary>
        /// <param name="go"></param>
        public static void ShowComponentGameObject(GameObject go, Action OnHideCallback = null,
            Action CloseManuallyCallBack = null)
        {
            if (go == null) return;

            BasePanel panel = null;
            string panelName = go.name;
            if (!m_panelDic.TryGetValue(panelName, out panel))
            {
                panel = new ComponentPanel(go);
                ComponentPanel componentPanel = panel as ComponentPanel;
                componentPanel.OnHideCallback = OnHideCallback;
                componentPanel.CloseManuallyCallBack = CloseManuallyCallBack;
                m_panelDic.Add(panelName, panel);
                panel.PanelClassName = panelName;
            }

            //m_contentPanel = panel;
            if (go.activeInHierarchy == false)
            {
                panel.ShowPanel();
            }

            // RemovePanelFromStack(panel);
            mStackPanel.Push(panel);
        }

        /// <summary>
        /// 关闭面板内容子面板
        /// </summary>
        /// <param name="go"></param>
        public static void HideComponentGameObject(GameObject go)
        {
            BasePanel panel = null;
            string panelName = go.name;
            if (m_panelDic.TryGetValue(panelName, out panel))
            {
                HidePanelByNameFromStack(panelName);
            }
        }

        public static void HidePanelByEsc()
        {
            if (mStackPanel.Count == 0)
            {
                return;
            }
        }

        static BasePanel GetFirstShowPanel()
        {
            BasePanel panel = null;
            if (mStackPanel.Count > 0)
            {
                panel = mStackPanel.Peek();
            }
            return panel;
        }

        /// <summary>
        /// 执行返回
        /// </summary>
        public static void BackPanel()
        {
            if (mStackPanel.Count > 0)
            {
                BasePanel firstPanel = GetFirstShowPanel();
                if (firstPanel == null)
                {
                    mStackPanel.Pop();
                    BackPanel();
                    return;
                }


                if (firstPanel.IsRootPanel())
                {
                    return;
                }

                //取出当前需要关闭的面板
                firstPanel = mStackPanel.Pop();
                //取出需要显示的前一个面板
                //关闭当前面板
                if (firstPanel.IsOpen())
                {
                    firstPanel.CloseManually();
                    firstPanel.HidePanel();
                }

                //防止关闭弹框面板时没有从堆栈中移除，如果返回到的上层面板时弹框
                //则关闭弹框，继续向上遍历面板
                BasePanel prePanel = GetFirstShowPanel();
                while (null == prePanel ||
                    prePanel.PanelType == EPanelType.ContentTips )
                {
                    //如果已经没有可回退的面板，则当在非战斗场景中则显示主面板
                    if (prePanel == null)
                    {
                       
                        PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_MAIN_OPEN);
                        return;
                    }
                    else
                    {
                        //如果是弹框面板，则关闭弹框，再打开上一层面板
                        BasePanel panel = mStackPanel.Pop();
                        Logger.LogWarning("~~~UIpanelmange-BackPanel:堆栈中有弹框面板，可能会导致ui出现空白：{0}", panel.GetPanelName());

                        panel.HidePanel();
                        prePanel = GetFirstShowPanel();
                    }
                }

                if (prePanel.IsOpen() == false)
                {
                    prePanel.SetActive(true);
                }

                if (prePanel.NeedShowPrePanel && mStackPanel.Count > 1)
                {
                    ///如果打开的面板有半透明背景，可以看见上层面板或者是某个面板的子面板，那上层面板也
                    ///需要打开，比如从主界面打开个人信息界面，然后打开比赛详情，这时关闭
                    ///比赛详情界面，则重新打开了个人信息界面，这时可以看见主界面，这时主界面
                    ///需要打开
                    BasePanel[] panels = mStackPanel.ToArray();
                    int index = 1;
                    do
                    {
                        var nextPanel = panels[index];
                        index++;
                        if(nextPanel != null && nextPanel.PanelType == EPanelType.Content)
                        {
                            nextPanel.OpenAffiliatedPanels();
                            nextPanel.OnBack();
                            break;
                        }
                    } while (mStackPanel.Count>index);
                 
                }
                //if (newParams != null)
                //    prePanel.SetParams(newParams);
                prePanel.OpenAffiliatedPanels();
                prePanel.OnBack();
                //返回界面也派发一个打开界面的事件
                PanelEvents.notifier.DispatchCmd(PanelEvents.OPEN_UI_PANEL, prePanel);
            }
            else
            {
                
                PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_MAIN_OPEN);
               
            }
        }

        public static void PopStack()
        {

        }

        private static void PushStack(BasePanel panel, string panelName, object[] @params = null)
        {
            ///如果是主功能面板或弹出面板则加入堆栈
            if (mStackPanel.Contains(panel))
            {
                BasePanel curPanel = mStackPanel.Peek();
                List<BasePanel> listPanels = new List<BasePanel>();
                while (curPanel != panel)
                {
                    listPanels.Add(curPanel);
                    mStackPanel.Pop();
                    curPanel = mStackPanel.Peek();
                }
                mStackPanel.Pop();
                for (int i = listPanels.Count - 1; i >= 0; --i)
                {
                    mStackPanel.Push(listPanels[i]);
                }
                mStackPanel.Push(panel);
            }
            else
            {
                ///只有功能模块面板和子面板需要入栈，因为这些面板需要指出返回键回退功能
                ///附带面板不需要入栈管理，只需和依附的面板一致即可
                if (panel.PanelType == EPanelType.Content ||
                    panel.PanelType == EPanelType.SubContent ||
                    panel.PanelType == EPanelType.ContentTips)
                {
                    mStackPanel.Push(panel);
                }
            }
        }



        /// <summary>
        /// 在pop层弹出窗口并置顶，该层需要自己维护显示隐藏
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static BasePanel ShowPanelByCanvas2(string panelName, object[] param = null)
        {
            GameObject canvas2 = UICanvasManager.Instance.Canvas2;
            if (null == canvas2)
            {
                return null;
            }

            BasePanel panel = null;
            if (!m_panelDic.TryGetValue(panelName, out panel))
            {
                panel = CreatePanelByName(panelName);
                m_panelDic.Add(panelName, panel);
                panel.PanelClassName = panelName;
            }

            if (panel.IsOpen())
            {
                panel.UpdatePanelSortingOrder();
                return panel;
            }

            //m_contentPanel = panel;

            PushStack(panel, panelName, param);

            panel.SetParams(param);
            panel.ShowPanel(canvas2.transform);
            // panel.LayerType = LayerType.Content;
            //派发打开界面的事件
            PanelEvents.notifier.DispatchCmd(PanelEvents.OPEN_UI_PANEL, panel);
            return panel;
        }

     
        /// <summary>
        /// 在pop层弹出窗口并置顶，该层需要自己维护显示隐藏
        /// </summary>
        public static BasePanel ShowPanelByTopCanvas(string panelName, object[] param = null)
        {
            BasePanel panel = null;
            if (!m_TopPanelDic.TryGetValue(panelName, out panel))
            {
                panel = CreatePanelByName(panelName);
                m_TopPanelDic.Add(panelName, panel);
                panel.PanelClassName = panelName;
            }

            if (panel.IsOpen())
            {
                panel.UpdatePanelSenderOrder();
                return panel;
            }

            //m_contentPanel = panel;

            PushStack(panel, panelName, param);

            panel.SetParams(param);

            GameObject rootCanvas = UICanvasManager.Instance.TopCanvas;
            if (null != rootCanvas)
            {
                panel.ShowPanel(rootCanvas.transform);
            }

            //派发打开界面的事件
            PanelEvents.notifier.DispatchCmd(PanelEvents.OPEN_UI_PANEL, panel);
            return panel;
        }

      
        /// <summary>
        /// 弹出一个界面
        /// 放在topcanvas层，并且不加入UI栈
        /// </summary>
        /// <param name="param"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static BasePanel PopPanel(string panelName, object[] param = null)
        {
            if (!m_TopPanelDic.ContainsKey(panelName))
            {
                m_TopPanelDic.Add(panelName, CreatePanelByName(panelName));
            }

            var popPanel = m_TopPanelDic[panelName];
            popPanel.SetParams(param);
            GameObject rootCanvas = UICanvasManager.Instance.TopCanvas;
            if (null != rootCanvas)
            {
                popPanel.ShowPanel(rootCanvas.transform);
            }
            popPanel.PanelClassName = panelName;
            PanelEvents.notifier.DispatchCmd(PanelEvents.OPEN_UI_PANEL, popPanel);

            return popPanel;
        }

        #region 每次添加新的界面Class记得来这添加一个
        /// <summary>
        /// 根据UI类名new 一个
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        private static BasePanel CreatePanelByName(string panelName)
        {
            BasePanel panel = null;
           // TODO 注册新的面板
            switch (panelName)
            {
                case UI_PANEL_NAME.PANEL_LOGIN_VIEW:
                    panel = new PanelLoginView();
                    break;


            }
            return panel;
        }
        #endregion


        /// <summary>
        /// 显示等待面板
        /// </summary>
        /// <param name="panelType">面板类型（重连提示、网络等待...)</param>
        /// <param name="callBack">关闭面板时的回调</param>
        /// <param name="AutoClose">是否自动关闭</param>
        /// <param name="ShowBtnDelay">是否延迟显示按钮</param>
        /// <param name="BtnCallBack">按钮回调</param>
        public static void ShowWaitingPanel(int panelType, Action callBack = null,
            bool AutoClose = false,
            float ShowBtnDelay = -1,
            int BtnTipId = 11010,
            int ContentId = 0,
            int AutoCloseTIme = 0)
        {
            if (bInLoginState)
                return;

            object[] param = { panelType, callBack, AutoClose, ShowBtnDelay,
                BtnTipId ,ContentId,AutoCloseTIme};
            
           
           
        }




              /// <summary>
        /// 通过名字判断是否存在某一个UI
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static bool HasPanel(string panelName)
        {
            return m_panelDic.ContainsKey(panelName);
        }


        /// <summary>
        /// 通过UI类名获取某一界面
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static BasePanel GetPanel(string panelName)
        {
            BasePanel panel = null;
            if (m_panelDic.TryGetValue(panelName, out panel))
            {
                return panel;
            }
            if (m_TopPanelDic.TryGetValue(panelName, out panel))
            {
                return panel;
            }
            return null;
        }

        /// <summary>
        /// 销毁某一界面
        /// </summary>
        /// <param name="panelName"></param>
        public static void DestroyPanel(string panelName)
        {
            BasePanel panel = null;
            if (m_panelDic.TryGetValue(panelName, out panel))
            {
                DestroyPanelByName(panelName, panel);
                m_panelDic.Remove(panelName);
                return;
            }

            if (m_TopPanelDic.TryGetValue(panelName, out panel))
            {
                DestroyPanelByName(panelName, panel);
                m_TopPanelDic.Remove(panelName);
                return;
            }
        }

        private static void DestroyPanelByName(string panelName, BasePanel panel)
        {
            if (null != panel)
            {
                RemovePanelFromStack(panel);
                //if (panel == m_contentPanel)
                //{
                //    m_contentPanel = null;
                //}
                panel.DestroyPanel();
            }
        }

        private static void RemovePanelFromStack(BasePanel panel)
        {
            if (mStackPanel.Contains(panel))
            {
                List<BasePanel> panelList = new List<BasePanel>();
                BasePanel result;
                while ((result = mStackPanel.Pop()) != panel)
                {
                    panelList.Add(result);
                }

                for (int i = panelList.Count - 1; i >= 0; --i)
                    mStackPanel.Push(panelList[i]);
            }
        }

        #region 关闭面板的函数 (除了面板内部关闭自己，外部关闭面板都应该通过下面4个接口）
        /// <summary>
        /// 关闭面板，但保留在堆栈中，支持返回键重新打开
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bOnClose"></param>
        public static void HidePanelByType(BasePanel panel, bool bOnClose = true)
        {
            if (panel != null)
            {
                HidePanel(panel, false, bOnClose);
            }
        }

        /// <summary>
        /// 通过类别关闭面板并从堆栈移除
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bOnClose"></param>
        public static void HidePanelByTypeFromStack(BasePanel panel, bool bOnClose = true)
        {
            if (panel != null)
            {
                HidePanel(panel, true, bOnClose);
            }
        }

        /// <summary>
        /// 隐藏某一界面
        /// </summary>
        /// <param name="panelName"></param>
        public static void HidePanelByName(string panelName, bool bOnClose = true)
        {
            if (!HidePanelFromDicHandle(panelName, m_panelDic, bOnClose))
            {
                HidePanelFromDicHandle(panelName, m_TopPanelDic, bOnClose);
            }
        }

        /// <summary>
        /// 关闭面板并从堆栈移除
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="bOnClose"></param>
        public static void HidePanelByNameFromStack(string panelName, bool bOnClose = true)
        {
            if (!HidePanelFromDicHandle(panelName, m_panelDic, bOnClose, true))
            {
                HidePanelFromDicHandle(panelName, m_TopPanelDic, bOnClose, true);
            }
        }

        /// <summary>
        /// 遍历字典隐藏某一界面
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="dic"></param>
        private static bool HidePanelFromDicHandle(string panelName, XDictionary<string, BasePanel> dic,
            bool bOnClose = true, bool deleteFromStack = false)
        {
            if (dic.ContainsKey(panelName))
            {
                HidePanel(dic[panelName], deleteFromStack, bOnClose);
                return true;
            }
            return false;
        }


        static void HidePanel(BasePanel panel, bool deleteFromStack = false, bool bOnClose = true)
        {
            if (panel != null)
            {
                if (panel.mLogic != null)
                    panel.mLogic.ShutDown(bOnClose);
                else
                    panel.HidePanel(bOnClose);

                //是否要从堆栈移除（不支持返回键的面板都应该移除)
                if (deleteFromStack)
                {
                    RemovePanelFromStack(panel);
                }
            }
        }

        /// <summary>
        /// 遍历字典隐藏所有的界面
        /// </summary>
        /// <param name="dic"></param>
        private static void HideAllPanelFromDicHandle(XDictionary<string, BasePanel> dic,
            bool deletFromStack = false)
        {
            var L = dic.GetList();
            List<KeyValuePair<string, BasePanel>> panels = new List<KeyValuePair<string, BasePanel>>(L);
            for (int i = 0, n = panels.Count; i < n; i++)
            {
                var kv = panels[i];
                if (kv.Value != null)
                {
                    HidePanel(kv.Value, deletFromStack);
                }
            }
        }//end fun HideAllPanelFromDicHandle

        /// <summary>
        /// 隐藏打开的面板,不包括加载面板
        /// </summary>
        public static void HideAllPanel(bool deleteFromStack = false)
        {
            HideAllPanelFromDicHandle(m_panelDic, deleteFromStack);
            HideAllPanelFromDicHandle(m_TopPanelDic, deleteFromStack);
            BasePanel.mMaxCanvasPanelOrder = 1;
        }


        /// <summary>
        /// 关闭Canvas、Canvas2下的所有面板
        /// </summary>
        public static void HideAllPanelInCanvas(bool deleteFromStack = false)
        {
            HideAllPanelFromDicHandle(m_panelDic, deleteFromStack);
        }


        /// <summary>
        /// 关闭Canvs、Canvas2下的非加载面板
        /// </summary>
        public static void HideAllPanelInCanvasNotLoadingPanel()
        {
            var L = m_panelDic.GetList();
            for (int i = 0, n = L.Count; i < n; i++)
            {
                var kv = L[i];
                if (kv.Value != null)
                {
                    var itemValue = kv.Value;
                    if (itemValue.IsOpen() )
                    {
                        if (itemValue.mLogic != null)
                        {
                            itemValue.mLogic.ShutDown();
                        }
                        else
                            itemValue.HidePanel();
                    }//end if itemValue.IsOpen
                }//end if kv.value != null
            }//end for
        }
        #endregion

        /// <summary>
        /// 初始化面板ID及对应的打开面板事件
        /// </summary>
        static void InitPanelIDData()
        {
            mDicPanelIDAndEvents.Add(E_PanelID.ACCOUNT_VIEW, PanelEvents.PANEL_OPEN_ACCOUNT_VIEW);//账号界面

            mDicPanelIDAndEvents.Add(E_PanelID.LOGIN_VIEW, PanelEvents.PANEL_OPEN_LOGIN_VIEW);//登录界面


        }



        /// <summary>
        /// 将要从字段中移除的面板名字
        /// </summary>
        private static List<string> mRemovePanelNameList = new List<string>();
        /// <summary>
        /// 从字典中销毁未打开的界面
        /// </summary>
        /// <param name="idc"></param>
        private static void DestroyUnusedPanelFromDic(XDictionary<string, BasePanel> dic)
        {
            mRemovePanelNameList.Clear();
            var L = dic.GetList();
            KeyValuePair<string, BasePanel> kv;
            for (int i = 0, n = L.Count; i < n; i++)
            {
                kv = L[i];
                if (kv.Value != null)
                {
                    if (kv.Value.IsOpen() || kv.Value.DontDestroyOnLoad)
                    {
                        continue;
                    }
                    mRemovePanelNameList.Add(kv.Key);
                    kv.Value.DestroyPanel();
                }
            }

            if (mRemovePanelNameList.Count > 0)
            {
                for (int i = 0, n = mRemovePanelNameList.Count; i < n; i++)
                {
                    var panelName = mRemovePanelNameList[i];
                    dic.Remove(panelName);
                }
            }
            mRemovePanelNameList.Clear();
        }//end fun DestroyUnusedPanelFromDic

        /// <summary>
        /// 关闭没有打开的界面
        /// </summary>
        public static void DestroyUnusedPanel()
        {
            return;
            ///从堆栈中删除被销毁的面板
            List<BasePanel> panelList = new List<BasePanel>();
            int count = mStackPanel.Count;
            for (int index = 0; index < count; ++index)
            {
                BasePanel p = mStackPanel.Pop();
                if (p.IsOpen() || p.DontDestroyOnLoad)
                {
                    panelList.Add(p);
                    continue;
                }
            }
            panelList.Clear();
            for (int index = panelList.Count - 1; index >= 0; --index)
            {
                mStackPanel.Push(panelList[index]);
            }

            #region PanelDic TopPanelDic Handle
            //从面板dic中移除已经关闭并需要移除的面板
            DestroyUnusedPanelFromDic(m_panelDic);
            DestroyUnusedPanelFromDic(m_TopPanelDic);
            #endregion


        }


        
        /// <summary>
        /// 弹出顶部面板
        /// </summary>
        /// <param name="titleId">标题对应X_text表中id</param>
        /// <param name="panelLevel">面板层级（用于设置返回按钮类型）</param>
        /// <param name="panelId">面板id</param>
        /// <param name="panelHelpContentId">帮助弹框内容ID</param>
        /// <param name="panelHelpTitleId">帮助弹框标题ID</param>
        public static OpenTopViewParam GetTopViewParam(int titleId, int panelId,
            int panelHelpContentId = 0, int panelHelpTitleId = 0,
            Func<bool> preCallbackBeforClose = null)
        {
            OpenTopViewParam param = new OpenTopViewParam();
            param.panelTitleTextId = titleId;
            param.panelId = panelId;
            param.panelHelpContentId = panelHelpContentId;
            param.panelHelpTitleId = panelHelpTitleId;
            param.preCallbackBeforClose = preCallbackBeforClose;
            // PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_TOP_OPEN, param);
            return param;
        }

      

        public static void Update()
        {
            //if (clearWatchDog > 0)
            //{
            //    clearWatchDog -= Time.deltaTime;
            //    if (clearWatchDog <= 0)
            //    {
            //        clearWatchDog = 0;
            //        HideMask();
            //    }
            //}
            //for (int i = 0; i < m_panelLoopList.Count; i++)
            //{
            //    if (m_panelLoopList[i].IsInited())
            //        m_panelLoopList[i].Update();
            //}
        }


        #endregion

        #region DisposeLugic
        /// <summary>
        /// 切换场景调用释放面板
        /// </summary>
        public static void SwitchoverSceneDispose()
        {
            DisposeForDic(m_panelDic);
            DisposeForDic(m_TopPanelDic);
           
        }

        private static void DisposeForDic(XDictionary<string, BasePanel> dic)
        {
            if (dic != null)
            {
                var list = dic.GetList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0, n = list.Count; i < n; i++)
                    {
                        var temp = list[i];
                        if (temp.Value != null)
                        {
                            temp.Value.Dispose();
                        }
                    }
                }

                dic.Clear();
            }
        }
        #endregion

        public static void SetPanelProgressDepthZero()
        {
            GameObject obj = GameObject.Find("PanelProgress");
            if (obj != null)
            {
                Canvas canvas = obj.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.worldCamera.depth = 0;
                }
            }
        }

        public static void DisposePanels()
        {
            Logger.Log("UIPanelManager.DisposePanels------------>>");
            DisposeMap(m_panelDic);
            DisposeMap(m_TopPanelDic);
           
            //情况所有面板动画记录
            //UIPanelAnimation.Instance.RemoveAll();
            //GameLogic.Instance.mUILogic.Dispose();
            //GameLogic.Instance.mUILogic = new UILogicController();
            //GameLogic.Instance.mUILogic.Init();
            UIEffectManager.Instance.Dispose();
            InitData();
            Init();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}