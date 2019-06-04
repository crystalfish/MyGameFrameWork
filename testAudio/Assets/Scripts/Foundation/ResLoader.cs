using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace COMMON
{
    /// <summary>
    /// 用来装载资源的工具类
    /// </summary>
    public class ResLoader : Ref
    {

        public bool bIsShowHeroRes = false;
        /// <summary>
        /// 销毁游戏资源
        /// </summary>
        public override void Dispose()
        {
            if(null != mAssetBundle)
            {
                //if(!bIsShowHeroRes)
                //    AssetBundleLoadManager.Instance.UnloadAll(true);
                //else
                //{
                //    AssetBundleLoadManagerForShowHero.Instance.UnloadAll(true);

                //}

            }
        }
        public void DisposeFalse()
        {
            if (null != mAssetBundle)
            {
                //if (bIsShowHeroRes)
                //    AssetBundleLoadManagerForShowHero.Instance.UnloadAll(false);
                //else
                //    AssetBundleLoadManager.Instance.UnloadAll(false);

            }
        }
        /// <summary>
        /// 加载资源
        /// </summary>
        public ResLoader LoadFromBundle()
        {
            if(null != mMainAsset)
            {
                return this;
            }
            else
            {
                string assetName = Path.GetFileNameWithoutExtension(mPath);
    //            mAssetBundle = AssetBundleLoadManager.Instance.LoadAssetBundle(DirTool.GetBundleName(mPath));
				//if (mAssetBundle!= null) {
				//	mMainAsset = mAssetBundle.LoadAsset(assetName);
				//}
            }
            return this;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        public ResLoader LoadFromBundleForHero()
        {
            if (null != mMainAsset)
            {
                return this;
            }
            else
            {
                string assetName = Path.GetFileNameWithoutExtension(mPath);
                //mAssetBundle = AssetBundleLoadManagerForShowHero.Instance.LoadAssetBundle(DirTool.GetBundleName(mPath));
                //if (mAssetBundle != null)
                //{
                //    mMainAsset = mAssetBundle.LoadAsset(assetName);
                //}
            }
            return this;
        }
        public ResLoader LoadFromResource()
        {
            if(null != mMainAsset)
            {
                return this;
            }
            else
            {
                mMainAsset = Resources.Load(mPath);
            }
            return this;
        }

        public ResourceRequest LoadFromResourceAsync()
        {
            if (null == mResourceRequest)
            { 
                mResourceRequest = Resources.LoadAsync(mPath);
            }
            return mResourceRequest;
        }

        /// <summary>
        /// 加载全部的资源
        /// </summary>
        public ResLoader  LoadAll()
        {
            if (null != mVec)
            {
                return this;
            }
            else
            {
#if USE_ASSETBUNDLE
				if (mAssetBundle!=null) {
					mVec = mAssetBundle.LoadAllAssets();
				}
               	else {
					mVec = Resources.LoadAll(mPath);
               	}
#else
                mVec = Resources.LoadAll(mPath);
#endif
            }
            return this;
        }

        public T GetResFromVec<T>(string resName) where T : UnityEngine.Object
        {
            if (null == mVec)
            {
                Logger.LogError("You should call LoadAll first");
                return null;
            }

            T ret = null;
            for(int i=0,n=mVec.Length;i<n;++i)
            {
                var it = mVec[i];
                if(it is T)
                {
                    if(it.name.Equals(resName))
                    {
                        ret = it as T;
                        break;
                    }
                }
            }
            return ret;
        }

        

        /// <summary>
        /// 获取真正的资源路径
        /// </summary>
        public string RealPath
        {
            get
            {
                var ret = string.Empty;
                do
                {
                    if(null == mPath)
                    {
                        break;
                    }
                    return DirTool.MakeABPath(DirTool.GetBundleName(mPath));
                } while (false);
                mRealPath = ret;
                return ret;
            }
        }

        /// <summary>
        /// 用来加载资源使用的
        /// </summary>
        private AssetBundle mAssetBundle;
        /// <summary>
        /// 资源路径
        /// </summary>
        private string mPath;
        /// <summary>
        /// 资源的真正路径
        /// </summary>
        private string mRealPath;

        public string ResPath
        {
            get
            {
                return mPath;
            }
            set
            {
                mPath = value;
            }
        }


        /// <summary>
        /// 重置数据
        /// </summary>
        public void Reset()
        {
            Dispose();
            mPath = null;
            mRealPath = null;
            mMainAsset = null;
            mVec = null;
            bIsShowHeroRes = false;
            mResourceRequest = null;
            mResourceRequest = null;
        }

        /// <summary>
        /// 异步加载的时候可用
        /// </summary>
        public ResourceRequest mResourceRequest;

        /// <summary>
        /// 主资源
        /// </summary>
        public UnityEngine.Object mMainAsset;
        /// <summary>
        /// 存放了所有的资源，针对LoadAll方法获取到的情况
        /// </summary>
        public UnityEngine.Object[] mVec;

        //public Dictionary<string, UnityEngine.Object> mMap;
        /// <summary>
        /// 对象池
        /// </summary>
        private static readonly XPool<ResLoader> sPool = new XPool<ResLoader>();
        /// <summary>
        /// 获取数据节点
        /// </summary>
        /// <returns></returns>
        public static ResLoader Get()
        {
            ResLoader ret = sPool.Get();
            ret.Reset();
            return ret;
        }
    }//end class
}//end namesapce

