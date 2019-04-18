using UnityEngine;
using System.Collections;

namespace COMMON
{
    public class MAudioObject
    {

        public GameObject mGo;

        public AudioSource mAudio;

        public Transform mTransform;

        /// <summary>
        /// 音效的非真正意义的父级（用于跟设置声效GameObj Trans）
        /// </summary>
        //public Transform mParent;
       public MAudio mAudioInfo;
       
        public MAudioObject()
        {
            InitObject();
        }
        private static int Count = 0;

        public void InitObject()
        {
            if (mGo == null)
            {
                ++Count;
                mGo = new GameObject("MAudioObject:"+Count);
                
                mAudio = mGo.AddComponent<AudioSource>();
                mAudio.loop = true;
                mAudio.enabled = false;
                mTransform = mGo.transform;
            }
        }

        public void Release(bool needRemoveFormMap = true)
        {
            if (null != mAudio)
            {
                
                mAudio.minDistance = 1f;
                mAudio.maxDistance = 500f;
                mAudio.pitch = 1f;
                mAudio.rolloffMode = AudioRolloffMode.Linear;
                mAudio.Stop();
                mAudio.clip = null;
            }
            if (mGo != null)
            {
                mGo.transform.SetParent(null);
                mGo.SetActive(false);
            }
            if (!inPool)
            {
                 GameCache.Instance.AudioObjectPool.Put(this);
                inPool = true;
            }
            //
            //mAudioInfo = null;
        }




        public void SetPosition(Vector3 pos) {
            if ( mTransform != null)
            {
                mTransform.position = pos;
            }
        }

        //public void DoUpdate()
        //{
        //    if (null != mParent && mTransform != null)
        //    {
        //        mTransform.position = mParent.position;
        //    }
        //}

       
        private bool inPool = false;
        public static MAudioObject Get(Vector3 position)
        {
            var it =  GameCache.Instance.AudioObjectPool.Get();
            if (it.mGo == null)
            {
                //有时候从池里面拿出来的GameObject是空的，在其他地方被Destroy了
                it.InitObject();
            }
            it.mGo.SetActive(true);
            //if (null != parent)
            //{
                it.mTransform.position = position;
            //}
            //it.mParent = parent;
            it.inPool = false;
            return it;
        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public static void CleanUpPool()
        {
             GameCache.Instance.AudioObjectPool.Clear();
        }
    }//end class AudioObject
}