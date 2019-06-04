//joi
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System;
using System.Reflection;
namespace COMMON
{
    /// <summary>
    /// 音效资源所处状态
    /// </summary>
    public static class E_AudioPlayState
    {
        /// <summary>
        /// 加载阶段
        /// </summary>
        public const int Loading = 1;
        /// <summary>
        /// 延迟准备阶段
        /// </summary>
        public const int Ready = 2;
        /// <summary>
        /// 播放
        /// </summary>
        public const int Playing = 3;
        /// <summary>
        /// 暂停
        /// </summary>
        public const int Pause = 4;
        /// <summary>
        /// 休眠停止状态
        /// </summary>
        public const int Sleeping = 5;
    }

    public static class E_MAudioType
    {
        /// <summary>
        /// 英雄喊话
        /// </summary>
    public const int UIHUMAN = 1;
        /// <summary>
        /// 英雄战斗喊话
        /// </summary>
    public const int BATTLE_HUMAN = 2;
        /// <summary>
        /// 战斗声效
        /// </summary>
        /// 
    public const int BATTLE = 3;
        /// <summary>
        /// skillresconfig配置的
        /// </summary>
    public const int SKILL_RES_CONFIG = 4;
        /// <summary>
        /// 界面声效
        /// </summary>
    public const int VIEW_AUDIO = 5;
        /// <summary>
        /// 加载界面声效
        /// </summary>
    public const int LOADING_AUDIO = 6;
        /// <summary>
        /// 背景音乐
        /// </summary>
    public const int BGM = 7;

        /// <summary>
        /// 逻辑层用来表现自己的音效
        /// </summary>
    public const int PLAYLER_AUDIO = 8;

        /// <summary>
        /// 战斗内系统界面音效,包括回血，boss出现等等
        /// </summary>
    public const int BATTLE_VIEW = 9;
    }


    /// <summary>
    /// 单一音效基类
    /// </summary>
    public class MAudio : MBaseLogic
    {
        public MAudioObject mAudioObject;
        public Vector3 tfPos { get; set; }
        public  MAudioIdType audioId;
        private float clipLenght { get; set; }
        float mTotalPlayTime = 0f;
        bool mPlayWhenLoaded = true;
        int mSate = E_AudioPlayState.Sleeping;
  
