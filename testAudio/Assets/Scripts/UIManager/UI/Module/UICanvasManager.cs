using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Author:joi
///用于创建各个层级的UIcamera，不能被销毁
/// </summary>
namespace COMMON
{
    class UICanvasManager : MonoBehaviour
    {
        protected static UICanvasManager sInstance;
        public static UICanvasManager Instance
        {
            get
            {
                return sInstance;
            }//get
        }//Instance


        //UICanvas
        GameObject mCanvas;
        GameObject mCanvas2;
        GameObject mTopCanvas;
        GameObject mEventSystem;
        GameObject mGameRoot;
        GameObject mTableManager;

        Camera mCameraCanvas;
        Camera mCameraCanvas2;
        Camera mCameraTopCanvas;


        //UI camera默认的layer
        private LayerMask mCanvasLayerMask = 0;
       
        private LayerMask mCanvas2LayerMask = 0;
        private LayerMask mPopCameraLayerMask = 0;
       

        private bool bEnterBattle = false; //是否在战斗场景中
        private bool bUIShowState = true;

        bool mIsGameRootExit = false;

        private float adjustorAspect = 1.0f;
        private float deviceAspect = 2f; //默认2：1，用于背景
        private float adjustorAspectbg = 1; 


        float standard_width = 1920;        //初始宽度  
        float standard_height = 1080;       //初始高度  
        float device_width = 0f;                //当前设备宽度  
        float device_height = 0f;               //当前设备高度  

        /// <summary>
        /// 获取Canvas
        /// </summary>
        public GameObject Canvas
        {
            get { return mCanvas; }
        }

        /// <summary>
        /// mCanvas2
        /// </summary>
        public GameObject Canvas2
        {
            get { return mCanvas2; }
        }

        /// <summary>
        /// mTopCanvas
        /// </summary>
        public GameObject TopCanvas
        {
            get { return mTopCanvas; }
        }

        /// <summary>
        /// Canvas2中的相机
        /// </summary>
        public Camera CameraCanvas2
        {
            get { return mCameraCanvas2; }
        }


        /// <summary>
        /// TopCanvas中的相机
        /// </summary>
        public Camera CameraTopCanvas
        {
            get { return mCameraTopCanvas; }
        }




