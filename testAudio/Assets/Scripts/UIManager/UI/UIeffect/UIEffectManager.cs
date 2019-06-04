using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;
using System.Reflection;
namespace COMMON
{
    public enum eUIEffectType
    {
        None = -1,
        [Description("Effect/Victory/Prefab/ui_Victory_chixu")]
        BulletEmpty = 0,

        
    }
    public class UIEffectManager : SimpleSingleton<UIEffectManager>
    {
        //存储每个物体及对应特效
        XDictionary<Transform, XDictionary<int, Transform>> mTran2DicEffect = new XDictionary<Transform, XDictionary<int, Transform>>();

        public void RemoveEffect(Transform trans)
        {
            if (null == trans)
            {
                return;
            }
            ParticleSystem[] particles = trans.GetComponentsInChildren<ParticleSystem>();
            int count = particles.Length;
            for (int i = 0; i < count; ++i)
            {
                particles[i].Stop(true);
                particles[i].Clear(true);
            }
        }


        /// <summary>
        /// 添加特效
        /// </summary>
        /// <param name="type"> eUIEffectType类别，特效路径</param>
        /// <param name="parent">需要添加特效的物体</param>
        /// <param name="localOffset">特效位置偏移量</param>
        /// <param name="localScaleTr">特效缩放比例</param>
        /// <param name="localRotaion">特效旋转</param>
        /// <param name="strPath">lua中不支持eUIEffectType，传入的特效预设路径</param>
        /// <param name="overLayer">是否在parent上层</param>
        /// <param name="flushPos">是否需要刷新坐标</param> 
        /// <param name="setSortLayer">是否需要将sortingLayer设置为父物体一样</param>
        public Transform CreateEffect(eUIEffectType type, Transform parent = null,
            Vector3 localOffset = default(Vector3),
           Vector3 localScaleTr = default(Vector3),
           Vector3 localRotaion = default(Vector3),
           string strPath = "", bool overLayer = true,
           bool flushPos = false,
           bool setSortLayer = true)
        {
            if (null == parent)
            {
                return null;
            }
            var intType = (int)type;
            XDictionary<int, Transform> tf = null;
            if (mTran2DicEffect.TryGetValue(parent, out tf))
            {
                //如果已经添加过，则直接播放特效
                Transform effectObj;
                if (tf.TryGetValue(intType, out effectObj))
                {
                    PlayEffect(effectObj.gameObject, parent, overLayer, setSortLayer);
                    if (flushPos)
                    {
                        SetPosition(localOffset, parent, effectObj);
                    }
                    return effectObj;
                }
            }

            strPath = strPath.IsNullOrEmpty() ? GetEffectPrefabPathByEffectType(type) : strPath;
            Transform ts = Resources.Load<Transform>(strPath) as Transform;
            Transform tsImpl = GameObject.Instantiate<Transform>(ts);
            if (tsImpl != null)
            {
                tsImpl.parent = parent;
                if (parent != null)
                {
                    SetLayer(tsImpl.gameObject, parent.gameObject.layer);

                }

                Quaternion qua = Quaternion.identity;
                if (localRotaion != default(Vector3))
                {
                    qua = Quaternion.Euler(localRotaion);
                }
                tsImpl.localRotation = qua;
                tsImpl.localScale = localScaleTr == default(Vector3) ? Vector3.one : localScaleTr;

                if (mTran2DicEffect.ContainsKey(parent) && null != tf)
                {
                    tf.Add(intType, tsImpl);
                }
                else
                {
                    XDictionary<int, Transform> pairkey = new XDictionary<int, Transform>();
                    pairkey.Add(intType, tsImpl);
                    mTran2DicEffect.Add(parent, pairkey);
                }
                SetPosition(localOffset, parent, tsImpl);
                PlayEffect(tsImpl.gameObject, parent, overLayer, setSortLayer);
                return tsImpl;
            }
            return null;
        }

        /// <summary>
        /// 设置坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        void SetPosition(Vector3 pos, Transform parent, Transform obj)
        {
            Vector3 localOffset = pos == default(Vector3) ? Vector3.zero : pos;
            Vector3 creatPos = parent == null ? localOffset : parent.TransformPoint(localOffset);
            obj.transform.position = creatPos;
        }


        void SetEffectSortingOrder(Transform trans, Transform parent, bool overLayer = true,
            bool setSortLayer = true)
        {
            if (null == trans || null == parent)
            {
                return;
            }

            Renderer[] renders = trans.GetComponentsInChildren<Renderer>();

            //如果特效需要放在父物体的下层，则需要在父物体上添加Renderer并设置比特效高
            if (!overLayer)
            {
                Canvas renderComs = parent.gameObject.AddMissingComponent<Canvas>();
                renderComs.overrideSorting = true;
                Canvas canvas = FindFirstParentCanvans(parent);
                if (null != canvas)
                {
                    renderComs.sortingLayerID = canvas.sortingLayerID;
                    renderComs.sortingOrder = canvas.sortingOrder + 5;
                    SetEffectSenderOrder(trans, renderComs, true, setSortLayer);
                }
            }
            else
            {
                Canvas canv = trans.GetComponentInParent<Canvas>();
                SetEffectSenderOrder(trans, canv, setSortLayer: setSortLayer);
            }
        }