        /// <summary>
        /// 根据配置表读取相对应的时间设置
        /// </summary>
        float  mLoopCount;
        private float duarationTime;
        private int audioType;
        bool isLoop = false;
        private Transform mParent;
        public bool IsPlayer(int playerId)
        {
            //var player = VMCache.Instance.GetPlayer();
            //if (player != null && player.Model != null)
            //{
            //    return player.Model.Uid == playerId;
            //}
            return false;
        }
        public bool IsMyteamate(int TeamId)
        {
            //var player = VMCache.Instance.GetPlayer();
            //if (player != null && player.Model != null)
            //{
            //    return player.Model.TeamId == TeamId;
            //}
            return false;
        }
        public int state { get { return mSate; } }
        public float targetVolume
        {
            get
            {
                return 1f;
            }
        }

        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <returns></returns>
        public static MAudio Get()
        {
            return  GameCache.Instance.audioPool.Get();
        }
        // 绑定audioObject对象
        public void AttachAudioObject(MAudioIdType configId  ,Transform tf, float durationTime, int audioType,

            bool loop, int loopCount) {
            
            audioId = configId;
            tfPos = tf.position;
            if(configId != MAudioIdType.None)
            {
                mSate = E_AudioPlayState.Loading;
                //加入特定的路径，也支持异步绑定
                curAudioBigType = MAudioManager.GetAudioBigType(CurAudioType);
                mTotalPlayTime = 0;
                InitAudioObject(configId, tfPos ,durationTime , audioType , loop , loopCount);
                mSate = E_AudioPlayState.Ready;
            }
            else
            {
                mSate = E_AudioPlayState.Sleeping;
            }
        }
        /// <summary>
        /// 初始化AudioObject
        /// </summary>
        /// <param name="path"></param>
        public void InitAudioObject(MAudioIdType configId ,Vector3 pos , float durTime, int audioType,

            bool loop, int loopCount)
        {
            mAudioObject = MAudioObject.Get(pos);
            var path = GetEffectPrefabPathByEffectType(configId);
            
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            this.duarationTime = durTime;
            this.audioType = audioType;
            this.isLoop = loop;
            this.mLoopCount = loopCount;
            var source = mAudioObject.mAudio;
            var loader = ResManager.Instance.Load(path, false);
            if (loader == null)
            {
#if DEBUG
                //Logger.LogError("InitAudioObject loader Not Found : " + path);
#endif
                return;
            }
            var clip = loader.mMainAsset as AudioClip;
            if (clip == null)
            {
#if DEBUG
                //Logger.LogError("InitAudioObject clip Not Found : " + path);
#endif
                return;
            }
            source.clip = clip;
            clipLenght = clip.length;

            if (curAudioBigType == E_MAudioBigType.EFFECT)
            {
                source.minDistance = 5;
                source.maxDistance = 1000;
                source.dopplerLevel = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.pitch = 1f;
                source.spatialBlend = 1F;
                source.ignoreListenerVolume = false;
                source.spread = 180f;
            }
            else
            {
                source.spatialBlend = 0f;
                GameObject.DontDestroyOnLoad(mAudioObject.mGo);
            }
            source.volume = targetVolume;
            mAudioObject.mGo.name = CurAudioType+"----"+path;
            mAudioObject.mAudioInfo = this;
            source.time = mTotalPlayTime%clipLenght;
            source.loop = isLoop;
        }
        float tempVom;
        /// <summary>
        /// 每个音频自循环
        /// </summary>
        public override void DoUpdate()
        {
            base.DoUpdate();
            switch (mSate)
            {
                case E_AudioPlayState.Ready:
                    if (  mAudioObject.mAudio != null && mAudioObject.mGo != null)
                    {
                        
                        mAudioObject.mGo.SetActive(true);
                        mAudioObject.mAudio.enabled = true;
                        mAudioObject.mAudio.Play();
                        mTotalPlayTime = 0f;
                        mSate = E_AudioPlayState.Playing;
                    }
                    break;
                case E_AudioPlayState.Pause:

                    break;
                case E_AudioPlayState.Playing:

                    mTotalPlayTime += Time.deltaTime;
                    //如果duarationTime 是999的话 ，就是无限时长，需要手动关闭
                    if (mTotalPlayTime >= duarationTime && duarationTime != 999)
                    {
                        this.Stop();
                        mSate = E_AudioPlayState.Sleeping;
                    }//控制循环时长
                    else if (null != mAudioObject && null != mAudioObject.mAudio && mAudioObject.mAudio.volume != targetVolume)
                    {
                        tempVom = targetVolume;
                    }//
                   
                    if (null != mAudioObject && null != mAudioObject.mAudio && 
                        null != mAudioObject.mAudio.clip&& mAudioObject.mAudio.volume != tempVom)
                    {
                        mAudioObject.mAudio.volume = tempVom;
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// 重新开始播放
        /// </summary>
        public void Resume()
        {
            switch (mSate)
            {
                case E_AudioPlayState.Pause:
                    mAudioObject.mAudio.UnPause();
                    mSate = E_AudioPlayState.Playing;
                    break;
                case E_AudioPlayState.Loading:
                    mPlayWhenLoaded = true;
                    break;
            }
        }
        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="rePlay"></param>
        public void Play(bool rePlay = false)
        {
            switch (mSate)
            {
                case E_AudioPlayState.Loading:
                    mPlayWhenLoaded = true;
                    break;
                case E_AudioPlayState.Sleeping:
                case E_AudioPlayState.Ready:
                    
                    break;
                case E_AudioPlayState.Playing:
                    if (rePlay)
                    {
                        Stop();
                        Play();
                    }
                    break;
                case E_AudioPlayState.Pause:
                    if (rePlay)
                    {
                        Stop();
                        Play();
                    }
                    else
                    {
                        mAudioObject.mAudio.UnPause();
                        mSate = E_AudioPlayState.Playing;
                    }
                    break;
            }
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            switch (mSate)
            {
                case E_AudioPlayState.Loading:
                    mPlayWhenLoaded = false;
                    break;
                case E_AudioPlayState.Ready:
                    break;
                case E_AudioPlayState.Pause:
                case E_AudioPlayState.Playing:
                    mTotalPlayTime = 0;
                    if (mAudioObject.mAudio!= null)
                    {
                        mAudioObject.mAudio.Stop();
                        mAudioObject.mAudio.enabled = false;
                    }
                    mSate = E_AudioPlayState.Sleeping;
                    break;
            }
        }

        public void Pause()
        {
            switch (mSate)
            {
                case E_AudioPlayState.Loading:
                    mPlayWhenLoaded = false;
                    break;
                case E_AudioPlayState.Playing:
                    mAudioObject.mAudio.Pause();
                    mSate = E_AudioPlayState.Pause;
                    break;
            }
        }

        /// <summary>
        ///清理数据，回收对象 
        /// </summary>
        public override void DoClean()
        {
            base.DoClean();

            ReleaseAudioObject();
            
            mTotalPlayTime = 0;
            curAudioBigType = E_MAudioBigType.BGM;
             GameCache.Instance.audioPool.Put(this);//对象池对象回收
            mSate = E_AudioPlayState.Sleeping;
            tfPos = Vector3.zero;
        }

        /// <summary>
        /// 回收audioObject ，主要用于战斗内最大衰减外的音效回收
        /// </summary>
        public  void ReleaseAudioObject() {
            if (null != mAudioObject)
            {
                mAudioObject.Release();
            }
        } 
        public override void DoDestroy()
        {
            base.DoDestroy();
            GameObject.Destroy(mAudioObject.mGo);
        }

        /// <summary>
        /// 音效类型
        /// </summary>
        public int CurAudioType {
            get {
                return audioType;
            }
            
        }
        /// <summary>
        /// 记录当前音效所处的大类型，只设置一次，优化效率
        /// </summary>
        private int curAudioBigType;
        public int CurAudioBigType {
            set {

                curAudioBigType = value;
            }
            get
            {
                return curAudioBigType;
            }
        }

        public string GetEffectPrefabPathByEffectType(MAudioIdType type)
        {
            return GetEnumDes(type);
        }
        public string GetEnumDes(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }
    }
}
