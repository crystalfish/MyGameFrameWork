using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
namespace COMMON
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResManager : SimpleSingleton<ResManager>
    {
        /// <summary>
        /// 资源路径对Loader的映射表
        /// </summary>
        public static XDictionary<string, ResLoader> sMap
            = new XDictionary<string, ResLoader>();


       

        /// <summary>
        /// persistant目录的文件名对应的loader的映射表
        /// </summary>
        public static XDictionary<string, ResLoader> sMapDependsResLoader
            = new XDictionary<string, ResLoader>();


        public static void Purge()
        {
            var L = sMap.GetList();
            for (int i = 0, n = L.Count; i < n; ++i)
            {
                var kv = L[i];
                kv.Value.Reset();
            }
            sMapDependsResLoader.Clear();
            sMap.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        public ResLoader Load(string resPath, bool loadAll = false)
        {
            ResLoader ret = null;
            if (!resPath.Filled())
            {
                Logger.Log("resPath no Filled");
                return ret;
            }
            if (sMap.TryGetValue(resPath, out ret))
            {
                return ret;
            }
            else
            {
#if USE_ASSETBUNDLE
               ret = ResLoader.Get();
               sMap.Add(resPath, ret);
               ret.ResPath = resPath;
                if (File.Exists(ret.RealPath))
                {
                    ret.LoadFromBundle();
                }
                else
                {
                    ret.LoadFromResource();
                }

#else
                ret = LoadFromResource(resPath, loadAll);
#endif

            }
            return ret;
        }//end Load

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public ResLoader LoadFromResource(string resPath, bool loadAll = false)
        {
            ResLoader ret = null;
            if (sMap.TryGetValue(resPath, out ret))
            {
                return ret;
            }
            else
            {
                ret = ResLoader.Get();
                sMap.Add(resPath, ret);
                ret.ResPath = resPath;
                ret.LoadFromResource();
                if (loadAll)
                {
                    ret.LoadAll();
                }
            }
            return ret;
        }//end Load

       



        /// <summary>
        /// 加载展示模型和对应的特效资源
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public ResLoader LoadShowHeroResourcesAsync(string resPath, bool loadAll = false)
        {
            ResLoader ret = null;
            if (!resPath.Filled())
            {
                Logger.Log("resPath no Filled");
                return ret;
            }
            
            else
            {
#if USE_ASSETBUNDLE
               ret = ResLoader.Get();
              
               ret.ResPath = resPath;
               ret.bIsShowHeroRes = true; //需要设置这个标志，表示是展示英雄（选角）的资源
                if (File.Exists(ret.RealPath))
                {
                    ret.LoadFromBundleForHero();
                }
                else
                {
					Logger.Log("LoadFromResource："+resPath);
                    ret.LoadFromResourceAsync();
                    if (loadAll)
                    {
                        ret.LoadAll();
                    }
                }

#else
                //ret = LoadFromResourceForShowHeroAsync(resPath, loadAll);
#endif
            }
            return ret;
        }//end Load


        //private ResLoader LoadFromResourceForShowHeroAsync(string resPath, bool loadAll = false)
        //{
        //    ResLoader ret = null;
        //    if (sHeroShowMap.TryGetValue(resPath, out ret))
        //    {
        //        return ret;
        //    }
        //    else
        //    {
        //        ret = ResLoader.Get();
        //        sHeroShowMap.Add(resPath, ret);
        //        ret.ResPath = resPath;
        //        ret.bIsShowHeroRes = true; //需要设置这个标志，表示是展示英雄（选角）的资源
        //        ret.LoadFromResourceAsync();
        //        if (loadAll)
        //        {
        //            ret.LoadAll();
        //        }
        //    }
        //    return ret;
        //}//end Load


        /// <summary>
        /// 实际的加载展示模型和对应的特效资源功能
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        //private ResLoader LoadFromResourceForShowHero(string resPath, bool loadAll = false)
        //{
        //    ResLoader ret = null;
        //    if (sHeroShowMap.TryGetValue(resPath, out ret))
        //    {
        //        return ret;
        //    }
        //    else
        //    {
        //        ret = ResLoader.Get();
        //        sHeroShowMap.Add(resPath, ret);
        //        ret.ResPath = resPath;
        //        ret.bIsShowHeroRes = true; //需要设置这个标志，表示是展示英雄（选角）的资源
        //        ret.LoadFromResource();
        //        if (loadAll)
        //        {
        //            ret.LoadAll();
        //        }
        //    }
        //    return ret;
        //}//end Load
         /// <summary>
         /// 手动调用，当加载的数量超过一定的阈值的时候，调用，而且在Purge接口也会Clear HeroShowMap的资源
         /// </summary>
         /// <param name="resPath"></param>
         /// <returns></returns>
        public static void PurgeShowHeroRes()
        {
            //var LHero = sHeroShowMap.GetList();
            //for (int i = 0, n = LHero.Count; i < n; ++i)
            //{
            //    var kv = LHero[i];
            //    kv.Value.Reset();
            //}

            //sHeroShowMap.Clear();
            //AssetBundleLoadManagerForShowHero.Instance.UnloadAll(true);
            Resources.UnloadUnusedAssets();
      
        }

        public void RemoveRes(string resPath)
        {
            if (!resPath.Filled())
            {
                Logger.Log("resPath no Filled");
                return ;
            }
            ResLoader ret = null;

            if (sMap.TryGetValue(resPath, out ret))
            {
                ret.Reset();
                sMap.Remove(resPath);
            }
        }//end Load
    }//end class
}//end namespace

