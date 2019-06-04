using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMON
{
    public class GameLogic : MonoBehaviour
    {
        private void Awake()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
            UIPanelManager.Init();
            UILogicController ui = new UILogicController();
            ui.Init();

            CreateCache();
            LoadConfig();
        }

        // Update is called once per frame
        void Update()
        {

            SimpleSingleton<MAudioManager>.Instance.DoUpdate();
            GameCache.Instance.gGlobalTimer.Tick(Time.deltaTime);
        }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        private void LoadConfig()
        {
            TableManager.Instance.TableLoad();
        }

       
        private void OnGUI()
        {
            if (GUI.Button(new Rect(100, 100, 100, 100), "打开登录界面"))
            {
                PanelEvents.notifier.DispatchCmd(PanelEvents.PANEL_OPEN_LOGIN_VIEW);
                //COMMON.UIEffectManager.Instance.CreateEffect(eUIEffectType.BulletEmpty,ig , new Vector3(0,0,-1f));
            }
        }
        public void CreateCache()
        {
            try
            {
                GameCache.CreateInstance();
                GameCache.Instance.SetUp();
            }
            catch (System.Exception ex)
            {
                //COMMON.Logger.LogError(ex.ToString());
            }
        }
        
    }

}
