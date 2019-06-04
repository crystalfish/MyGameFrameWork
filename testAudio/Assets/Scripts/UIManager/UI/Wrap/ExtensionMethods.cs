using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace COMMON
{
    
    public static class ExtensionMethods
    {
        private static E_LAN mLan = E_LAN.EN;
        /// <summary>
        /// 多语言种类
        /// </summary>
        public enum E_LAN
        {
            /// <summary>
            /// 中文
            /// </summary>
            CNH = 0,
            /// <summary>
            /// 英文
            /// </summary>
            EN = 1
        }

       
        public static void ChangeLanguage(E_LAN lan)
        {
          
        }

        public static void SetText(this Text note, string content)
        {
            if (note != null && note.gameObject != null)
            {
                note.text = content;
            }
        }

        public static void SetText(this Text note, int content)
        {
            if (note != null && note.gameObject != null)
            {
                note.text = content.ToString();
            }
        }

       
       
        
        public static int GetTxtLines(this Text txt)
        {
            int line = 1;
            string content = txt.text;
            txt.text = "A";
            float height = txt.preferredHeight;
            txt.text = content;
            float totalHeight = txt.preferredHeight;
            line = (int)(totalHeight / height);
            return line;
        }

       

     

       



        public static void SetText(this InputField note, string content)
        {
            if (note != null && note.gameObject != null)
            {
                note.text = content;
            }
        }


        /// <summary>
        /// 设置控件大小的扩展方法
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        public static void SetLocalSizeDeltaX(this Transform t, float value)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.sizeDelta = new Vector3(value, rectTransfrom.sizeDelta.y);
            }
        }
        public static void SetLocalSizeDeltaY(this Transform t, float value)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.sizeDelta = new Vector3(rectTransfrom.sizeDelta.x, value);
            }
        }
        public static void SetLocalSizeDelta(this Transform t, Vector2 value)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.sizeDelta = value;
            }
        }
        /// <summary>
        /// 设置本地坐标的扩展方法
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        public static void SetLocalPositionX(this Transform t, float value)
        {
            if (t != null)
            {
                t.localPosition = new Vector3(value, t.localPosition.y, t.localPosition.z);
            }
        }

        public static void SetAnchoridPositionX(this Transform t, float value)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.anchoredPosition = new Vector3(value, rectTransfrom.anchoredPosition.y);
            }
        }

        public static void SetAnchoridPositionY(this Transform t, float value)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.anchoredPosition = new Vector3(rectTransfrom.anchoredPosition.x, value);
            }
        }

        public static void SetAnchoridPosition(this Transform t, Vector3 pos)
        {
            if (t != null)
            {
                RectTransform rectTransfrom = t.GetComponent<RectTransform>();
                rectTransfrom.anchoredPosition = pos;
            }
        }

        public static Vector3 GetAnchoridPosition(this Transform t)
        {
            RectTransform rectTransfrom = t.GetComponent<RectTransform>();

            return rectTransfrom.anchoredPosition;
        }

        public static void SetLocalPositionY(this Transform t, float value)
        {
            if (t != null)
            {
                t.localPosition = new Vector3(t.localPosition.x, value, t.localPosition.z);
            }
        }

        public static void SetLocalPositionZ(this Transform t, float value)
        {
            if (t != null)
            {
                t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, value);
            }
        }

        /// <summary>
        /// 设置RectTransform坐标X
        /// </summary>
        public static void SetAnchoredPosX(this RectTransform t, float value)
        {
            if (t != null)
            {
                t.anchoredPosition = new Vector2(value, t.anchoredPosition.y);
            }
        }

        /// <summary>
        /// 设置RectTransform坐标Y
        /// </summary>
        public static void SetAnchoredPosY(this RectTransform t, float value)
        {
            if (t != null)
            {
                t.anchoredPosition = new Vector2(t.anchoredPosition.x, value);
            }
        }

        public static void SetAnchoredPos(this RectTransform t, Vector2 pos)
        {
            if (t != null)
            {
                t.anchoredPosition = pos;
            }
        }

       

        static Vector3 TransformPosGetter(this Transform t)
        {
            return t.position;
        }

        static void TransformPosSetter(this Transform t, Vector3 targetValue)
        {
            t.position = targetValue;
        }


        static Vector3 GetCurAnchoridPosGetter(this RectTransform rect)
        {
            return rect.anchoredPosition;
        }


        static void SetAnchoridPos(this RectTransform t, Vector3 endValue)
        {
            if (t != null)
            {
                t.anchoredPosition = endValue;
            }
        }


        private static float GetCurAnchoridPosXGetter(this RectTransform rectTrans)
        {
            return rectTrans.anchoredPosition.x;
        }

        private static void SetCurAnchoridPosX(this RectTransform rectTrans, float endValue)
        {
            if (rectTrans != null)
            {
                rectTrans.anchoredPosition = new Vector3(endValue, rectTrans.anchoredPosition.y);
            }
        }

        private static float GetCurAnchoridPosYGetter(this RectTransform rectTrans)
        {
            return rectTrans.anchoredPosition.y;
        }

        private static void SetCurAnchoridPosY(this RectTransform rectTrans, float endValue)
        {
            if (rectTrans != null)
            {
                rectTrans.anchoredPosition = new Vector3(rectTrans.anchoredPosition.x, endValue);
            }
        }

        public static void SetActive2(this GameObject uiObj, bool value)
        {
            if (null != uiObj)
            {
                if (uiObj.activeSelf != value)
                {
                    uiObj.SetActive(value);
                }
            }
        }

        public static T AddOrGetComponent<T>(this GameObject go) where T : Component
        {
            if (null != go)
            {
                T component = go.GetComponent<T>();
                if (component == null)
                {
                    component = go.AddComponent(typeof(T)) as T;
                }
                return component;
            }

            return null;
        }

        public static void SetFillAmount(this Image img, float value)
        {
            if (img != null)
            {
                var amount = img.fillAmount;
                if (amount != value)
                {
                    img.fillAmount = value;
                }
            }
        }

        public static GameObject FindChildObj(this GameObject uiObj, string name)
        {
            if (null == uiObj)
            {
                return null;
            }

            Transform tf = uiObj.transform.Find(name);
            if (tf == null)
            {
                return null;
            }

            return tf.gameObject;
        }


        /// <summary>
        /// 直接使用16进制颜色赋值给text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="HexColor"></param>
        public static void SetColor(this Text text, UInt32 HexColor)
        {
            byte R, G, B, A;   //8位RGB值
            R = (byte)(HexColor >> 24);             //取出高位R的分量
            G = (byte)((HexColor >> 16) & 0x00FF);  //取出高位G的分量
            B = (byte)(HexColor >> 8 & 0x0000FF);             //取出高位B的分量
            A = (byte)(HexColor & 0x000000FF);
            text.color = new Color(R / 255f, G / 255f, B / 255f, A);
        }

        /// <summary>
        /// 设置UI的颜色
        /// </summary>
        /// <param name="grphic"></param>
        /// <param name="HexColor"></param>
        public static void SetColor(this Graphic grphic, UInt32 HexColor)
        {
            byte R, G, B, A;   //8位RGB值
            R = (byte)(HexColor >> 24);             //取出高位R的分量
            G = (byte)((HexColor >> 16) & 0x00FF);  //取出高位G的分量
            B = (byte)(HexColor >> 8 & 0x0000FF);             //取出高位B的分量
            A = (byte)(HexColor & 0x000000FF);
            grphic.color = new Color(R / 255f, G / 255f, B / 255f, A / 255f);
        }

        public static void PrintLog(string msg)
        {
            Logger.Log(msg);
        }

        public static void PrintError(string msg)
        {
            Logger.LogError(msg);
        }

        

        private static float AlphaGetter(this CanvasGroup canvasGrounp)
        {
            if (canvasGrounp == null)
            {
                return 1;
            }

            return canvasGrounp.alpha;
        }

        private static void AlphaSetter(this CanvasGroup canvasGrounp, float alpha)
        {
            if (canvasGrounp != null)
            {
                canvasGrounp.alpha = alpha;
            }
        }


        private static float AlphaGetter(this Image image)
        {
            return image.color.a;
        }

        private static void AlphaSetter(this Image image, float alpha)
        {
            Color oldColor = image.color;
            oldColor.a = alpha;
            image.color = oldColor;
        }
        


        #region 查找子物体上的UI组件
        public static T GetChildUIScript<T>(this GameObject uiObj, string name)
        {
            if (uiObj != null)
            {
                Transform tra = string.IsNullOrEmpty(name) ? uiObj.transform : uiObj.transform.Find(name);
                if (tra != null)
                {
                    return tra.gameObject.GetComponent<T>();
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取子物体上的组件，排除物体本身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="includInactive"></param>
        /// <returns></returns>
        public static T[] GetComponentsInReelChildren<T>(this GameObject go, bool includInactive = false)
        {
            if (null == go)
            {
                return null;
            }
            List<T> TList = new List<T>();
            TList.AddRange(go.GetComponentsInChildren<T>(includInactive));
            TList.RemoveAt(0);
            return TList.ToArray();
        }
        #endregion

        public static Vector2 ToVec2XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static void SetLayer(this GameObject go, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);

            ExtensionMethods.SetLayerRecursively(go, layer);
        }

        public static void SetLayer(this GameObject go, int layer)
        {
            ExtensionMethods.SetLayerRecursively(go, layer);
        }

        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                ExtensionMethods.SetLayerRecursively(go.transform.GetChild(i).gameObject, layer);
            }
        }

        /// <summary>
        /// 设置特效层级
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layerName"></param>
        public static void SetEffectLayer(this GameObject go, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);

            ExtensionMethods.SetEffectLayerRecursively(go, layer);
        }
        /// <summary>
        /// 设置特效层级
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetEffectLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject childGo = go.transform.GetChild(i).gameObject;

                ExtensionMethods.SetEffectLayerRecursively(childGo, layer);
            }
        }

        public static void SetLayerSelf(this GameObject go, string effect)
        {
            int layer = LayerMask.NameToLayer(effect);
            go.layer = layer;
        }

        public static void SetAlpha(this Image img, float a)
        {
            if (null != img)
            {
                Color col = img.color;
                col.a = a;
                img.color = col;
            }
        }
       

    }//end class
}//end namespace