        void Awake()
        {
            sInstance = this;
            InitAdjustorAspect();
            DontDestroyOnLoad(this.gameObject);
            CreateUICanvas();
            bUIShowState = true;
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// 创建UI系统
        /// </summary>
        public void CreateUICanvas()
        {
            //Canvas
            string path = "GameObjs/Canvas";
            GameObject obj = Resources.Load(path) as GameObject;
            if (null != obj)
            {
                mCanvas = GameObject.Instantiate<GameObject>(obj);
                mCanvas.name = "Canvas";
                mCanvas.transform.position = Vector3.left * 200f;

                mCameraCanvas = mCanvas.GetComponentInChildren<Camera>();
            }


            //Canvas2
            path = "GameObjs/Canvas2";
            obj = Resources.Load(path) as GameObject;
            if (null != obj)
            {
                mCanvas2 = GameObject.Instantiate<GameObject>(obj);
                mCanvas2.name = "Canvas2";
                mCanvas2.transform.position = Vector3.right * 200f;
                mCameraCanvas2 = mCanvas2.GetComponentInChildren<Camera>();
            }

            //TopCanvas
            path = "GameObjs/TopCanvas";
            obj = Resources.Load(path) as GameObject;
            if (null != obj)
            {
                mTopCanvas = GameObject.Instantiate<GameObject>(obj);
                mTopCanvas.name = "TopCanvas";
                mTopCanvas.transform.position = Vector3.down * 200f;

                mCameraTopCanvas = mTopCanvas.GetComponentInChildren<Camera>();
            }

           
            //Gameroot
            if (!mIsGameRootExit)
            {
                path = "GameObjs/GameRoot";
                obj = Resources.Load(path) as GameObject;
                if (null != obj)
                {
                    mGameRoot = GameObject.Instantiate<GameObject>(obj);
                    mGameRoot.name = "GameRoot";
                    mIsGameRootExit = true;
                }
            }

            //EventSystem
            path = "GameObjs/EventSystem";
            obj = Resources.Load(path) as GameObject;
            if (null != obj)
            {
                mEventSystem = GameObject.Instantiate<GameObject>(obj);
                mEventSystem.name = "EventSystem";
                var eventSystem = mEventSystem.GetComponent<EventSystem>();
                eventSystem.sendNavigationEvents = false;
                eventSystem.pixelDragThreshold = 8;
            }

            //TableManger
            path = "GameObjs/cTableManager";
            obj = Resources.Load(path) as GameObject;
            if (null != obj)
            {
                mGameRoot = GameObject.Instantiate<GameObject>(obj);
                mGameRoot.name = "TableManager";
            }

            //获取UI Camera MaskLayer ,进入战斗隐藏UI
            this.InitCameraMaskLayer();

        }//End CreateUICanvas

        /// <summary>
        /// 销毁UI系统
        /// </summary>
        public void DestoryUICanvas()
        {
            //销毁UI系统前先情况对各被销毁的canvas的引用
            //UIPanelManager.Instance.DestoryUIsystem();
            if (mCanvas != null)
            {
                GameObject.DestroyImmediate(mCanvas);
                mCanvas = mCanvas.TryDispose();
            }
            if (mCanvas2 != null)
            {
                GameObject.DestroyImmediate(mCanvas2);
                mCanvas2 = mCanvas2.TryDispose();
            }
            if (mTopCanvas != null)
            {
                GameObject.DestroyImmediate(mTopCanvas);
                mTopCanvas = mTopCanvas.TryDispose();
            }
            if (mEventSystem != null)
            {
                GameObject.DestroyImmediate(mEventSystem);
                mEventSystem = mEventSystem.TryDispose(); ;
            }
           
        }

       private void InitCameraMaskLayer()
        {
            //初始获取camera 默认cullingMask
            if (mCameraCanvas != null)
            {
                mCanvasLayerMask = mCameraCanvas.cullingMask;
            }
           
            if (CameraCanvas2 != null)
            {
                mCanvas2LayerMask = CameraCanvas2.cullingMask;
            }
			if (mCameraTopCanvas != null)
            {
                mPopCameraLayerMask = mCameraTopCanvas.cullingMask;
            }
           
        }

        public void DisableShowBattleUI()
        {

            if (bUIShowState == false)
                return;

            if (mCameraCanvas != null)
            {
                mCameraCanvas.cullingMask = 0;
            }
            if (CameraCanvas2 != null)
            {
                CameraCanvas2.cullingMask = 0;
            }
            if (mCameraTopCanvas != null)
            {
                mCameraTopCanvas.cullingMask = 0;
            }
            bUIShowState = false;
        }

        public void EnableShowBattleUI()
        {
            if (bUIShowState == true)
                return;
            if (mCameraCanvas != null)
            {
                mCameraCanvas.cullingMask = mCanvasLayerMask;
            }     
            if (CameraCanvas2 != null)
            {
                CameraCanvas2.cullingMask = mCanvas2LayerMask;
            }
            if (mCameraTopCanvas != null)
            {
                mCameraTopCanvas.cullingMask = mPopCameraLayerMask;
            }
            bUIShowState = true;
        }

        private void InitAdjustorAspect()
        {
            device_width = Screen.width;
            device_height = Screen.height;
            //计算宽高比例  
            float standard_aspect = standard_width / standard_height;
            deviceAspect = device_width / device_height;

            
            adjustorAspect = (deviceAspect / standard_aspect);

            adjustorAspectbg = deviceAspect / 2.0f; //背景图默认适应2：1，所以除以2

        }
        //用于调整canvas
        public float AdjustorAspect
        {
            get { return adjustorAspect; }
        }

        public float DeviceAspect
        {
            get { return deviceAspect; }
        }
        //用于调整背景
        public float AdjustorAspectBg
        {
            get { return adjustorAspectbg; }
        }
      
    }
}

