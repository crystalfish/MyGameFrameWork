using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace COMMON
{
    /// <summary>
    /// 这个类用来转换加载路径
    /// </summary>
    public class DirTool
    {

        public static string BUNDLE_EXTENSION = ".ab";
      
		public static Dictionary<string, string> PathMap = new Dictionary<string, string>();
        /// <summary>
        /// 生成ab文件的路径
        /// </summary>
        /// <param name="abPath"></param>
        /// <returns></returns>
        public static string MakeABPath(string abPath)
        {
            return string.Format("{0}/{1}", PersistantPath, abPath);
        }

        public static string GetBundleName(string resPath)
        {
			if (PathMap.ContainsKey(resPath)) {
				return PathMap[resPath];
			}
			return "";
        }

        public static string Combine(string pathA ,string pathB)
        {
            return "";
            //return StringTools.Format("{0}/{1}",pathA,pathB);
        }

        /// <summary>
        /// 存储路径
        /// </summary>
        public static string PersistantPath { get;private set; }
        /// <summary>
        /// 获取手机加载路径
        /// </summary>
        public static string StreamingPath { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        static DirTool()
        {
            PersistantPath = Application.persistentDataPath;

            switch(Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    StreamingPath = Application.dataPath + "/Raw";
                    Logger.Log("IOS Path：" + StreamingPath);
                    break;
                case RuntimePlatform.Android:
                    StreamingPath = Application.streamingAssetsPath;
                    Logger.Log("Android Path：" + StreamingPath);
                    break;
                default:
                    StreamingPath = Application.dataPath + "/StreamingAssets";
                    Logger.Log("Windows Path：" + StreamingPath);
                    break;
            }
        }

        //public static void LoadResInfoMap()
        //{
        //    var path = PersistantPath + "/ABResMap.bytes";
        //    if (!mLoadedMap && System.IO.File.Exists(path))
        //    {
        //        mMap.ReadFromFile(path);
        //        mMap.PrintDebugInfo();
        //    }
        //}

        //private static bool mLoadedMap = false;

        ///// <summary>
        ///// 描述资源路径和依赖资源路径的映射表
        ///// </summary>
        //private static ABResInfoMap mMap = new ABResInfoMap();
    }//end class
}//end namespace