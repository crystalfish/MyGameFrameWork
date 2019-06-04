using UnityEngine;
using System.Collections;

/// <summary>
/// Author:joi
/// Desc:修改UI上特效order，结合UI面板上Canvas实现Ui特效与
/// ui面板的层级控制
/// </summary>
namespace COMMON
{
    public class UIEffectDepth : MonoBehaviour
    {
        //特效挂载的面板root,特效层级由该canvas的order决定
        public Canvas UIpanelCanvas;

        void Start()
        {
            SetSortingOrder();
        }

        Canvas mParentCanvas;
        void OnEnable()
        {
            SetSortingOrder();
        }


        void SetSortingOrder()
        {
            if (null != UIpanelCanvas)
            {
                mParentCanvas = UIpanelCanvas;
            }
            else
            {
                mParentCanvas = GetComponentInParent<Canvas>();
            }

            if (null != mParentCanvas)
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>(true);
                int count = renders.Length;
                for (int i = 0; i < count; i++)
                {
                    Renderer render = renders[i];
                    render.sortingOrder = mParentCanvas.sortingOrder +1;
                    render.sortingLayerID = mParentCanvas.sortingLayerID;
                }
            }
        }

        void OnDestroy()
        {
            UIpanelCanvas = UIpanelCanvas.TryDispose();
        }
    }
}
