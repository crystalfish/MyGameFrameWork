using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 说明：
/// 1.每个页面都认为是一个Panel，大厅为常驻底层Panel，上面叠加各种功能Panel
/// 2.BasePanel类是个非Mono类，相当于MVC中M和C，V是BasePanel的属性，这样可以在界面隐藏状态下，依然能够监听处理事件数据，在页面打开的时候再更新界面信息
/// 3.Panel内的部件prefab上尽量不挂脚本，除非他的功能比较独立，尽量在Panel内集中处理
/// 
/// 创建Panel步骤：
/// 1.新建继承BasePanel的子类，添加构造函数、实现IPanel接口函数框架
/// 2.在构造函数中设置Panel的Prefab路径，并添加预加载资源信息。
/// 3.在Init接口中获取对象引用，以及其他初始化操作
/// 4.在InitEvent接口中注册UI事件或全局事件
/// 5.在OnShow、OnHide、OnDestroy、Update函数中写逻辑
/// 
/// Tips：
/// 1.基类BasePanel提供了m_go和m_tran，方便获取Panel的Prefab实例化后的GameoObject及其RectReansform对象
/// 2.执行ShowPanel后，先立即执行构造函数，等待资源加载完毕后，依次同步执行Init()，InitEvent()，OnShow()，在Panel没销毁之前再打开，就只执行OnShow()了
/// 3.对于像房间列表这种一打开UI就需要服务端数据的，可以在构造函数内向后端请求数据并添加监听，在后端消息返回和OnShow函数分别响应处理数据
/// 4.如果需要在页面没打开之前就能监听消息，可以写在UIManager类的RegedistServerMsg方法中
/// </summary>
/// 
namespace COMMON
{
   

    /// <summary>
    /// 面板类型
    /// </summary>
    public enum EPanelType
    {
        /// <summary>
        /// 功能模块面板
        /// </summary>
        Content,
        /// <summary>
        /// 弹框面板
        /// </summary>
        ContentTips,
        /// <summary>
        /// 附属面板
        /// </summary>
        Component,
        /// <summary>
        /// 面板中的子面板
        /// </summary>
        SubContent,
    }


    /// <summary>
    /// 打开topView时传递的参数
    /// </summary>
    public class OpenTopViewParam
    {
        /// <summary>
        /// 面板类型
        /// </summary>
        public int panelTitleTextId;

        /// <summary>
        /// 帮助按钮弹出框内容配置id
        /// </summary>
        public int panelHelpContentId;
        /// <summary>
        /// 帮助按钮弹框标题id
        /// </summary>
        public int panelHelpTitleId;
        /// <summary>
        /// 当前打开的面板的id
        /// </summary>
        public int panelId;
        /// <summary>
        /// 面板传递过来的参数（可不传）
        /// </summary>
        public int param;

        /// <summary>
        /// 父物体
        /// </summary>
        public BasePanel parentPanel;

        /// <summary>
        /// 附属面板在面板中的层级
        /// </summary>
        public int localUISibling;
        /// <summary>
        /// 执行关闭面板时的回调，如果该函数返回false,
        /// 则不会关闭面板
        /// </summary>
        public Func<bool> preCallbackBeforClose;
    }

    /// <summary>
    /// 附属面板参数
    /// </summary>
    public class AffiliatedPanelParam
    {
        public int ModelId;
        public string PanelName;
        public object Params;
        public int IntParam = int.MinValue;
    }



    public abstract class BasePanel : IPanel, System.IDisposable
    {

        public GameObject GetGameObject()
        {
            return m_go;
        }

        protected GameObject m_go;
        protected RectTransform m_tran;
        protected Canvas m_canvas;
        protected CanvasGroup m_canvasGroup;
        private List<string> m_resList = new List<string>();
        /// <summary>
        /// 附属面板，用于当面板已处于打开状态，再次打开时面板不会重新
        /// 走打开流程而是只是刷新层级时，可以同时刷新附属面板的层级，
        /// 保证附属面板显示在面板上层
        /// </summary>
        protected bool m_pixelPrefect = false;
        protected object[] m_params;
        private bool m_preloaded;
        private bool m_preloading;
        private bool m_isOpen;
        private bool m_isOpenLogic;

        public bool IsOpenLogic
        {
            get
            {
                return m_isOpenLogic;
            }
        }

        public bool m_isInited;
        protected bool m_isMainCameraActive = false;   //该面板打开时场景主相机状态
        public string m_prefabPath;
        /// <summary>
        /// 界面类名
        /// </summary>
        public string PanelClassName;
        /// <summary>
        /// 是否需要显示上层面板，比如一些带半透背景的面板或面板中的子面板
        /// </summary>
        public bool NeedShowPrePanel = false;