        void SetEffectSenderOrder(Transform effect, Canvas parentCanvas, bool overLayer = false,
            bool setSortLayer = true)
        {
            if (!setSortLayer)
            {
                return;
            }
            Renderer[] renders = effect.GetComponentsInChildren<Renderer>();
            int count = renders.Length;
            for (int i = 0; i < count; i++)
            {
                Renderer render = renders[i];
                render.sortingLayerID = parentCanvas.sortingLayerID;
                int offeset = overLayer ? -1 : 1;
                render.sortingOrder = parentCanvas.sortingOrder + offeset;
            }
        }


        /// <summary>
        /// 激活特效物体
        /// </summary>
        /// <param name="effectObj"></param>
        void PlayEffect(GameObject effectObj, Transform parent, bool overLayer = true,
            bool setSortLayer = true)
        {
            if (null == effectObj)
            {
                return;
            }
            if (effectObj.gameObject.activeSelf == false)
            {
                effectObj.gameObject.SetActive(true);
            }
            SetEffectSortingOrder(effectObj.transform, parent, overLayer, setSortLayer);
            ParticleSystem[] particles = effectObj.GetComponentsInChildren<ParticleSystem>();
            int count = particles.Length;
            for (int i = 0; i < count; ++i)
            {
                particles[i].Stop(true);
                particles[i].Clear(true);
                particles[i].Play();
            }
        }


        public void StopEffect(Transform obj, eUIEffectType type)
        {
            if (null == obj)
            {
                return;
            }
            XDictionary<int, Transform> tf = null;
            if (mTran2DicEffect.TryGetValue(obj, out tf))
            {
                Transform effect;
                if (tf.TryGetValue((int)type, out effect))
                {
                    ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
                    int count = particles.Length;
                    for (int i = 0; i < count; ++i)
                    {
                        particles[i].Stop(true);
                        particles[i].Clear(true);
                    }
                    effect.gameObject.SetActive2(false);
                }
            }
        }


        public void StopEffect(Transform obj)
        {
            if (null == obj)
            {
                return;
            }

            XDictionary<int, Transform> tf = null;
            if (mTran2DicEffect.TryGetValue(obj, out tf))
            {
                List<KeyValuePair<int, Transform>> tl = tf.GetList();
                for (int k = 0; k < tl.Count; ++k)
                {
                    Transform effect = tl[k].Value;
                    if (null == effect)
                    {
                        continue;
                    }
                    ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
                    int count = particles.Length;
                    for (int i = 0; i < count; ++i)
                    {
                        particles[i].Stop(true);
                        particles[i].Clear(true);
                    }
                    effect.gameObject.SetActive2(false);
                }
            }
        }


        public void PlayEffect(Transform trans, Transform parent, bool overLayer = true)
        {
            if (null == trans)
            {
                return;
            }
            if (trans.gameObject.activeSelf == false)
            {
                trans.gameObject.SetActive(true);
            }
            SetEffectSortingOrder(trans, parent, overLayer);
            ParticleSystem[] particles = trans.GetComponentsInChildren<ParticleSystem>();
            int count = particles.Length;
            for (int i = 0; i < count; ++i)
            {
                particles[i].Stop(true);
                particles[i].Clear(true);
                particles[i].Play();
            }
        }

        /// <summary>
        /// 销毁UI特效
        /// </summary>
        /// <param name="effectType"></param>
        public void RemoveEffect(Transform obj, eUIEffectType effectType)
        {
            if (null == obj)
            {
                return;
            }
            XDictionary<int, Transform> tf;
            if (mTran2DicEffect.TryGetValue(obj, out tf))
            {
                Transform effect;
                int intType = (int)effectType;
                if (tf.TryGetValue(intType, out effect))
                {
                    RemoveEffect(effect);
                    effect.parent = null;
                   // Destroy.DestroyImmediate(effect.gameObject, true);
                    tf.Remove(intType);
                }
            }
        }

        public void Dispose()
        {
            //Transform tf;
            //if (mEffect2Tran.TryGetValue(effectType, out tf))
            //{
            //    RemoveEffect(tf);
            //    tf.parent = null;
            //    Destroy.DestroyImmediate(tf.gameObject, true);
            //    mEffect2Tran.Remove(effectType);
            //}
            var L = mTran2DicEffect.GetList();
            for (int i = 0, m = L.Count; i < m; i++)
            {
                var item = L[i];
                var value = item.Value;
                if (null == value)
                {
                    continue;
                }
                var effectList = item.Value.GetList();
                for (int k = 0, n = effectList.Count; k < n; k++)
                {
                    var effectItem = effectList[k];
                    var effectObj = effectItem.Value;
                    if (null != effectObj)
                    {
                        RemoveEffect(effectObj);
                        effectObj.parent = null;
                         //Destroy.DestroyImmediate(effectObj.gameObject, true);
                    }
                }
            }
            mTran2DicEffect.Clear();
        }
        public string GetEffectPrefabPathByEffectType(eUIEffectType type)
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
        public void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;

            Transform t = go.transform;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        public Canvas FindFirstParentCanvans(Transform tf)
        {
            Canvas[] tCv;
            if (tf != null && null != tf.parent)
            {
                tCv = tf.parent.GetComponentsInParent<Canvas>();
                if (tCv != null)
                    return tCv[0];
                return null;
            }
            else
            {
                return null;
            }

        }

    }

}
