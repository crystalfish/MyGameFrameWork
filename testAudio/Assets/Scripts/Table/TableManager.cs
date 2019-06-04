using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace COMMON
{
    public delegate void WWWAsyncCallback(WWW www);

    public class WWWAsyncProcess
    {
        private Dictionary<string, WWWAsyncCallback> wwwCallbacks = new Dictionary<string, WWWAsyncCallback>();

        public Dictionary<string, WWWAsyncCallback> WWWCallbacks
        {
            get { return wwwCallbacks; }
        }

        public void Add(string key, WWWAsyncCallback value)
        {
            lock (wwwCallbacks)
            {
                wwwCallbacks.Add(key, value);
            }
        }
    };

    public class TableManager : MonoBehaviour
    {
        public static WWWAsyncProcess wwwAsyncProcess = new WWWAsyncProcess();

        /// <summary>
        /// 已经加载完的配置列表
        /// </summary>
        public static Dictionary<string, byte[]> mLoadedTables = new Dictionary<string, byte[]>();


        void ReadTable<T>(WWW www, T loader) where T : ReaL.ILoader, new()
        {
            ReadTable<T>(www.bytes, loader);
        }

        void ReadTable<T>(byte[] bytes, T loader) where T : ReaL.ILoader, new()
        {
            using (Stream stream = new MemoryStream(bytes))
            {
                loader.ReadFile(stream);
                stream.Dispose();
            }
        }

        public void ReadTable_Sync<T>(T loader) where T : ReaL.ILoader, new()
        {
            string fileName = loader.GetFileName();
            try
            {
                string path = GetDestPath(fileName);
                byte[] bytes = StreamTool.OpenRead(path);
               
                ReadTable(bytes, loader);

            }
            catch (Exception ex)
            {
                Logger.LogError("TableManager.ReadTable_Sync----Error---FileName:" + fileName + "\n" + ex.ToString());
            }
        }

        string GetDestPath(string str)
        {
            return string.Format("{0}/{1}.bytes", DirTool.StreamingPath, str);
        }

        //所有配置加载完毕
        public static string ALLCONFIGS_LOAD_COMPLETE = "ALLCONFIGS_LOAD_COMPLETE";
        private static TableManager s_instance = null;
        public static TableManager Instance
        {
            get
            {
                if (!s_instance)
                {
                    s_instance = GameObject.FindObjectOfType(typeof(TableManager)) as TableManager;
                    if (!s_instance)
                    {
                        GameObject container = new GameObject();
                        container.name = "cTableManager";
                        s_instance = container.AddComponent(typeof(TableManager)) as TableManager;
                    }
                }
                return s_instance;
            }
        }

        void Awake()
        {
            s_instance = this;
        }

        public void TableLoad()
        {
            StartCoroutine("GameTableLoad");
        }

        /// <summary>
        /// 在未运行编辑器的时候调用，免得一直弹错误
        /// </summary>
        public void TableLoadInEditor()
        {
            //异步转同步的实现
            var en = EditorTableLoad();
            while (en.MoveNext()) ;
        }

        IEnumerator GameTableLoad()
        {
            Logger.Log("_TableLoad");

            wwwAsyncProcess.WWWCallbacks.Clear();
            {
                //英雄表
                { var loader = new TBL.WOLF_BASE.Loader(); ReadTable_Sync(loader); }
                yield return 0;
            }
            //E_GameEvent.sNotifier.DispatchAndRemove(E_GameEvent.TABLE_CONFIG_READY);
            Logger.Log("_TableLoad-----Finished");

            yield return 0;
        }


        IEnumerator EditorTableLoad()
        {
            Logger.Log("_EditorTableLoad");

            wwwAsyncProcess.WWWCallbacks.Clear();
            {
                ////英雄表
                { var loader = new TBL.WOLF_BASE.Loader(); ReadTable_Sync(loader); }
                yield return 0;
            }     
            Logger.Log("_EditorTableLoad-----Finished");

            yield return 0;
        }
    }//TableManager
}//namespace COMMON