        string mAddtitionalDesc=string.Empty;
        public static int mMaxCanvasPanelOrder = 1;
        Canvas mCanvas;

        public bool DontDestroyOnLoad { get; set; }
        /// <summary>
        /// 记录当前最大层级,每打开新的面板，都要添加Canvas组件，并根据
        /// 当前最大层级来设置面板的order，确保新的面板层级大于其他面板
        /// 这样来可以控制不同面板间特效与UI的层次关系
        /// </summary>
        private List<AffiliatedPanelParam> m_affiliatedPanels = new List<AffiliatedPanelParam>();
        private BasePanel m_parentPanel = null;
        public BasePanelLogic mLogic;
        
        protected EPanelType panelType = EPanelType.Component;
        protected bool isRoot = false;//是否是根面板

        protected void SetPanelPrefabPath(string path)
        {
            m_prefabPath = path;
        }

        protected void AddPreLoadRes(string path)
        {
            m_resList.Add(path);
        }

        public List<string> GetPreloadRes()
        {
            return m_resList;
        }

        public void SetParams(object[] @params)
        {
            m_params = @params;
        }

        /// <summary>
        /// 添加附属面板（比如底部面板等)
        /// </summary>
        /// <param name="param"></param>
        public void AddAffiliatedPanel(AffiliatedPanelParam param)
        {
            
            if (null != m_affiliatedPanels)
            {
                if (!m_affiliatedPanels.Contains(param))
                {
                    m_affiliatedPanels.Add(param);
                }
            }
        }

        /// <summary>
        /// 根面板打开时返回键会弹出退出弹框，
        /// 其他面板会返回到上一面板
        /// </summary>
        /// <returns></returns>
        public bool IsRootPanel()
        {
            return isRoot;
        }

        public void SetParent(BasePanel parent)
        {
            m_parentPanel = parent;
        }

        public void SetActive(bool isActive)
        {
            m_isOpenLogic = isActive;
            if (m_go != null)
            {
                m_isOpen = isActive;
                m_go.SetActive(isActive);
            }
        }

        public Transform transform
        {
            get { return m_tran; }
        }


        /// <summary>
        /// 每次打开面板，更新面板层级，保证该面板
        /// 显示在最上层
        /// </summary>
        public void UpdatePanelSenderOrder()
        {
            SetUISenderOrder();
        }

        public void ShowPanel(Transform parent = null, bool addMask = false,
            Action onShow = null)
        {
            m_isOpenLogic = true;
            ///主功能面板打开时考虑是否关闭场景相机
            ///减少GPU功耗
            //if (panelType == EPanelType.Content &&
            //    !GlobalFunction.Instance.IsInBattleScene())
            //{
            //    GlobalFunction.Instance.SetMainCameraEnable(m_isMainCameraActive);
            //}
            if (m_preloaded)
            {
                ShowObject(parent, onShow);
            }
            else
            {
                //m_preloading = true;
                if (m_go == null)
                {
                    var loader = ResManager.Instance.Load(m_prefabPath);
                    if (loader != null && loader.mMainAsset != null)
                    {
                        GameObject objPreFabe = GameObject.Instantiate(loader.mMainAsset) as GameObject;
                        m_go = objPreFabe;
                        m_tran = m_go.GetComponent<RectTransform>();
                        m_tran.name = loader.mMainAsset.name;
                        //callback(m_go);
                        InitObj(parent, onShow);
                    }
                    else
                    {
                        Logger.LogError("BasePanel.ShowPanel----Path Not Found：" + m_prefabPath);
                    }
                }
                else
                {
                    //callback(m_go);
                    InitObj(parent, onShow);
                }
            }
        }

        public void UpdatePanelSortingOrder()
        {
            SetUISenderOrder();
        }

