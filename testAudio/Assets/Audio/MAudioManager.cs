//joi
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace COMMON
{
    /// <summary>
    /// 音频统一管理接口
    /// 使用说明：
    /// 播放 play接口 ，停止stop接口，清楚OnDestroy接口
    /// </summary>

    public class MAudioManager : MLogicManager
    {

        public bool isLoading = false;
        //闲置的播放单元//
        List<MAudio> _idleAudioUnitList = new List<MAudio>();
        //保存所有正在播放的音效//
        XDictionary< MAudioIdType, List<MAudio>> mPlayingSoundEffectDict = new XDictionary<MAudioIdType, List<MAudio>>();
        List<MAudioIdType> _playingSoundEffectConfigList = new List<MAudioIdType>();
        public XDictionary<int, bool> mAudio2IsEnable = new XDictionary<int, bool>();
        /// <summary>
        /// 用于排它类型，比如背景音乐，比如加载背景音效
        /// </summary>
        MAudio _single_music;
        int maxOverlay = 50;

        private XDictionary<int, List<int>> mAudioId2IndexArray = new XDictionary<int, List<int>>();

        public override void Initialize()
        {
            base.Initialize();
            //TODO: 先注释掉 放进 主逻辑更新循环里
            //GlobalLogic.gTimer.AddUpdateHandler(OnUpdate);
        }

        #region

        /// <summary>

        /// <summary>
        /// 播放 ，音频id 和父对象tf ,对应audioresconfig 表格的id
        /// </summary>
        /// <param name="configID"></param>
        /// <param name="tf"></param>
        public MAudio Play(MAudioIdType configID, Transform tf   ,float durationTime , int audioType ,
            bool loop , int loopCount )
        {
            MAudioIdType configId = configID;
            
            List<MAudio> mPlayingList;
            if (mPlayingSoundEffectDict.TryGetValue(configId, out mPlayingList))
            {
                if (mPlayingList == null)
                {
                    mPlayingList = new List<MAudio>();
                    mPlayingSoundEffectDict[configId] = mPlayingList;
                }
            }
            else
            {
                mPlayingList = new List<MAudio>();
                mPlayingSoundEffectDict.Add(configId, mPlayingList);
                _playingSoundEffectConfigList.Add(configId);
            }
            var mAudioType = audioType;
            if (mAudioType == E_MAudioType.LOADING_AUDIO || mAudioType == E_MAudioType.BGM)//TODO:改为音效类型
            {
                // 处于loading时期 ，禁止播放其他类型音效，避免野指针
               
                if (_single_music == null)
                    _single_music = MAudio.Get();
                _single_music.AttachAudioObject(configId, tf ,durationTime ,audioType , loop ,loopCount);
                return _single_music;
                
            }
            else
            {

                // 处于loading时期 ，禁止播放其他类型音效，避免野指针
                //音效//
                if (mPlayingList.Count < maxOverlay)
                {
                    MAudio audio = null;
                    audio = MAudio.Get();
                    if(audio.state != E_AudioPlayState.Sleeping)
                    {
                        audio = null;
                        audio = MAudio.Get();
                    }
                    mPlayingSoundEffectDict[configId].Add(audio);
                    audio.AttachAudioObject(configId, tf, durationTime, audioType, loop, loopCount);
                    return audio;
                }
                else
                {
                    for (int i = 0, count = mPlayingList.Count; i < count; i++)
                    {
                        var A = mPlayingList[i];
                        if (A.state == E_AudioPlayState.Pause)
                        {
                            A.Resume();
                            return A;

                        }
                        else if (A.state == E_AudioPlayState.Sleeping)
                        {
                            A.Play(true);
                            return A;
                        }
                    }

                    return null;
                }
            }
        }
        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="configID"></param>
        /// <param name="audioInfo"></param>
        public void Pause(MAudioIdType configID, MAudio audioInfo = null)
        {
            if (_single_music != null &&  _single_music.audioId == configID)
            {
                _single_music.Pause();
                return;
            }
            var L = mPlayingSoundEffectDict.GetList();
            for(int i = 0 ,count = L.Count; i < count; i++)
            {
                var kv = L[i];
                if(kv.Key == configID)
                {
                    var ValueList = kv.Value;
                    if (audioInfo != null)
                    {
                        for(int j = 0 ,VCount = ValueList.Count; j < VCount; j++ )
                        {
                            if (audioInfo == ValueList[j])
                                ValueList[j].Pause();
                        }
                        
                    }
                    else
                    {
                        for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                        {
                                ValueList[j].Pause();
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 恢复播放
        /// </summary>
        /// <param name="configID"></param>
        /// <param name="audioInfo"></param>
        public void Resume(MAudioIdType configID, MAudio audioInfo = null)
        {
            if (_single_music != null  && _single_music.audioId == configID)
            {
                _single_music.Resume();
                return;
            }
            var L = mPlayingSoundEffectDict.GetList();
            for (int i = 0, count = L.Count; i < count; i++)
            {
                var kv = L[i];
                if (kv.Key == configID)
                {
                    var ValueList = kv.Value;
                    if (audioInfo != null)
                    {
                        for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                        {
                            if (audioInfo == ValueList[j])
                                ValueList[j].Resume();
                        }

                    }
                    else
                    {
                        for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                        {
                            ValueList[j].Resume();
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="configID"></param>
        /// <param name="audioInfo"></param>
        public void Stop(MAudioIdType configID,MAudio audioInfo = null)
        {
            if (_single_music != null &&  _single_music.audioId == configID)
            {
                _single_music.Stop();
                return;
            }

            var L = mPlayingSoundEffectDict.GetList();
            for (int i = 0, count = L.Count; i < count; i++)
            {
                var kv = L[i];
                if (kv.Key == configID)
                {
                    var ValueList = kv.Value;
                    if (audioInfo != null)
                    {
                        for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                        {
                            if (audioInfo == ValueList[j])
                                ValueList[j].Stop();
                        }

                    }
                    else
                    {
                        for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                        {
                            ValueList[j].Stop();
                        }
                    }
                }
            }


        }
        #endregion
        void CleanOneAudio(MAudioIdType config)
        {
            List<MAudio> audioList = null;
            if (mPlayingSoundEffectDict.TryGetValue(config, out audioList))
            {
                if (audioList != null)
                {
                    for(int  i = 0 ,count = audioList.Count; i < count; i++)
                    {
                        audioList[i].DoClean();
                        _idleAudioUnitList.Add(audioList[i]);
                    }     
                }
                mPlayingSoundEffectDict.Remove(config);
                _playingSoundEffectConfigList.Remove(config);
            }
        }
        int _i = 0;
        int _leni = 0;
        int _j = 0;
        int _lenj = 0;
        MAudio _audio;
        public virtual void Clean(MAudioIdType configID)
        {
            this.Stop(configID);
            MAudioIdType config;
            
            for (_i = 0, _leni = _playingSoundEffectConfigList.Count; _i < _leni; _i++)
            {
                config = _playingSoundEffectConfigList[_i];
                if (config == configID)
                    CleanOneAudio(config);
            }
            
        }

        private bool OnUpdate(float t)
        {
            DoUpdate();
            return false;
        }
        public override void DoUpdate()
        {
            for (_i = 0, _leni = _playingSoundEffectConfigList.Count; _i < _leni; _i++)
            {
                for (_j = 0, _lenj = mPlayingSoundEffectDict[_playingSoundEffectConfigList[_i]].Count; _j < _lenj; _j++)
                {
                    _audio = mPlayingSoundEffectDict[_playingSoundEffectConfigList[_i]][_j];
                    if (_audio.state == E_AudioPlayState.Sleeping)
                    {
                        mPlayingSoundEffectDict[_playingSoundEffectConfigList[_i]].RemoveAt(_j);
                        _j--;
                        _lenj--;
                        _audio.DoClean();
                        _idleAudioUnitList.Add(_audio);
                        if (mPlayingSoundEffectDict[_playingSoundEffectConfigList[_i]].Count == 0)
                        {
                            mPlayingSoundEffectDict.Remove(_playingSoundEffectConfigList[_i]);
                            _playingSoundEffectConfigList.RemoveAt(_i);
                            _i--;
                            _leni--;
                        }
                    }
                    else
                    {
                        _audio.DoUpdate();
                    }
                }
            }
            if (_single_music != null)
                _single_music.DoUpdate();
        }

        /// <summary>
        /// 停止该类型的音效
        /// </summary>
        /// <param name="type"></param>
        public void StopVoidceByType(int type)
        {
            
            if(type == E_MAudioType.LOADING_AUDIO || type == E_MAudioType.BGM)
            {
                if(_single_music != null)
                    _single_music.Stop();
            }
            else
            {
                var L = mPlayingSoundEffectDict.GetList();
                for (int i = 0, count = L.Count; i < count; i++)
                {
                    var kv = L[i];
                    var ValueList = kv.Value;
                    
                    for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                    {
                        var A = ValueList[j];
                        
                        if (A != null && A.CurAudioType == type)
                        {
                            A.Stop();
                        }//if curAudio
                    }

                    
                    
                }
               
            }//else GetAudioBigType

        }//end fun

        /// <summary>
        /// 离开玩法场景的时候 ，统一清除一遍
        /// </summary>
        public override void DoDestroy()
        {
            DestroyEffect();
            if (_single_music != null)
                _single_music.DoDestroy();
            _single_music = null;
           
            GameCache.Instance.audioPool.Clear();
            GameCache.Instance.AudioObjectPool.Clear();
            base.DoDestroy();
        }

        public void DestroyEffect() {
            for (int i = 0, count = _idleAudioUnitList.Count; i < count; i++)
            {
                if (_idleAudioUnitList[i] != null)
                {
                    _idleAudioUnitList[i].DoDestroy();
                }
            }

            _idleAudioUnitList.Clear();
            var L = mPlayingSoundEffectDict.GetList();
            for (int i = 0, count = L.Count; i < count; i++)
            {
                var kv = L[i];
                var ValueList = kv.Value;

                for (int j = 0, VCount = ValueList.Count; j < VCount; j++)
                {
                    var A = ValueList[j];

                    if (A != null)
                    {
                        A.DoDestroy();
                    }//if curAudio
                }
            }
            mPlayingSoundEffectDict.Clear();
            _playingSoundEffectConfigList.Clear();
        }

        //public void Enba

       
        /// <summary>
        /// 创建50个缓存音频 ，避免内存碎片化
        /// </summary>
        /// <param name="count"></param>
        public void MakeReserve(int count)
        {
            var tmp = new MAudioObject[count];
            for (int i = 0; i < count; ++i)
            {
                tmp[i] = MAudioObject.Get(Vector3.zero);
            }//for
            for (int i = 0; i < count; ++i)
            {
                tmp[i].Release();
            }//for
        }//MakeReserve

        /// <summary>
        /// 用于重置声音类型筛选，或者关闭大类型声音
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetAudioBigType(int type) {
            switch(type)
            {
                case E_MAudioType.BGM:
                case E_MAudioType.LOADING_AUDIO:
                case E_MAudioType.UIHUMAN:
                case E_MAudioType.VIEW_AUDIO:
                case E_MAudioType.BATTLE_VIEW:
                    return E_MAudioBigType.BGM;
                    
                default:
                    return E_MAudioBigType.EFFECT;
                   
            }
        }

        #region 音效全局音量区域
        private float mEffectVolume = 1f;
        /// <summary>
        /// 声效音量(0 - 1)
        /// </summary>
        public float EffectVolume
        {
            get
            {
                return mEffectVolume;
            }
            set
            {
                if (mEffectVolume != value)
                {
                    mEffectVolume = Mathf.Clamp01(value);
                    
                }
            }
        }//end EffectVolume

        /// <summary>
        /// BGM音量(0 - 1)
        /// </summary>
        private float mBGMVolume = 1f;
        public float BGMVolume
        {
            get
            {
                return mBGMVolume;
            }
            set
            {
                if (mBGMVolume != value)
                {
                    mBGMVolume = Mathf.Clamp01(value);
                   
                }
            }
        }//end BGMVolume

     
        bool mEffectEnabled = true;
        /// <summary>
        /// 是否允许播放特效声音
        /// </summary>
        public bool EffectEnabled
        {
            get
            {
                return mEffectEnabled;
            }
            set
            {
                if (mEffectEnabled != value)
                {
                    mEffectEnabled = value;
                    if (!value)
                    {
                        mEffectVolume = 0;
                    }

                }
            }
        }//end fun EffectEnabled


        bool mBGMEnabled = true;
        /// <summary>
        /// 是否允许播放背景音乐
        /// </summary>
        public bool BGMEnabled
        {
            get
            {
                return mBGMEnabled;
            }
            set
            {
                if (mBGMEnabled != value)
                {
                    mBGMEnabled = value;
                    if(!value)
                    {
                        BGMVolume = 0;
                    }
                    
                }
            }
        }//BGMEnabled

        #endregion
    }//end class MAudioManager

  
    /// <summary>
    /// 音效设置的总类型，音效 和游戏背景
    /// </summary>
    public static class E_MAudioBigType{
        /// <summary>
        /// 背景类型
        /// </summary>
        public const int BGM = 1;
        /// <summary>
        /// 音效类型
        /// </summary>
        public const int EFFECT = 2;
    }
}