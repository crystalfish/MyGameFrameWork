using UnityEngine;
using System.Collections;

/// <summary>
/// Author:joi
/// Desc:设置Canvas下3d物体的屏幕自适应
/// </summary>

namespace COMMON
{
    public class UI3DObjectAutoScale : MonoBehaviour
    {
        /// <summary>
        /// 用代码动态加入场景中后
        /// 在start中localScale始终是1，所有这里先手动填写
        /// </summary>
        public Vector2 Scale;
        // Use this for initialization
        void Start()
        {
            float flag = UICanvasManager.Instance.AdjustorAspect;
            if (flag > 1.0)
            {
                float widthScale = Scale.x / Scale.y;
                widthScale *= flag;
                widthScale *= Scale.y;
                transform.localScale = new Vector3(widthScale, Scale.y);
            }
            else
            {
                float hightScale = Scale.x / Scale.y;
                hightScale /= flag;
                hightScale *= Scale.y;
                transform.localScale = new Vector3(Scale.x, hightScale);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
