using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// author:joi
/// 自适应canvas,当大于默认分辨率时自适应高度，当小于等于默认1.7777的比例时，以宽度自适应
/// </summary>
namespace COMMON
{
    public class UIAutoScale : MonoBehaviour
    {

        void Awake()
        {
          
            CanvasScaler canvasScalerTemp = transform.GetComponent<CanvasScaler>();
            if (canvasScalerTemp != null)
            {
                float flag = UICanvasManager.Instance.AdjustorAspect;
                if (flag > 1.0)
                {

                    canvasScalerTemp.matchWidthOrHeight = 1;
                }
                else
                {

                    canvasScalerTemp.matchWidthOrHeight = 0;
                }

            }
        }
    }//end Class
}//end namespace