        /// <summary>
        /// 设置UI面板的层级
        /// </summary>
        protected virtual void SetUISenderOrder()
        {
            if (null == m_go)
            {
                return;
            }
            mCanvas = m_go.GetComponent<Canvas>();
            if (null == mCanvas)
            {
                return;
            }
            if (mCanvas.sortingOrder <= mMaxCanvasPanelOrder)
            {
                mMaxCanvasPanelOrder += 20;
            }
            else
            {
                mMaxCanvasPanelOrder = mCanvas.sortingOrder;
            }
            mCanvas.overrideSorting = true;
            mCanvas.sortingOrder = mMaxCanvasPanelOrder;

            //刷新子面板层级
            Canvas[] childCanvas = m_go.GetComponentsInReelChildren<Canvas>(true);
            if (null != childCanvas)
            {
                for (int i = 0, j = childCanvas.Length; i < j; ++i)
                {
                    var item = childCanvas[i];
                    //子面板中的canvas层级按4的倍数累加，为子面板中的特效设置层级预留空间
                    item.overrideSorting = true;
                    item.sortingOrder = mCanvas.sortingOrder + (i + 1) * 4;
                }
            }

            //刷新附属面板层级
            if (null != m_affiliatedPanels)
            {
                for (int i = 0; i < m_affiliatedPanels.Count; ++i)
                {
                    AffiliatedPanelParam param = m_affiliatedPanels[i];
                    if (null != param && !string.IsNullOrEmpty(param.PanelName))
                    {
                        BasePanel panel = UIPanelManager.GetPanel(param.PanelName);
                        if (null != panel && panel.IsOpen())
                        {
                            panel.SetUISenderOrder();
                        }
                    }
                }
            }
            GraphicRaycaster rayCaster = m_go.GetComponent<GraphicRaycaster>();
            if (null == rayCaster)
            {
                rayCaster = m_go.AddComponent<GraphicRaycaster>();
            }
            rayCaster.enabled = true;

        }


        void InitObj(Transform parent, Action onShow = null)
        {
            m_preloaded = true;
            m_isInited = true;
            var tran = m_go.GetComponent<RectTransform>();

            tran.SetUILocation(parent, 0, 0);
            tran.localScale = new Vector3(1f, 1f, 1f);
            tran.offsetMin = new Vector2(0f, 0f);
            tran.offsetMax = new Vector2(0f, 0f);
            tran.anchorMin = new Vector2(0f, 0f);
            tran.anchorMax = new Vector2(1f, 1f);


            Init();
            InitEvent();
            ShowObject(parent, onShow);
        }


        void ShowObject(Transform parent, Action onShow = null)
        {
            
            m_isOpen = true;
            SetUISenderOrder();
            OpenAffiliatedPanels();

            m_go.SetActive(true);
            var tran = m_go.GetComponent<RectTransform>();

            if (null != m_params && m_params.Length > 0)
            {
                var param = m_params[0] as OpenTopViewParam;
                if (null != param  && null != param.parentPanel)
                {
                    tran.SetUILocation(param.parentPanel.GetGameObject().transform);
                    tran.SetSiblingIndex(param.localUISibling);
                }
                else
                {
                    tran.SetUILocation(parent, 0, 0);
                }
            }
            else
            {
                tran.SetUILocation(parent, 0, 0);
            }
            //tran.SetAsLastSibling();
            //打开附属面板
            OnShow();
            if (onShow != null)
                onShow();
            PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_OPEN_NOTIFY, this);
        }

        /// <summary>
        /// 手动关闭
        /// 添加该函数主要是因为有时会从其他地方关闭面板，而不是
        /// 通过返回按钮关闭，这两种途径的处理或许会不一样
        /// 比如组队面板手动通过关闭按钮关闭时需要清除组队信息，而如果是开始
        /// 匹配后也会自动关闭面板，但是这时不能清除组队信息
        /// </summary>
        public virtual void CloseManually()
        {

        }

        //打开附属面板
        public void OpenAffiliatedPanels()
        {
            if (null == m_affiliatedPanels)
            {
                return;
            }
            for (int i = 0, n = m_affiliatedPanels.Count; i < n; ++i)
            {
                AffiliatedPanelParam param = m_affiliatedPanels[i];
                if (null != param && param.ModelId > 0)
                {
                    ///把object参数和int类型参数区分开来是为了调用不同的
                    ///PanelEvents事件，避免不必要的装箱
                    if (param.IntParam != int.MinValue)
                    {
                        UIPanelManager.OpenPanelByID(param.ModelId, param.IntParam);
                    }
                    else
                    {
                        UIPanelManager.OpenPanelByID(param.ModelId, param.Params);
                    }
                }
            }
        }



