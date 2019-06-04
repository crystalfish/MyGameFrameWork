using UnityEngine;
using System.Collections;
/// <summary>
/// 自适应背景大小
/// </summary>
namespace COMMON
{
    public class UIBgAutoScale : MonoBehaviour
    {

        RectTransform bgRT = null;
        // Use this for initialization
        void Awake()
        {
            bgRT = GetComponent<RectTransform>();
            if (bgRT != null)
            {
                //我们的背景图是以2：1的设备适配的，所以大于2：1的设备需要调整
                float flag = UICanvasManager.Instance.AdjustorAspectBg;
                
                if (flag > 1.0f)
                {

                    bgRT.sizeDelta *= flag;
                }
                //小于1024*768也会有问题
                if(flag<0.66f)
                {
                    bgRT.sizeDelta *= 0.66f / flag;
                }
            }
        }

    }
}