        public void HidePanel(bool bOnClose = true)
        {
            m_isOpenLogic = false;
            if (!m_isOpen || m_go == null)
            {
                return;
            }

            m_isOpen = false;
            m_preloading = false;
            //NetLayer.RemoveHandler(this);
            m_go.SetActive(false);

            //只有最高层面板关闭时刷新层次记录
            if (null != mCanvas)
            {
                int canvasOrder = mCanvas.sortingOrder;
                if (canvasOrder >= mMaxCanvasPanelOrder)
                {
                    mMaxCanvasPanelOrder -= 20;
                }
            }


            
            if (bOnClose)
                OnClose();

            OnHide();

           

            if (null != m_affiliatedPanels)
            {

                for (int i = 0; i < m_affiliatedPanels.Count; ++i)
                {
                    AffiliatedPanelParam param = m_affiliatedPanels[i];
                    if (null != param && !string.IsNullOrEmpty(param.PanelName))
                    {
                        BasePanel panel = UIPanelManager.GetPanel(param.PanelName);
                        if (null != panel && panel.IsOpen())
                        {
                            panel.HidePanel();
                        }
                    }
                }
            }

            //通知关闭界面
            PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_CLOSE_NOTIFY, this);
        }

        public bool IsOpen()
        {
            return m_isOpen;
        }

        public bool IsInited()
        {
            return m_isInited;
        }

        /// <summary>
        /// 设置logic
        /// </summary>
        /// <param name="logic"></param>
        protected void SetLogic(BasePanelLogic logic)
        {
            mLogic = logic;
        }


        //卸载面板上所有用到的image，rawimage等
        public void UnloadPanelImg()
        {
            if (null == m_go)
            {
                return;
            }

            List<Image> imgList = new List<Image>();
            GetImages(m_go.transform, imgList);
            foreach (var item in imgList)
            {
                item.sprite = null;
            }

            List<RawImage> rawList = new List<RawImage>();
            GetRawImages(m_go.transform, rawList);
            foreach (var item in rawList)
            {
                item.texture = null;
            }

            imgList.Clear();
            rawList.Clear();
        }

        private void GetImages(Transform tf, List<Image> imgList)
        {
            if (tf != null)
            {
                Image img = tf.GetComponent<Image>();
                if (img != null)
                {
                    imgList.Add(img);
                }

                int child = tf.childCount;
                if (child > 0)
                {
                    for (int i = 0; i < child; i++)
                    {
                        GetImages(tf.GetChild(i), imgList);
                    }
                }
            }
        }

        private void GetRawImages(Transform tf, List<RawImage> imgList)
        {
            if (tf != null)
            {
                RawImage img = tf.GetComponent<RawImage>();
                if (img != null)
                {
                    imgList.Add(img);
                }

                for (int i = 0; i < tf.childCount; i++)
                {
                    GetRawImages(tf.GetChild(i), imgList);
                }
            }
        }

        /// <summary>
        /// 获取面板上用到的所有Texture
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTexturesUsedInPanel()
        {
            List<string> listSprites = new List<string>();
            listSprites.Clear();

            List<Image> imgList = new List<Image>();
            GetImages(m_go.transform, imgList);
            foreach (var item in imgList)
            {
                if (null != item && null != item.sprite && null != item.sprite.texture)
                {
                    listSprites.Add(item.sprite.texture.name);
                }
            }

            List<RawImage> rawList = new List<RawImage>();
            GetRawImages(m_go.transform, rawList);
            foreach (var item in rawList)
            {
                if (null != item && null != item.mainTexture)
                {
                    listSprites.Add(item.mainTexture.name);
                }
            }

            return listSprites;
        }

        public void DestroyPanel()
        {
            

            m_canvasGroup = null;
            m_canvas = null;
            m_tran = null;
            mCanvas = null;
            m_parentPanel = null;
            

            m_isOpenLogic = false;

            m_isOpen = false;
            m_isInited = false;
            m_preloaded = false;
            m_preloading = false;
            DetroyUIDescInfo();

            if (null != mLogic)
            {
                mLogic.OnDestroy();
            }
            else
            {
                OnHide();
                //OnDestroy();
            }


            if (null != m_affiliatedPanels)
            {

                for (int i = 0; i < m_affiliatedPanels.Count; ++i)
                {
                    AffiliatedPanelParam param = m_affiliatedPanels[i];
                    if (null != param && !string.IsNullOrEmpty(param.PanelName))
                    {
                        BasePanel panel = UIPanelManager.GetPanel(param.PanelName);
                        if (null != panel)
                        {
                            panel.DestroyPanel();
                        }
                    }
                }
                m_affiliatedPanels.Clear();
            }

            if (m_go == null)
                return;

            GameObject.DestroyImmediate(m_go);
            m_go = null;

            Dispose();
        }


        /// <summary>
        /// 销毁面板上所有UIdescInfo组件
        /// </summary>
        void DetroyUIDescInfo()
        {
            if (null == m_go)
            {
                return;
            }

            //List<UIDescInfo> descList = new List<UIDescInfo>();
            //GetDescInfos(m_go.transform, descList);
            //for (int i = 0; i < descList.Count; ++i)
            //{
            //    UIDescInfo descInfo = descList[i];
            //    descInfo = descInfo.TryDispose();
            //}

            //descList.Clear();
        }

        //private void GetDescInfos(Transform tf, List<UIDescInfo> descList)
        //{
        //    if (tf != null)
        //    {
        //        UIDescInfo desc = tf.GetComponent<UIDescInfo>();
        //        if (desc != null)
        //        {
        //            descList.Add(desc);
        //        }

        //        for (int i = 0; i < tf.childCount; i++)
        //        {
        //            GetDescInfos(tf.GetChild(i), descList);
        //        }
        //    }
        //}

        public float Alpha
        {
            get
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                    if (m_canvasGroup == null)
                        m_canvasGroup = m_go.AddComponent<CanvasGroup>();
                }
                return m_canvasGroup.alpha;
            }
            set
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                    if (m_canvasGroup == null)
                        m_canvasGroup = m_go.AddComponent<CanvasGroup>();
                }
                m_canvasGroup.alpha = value;
            }
        }

        public bool EnableMouseEvent
        {
            get
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                    if (m_canvasGroup == null)
                        m_canvasGroup = m_go.AddComponent<CanvasGroup>();
                }
                return m_canvasGroup.blocksRaycasts;
            }
            set
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = m_go.GetComponent<CanvasGroup>();
                    if (m_canvasGroup == null)
                        m_canvasGroup = m_go.AddComponent<CanvasGroup>();
                }
                m_canvasGroup.blocksRaycasts = value;
            }
        }
        public Transform FindChild(string name)
        {
            if (null == m_go) { return null; }

            Transform tf = m_go.transform.Find(name);
            if (tf == null)
            {
                return null;
            }

            return tf;
        }

        public GameObject FindChildObj(string name)
        {
            Transform tf = FindChild(name);
            if (tf == null)
            {
                return null;
            }

            return tf.gameObject;
        }

        public T GetChildUIScript<T>(string name)
        {
            return GetChildUIScript<T>(name, m_go);
        }

        public T GetChildUIScript<T>(string name, GameObject uiObj)
        {
            if (uiObj != null)
            {
                Transform tra = string.IsNullOrEmpty(name) ? uiObj.transform : uiObj.transform.Find(name);
                if (tra != null)
                {
                    return tra.gameObject.GetComponent<T>();
                }
            }

            return default(T);
        }

        public bool IsActiveInHierarchy()
        {
            return m_go != null ? m_go.activeInHierarchy : false;
        }

        public virtual int GetPanelID()
        {
            return E_PanelID.NULL;
        }


        /// <summary>
        /// 面板类型
        /// </summary>
        public virtual EPanelType PanelType
        {
            set { panelType = value; }
            get { return panelType; }
        }


        /// <summary>
        /// 获取面板tag
        /// </summary>
        public string GetPanelName()
        {
            return this.PanelClassName;
            //string tag = string.Empty;
            //if (null != m_go)
            //{
            //    tag = m_go.name;
            //}
            //return tag;
        }


        public virtual void SetAdditionalDesc(string note)
        {
            mAddtitionalDesc = note;
        }

        /// <summary>
        /// 用于额外描叙信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetAdditionalDesc()
        {
            return mAddtitionalDesc;
        }

        public abstract void Init();
        public abstract void InitEvent();
        public abstract void OnShow();
        public abstract void OnHide();
        public abstract void OnBack();
        public abstract void OnDestroy();
        public abstract void Update();

        /// <summary>
        /// 界面按X关闭或按esc关闭时调用
        /// </summary>
        public virtual void OnClose()
        {
            if (null != CloseTodoEvent)
            {
                CloseTodoEvent();
            }
        }

        public Action CloseTodoEvent;

        /// <summary>
        /// 标记是否已经销毁了
        /// </summary>
        private bool mDisposed = false;
        /// <summary>
        /// 销毁资源
        /// </summary>
        public virtual void Dispose()
        {
            if (!mDisposed)
            {
                mDisposed = true;
            }//if
            else
            {
                return;
            }//else

            DetroyUIDescInfo();
            //消除引用
            m_go = m_go.TryDispose();
            m_tran = m_tran.TryDispose();
            m_canvasGroup = m_canvasGroup.TryDispose();
            m_resList = m_resList.TryDispose();
            m_params = m_params.TryDispose();
            mCanvas = mCanvas.TryDispose();
            m_affiliatedPanels = m_affiliatedPanels.TryDispose();
            m_parentPanel = m_parentPanel.TryDispose();
            mLogic = mLogic.TryDispose();
        }//Dispose
    }//BasePanel
}//COMMON