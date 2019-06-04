using UnityEngine;
using System.Collections;

/// <summary>
/// Author:joi
/// Desc:根据物体在Hierarchy中的层次，在父物体的Canvas的
/// layer基础上递增sortingorder,设置3d物体和UI的层级按siblingindex排序
/// </summary>
namespace COMMON
{
    public class UIDepth : MonoBehaviour
    {
        //父物体面板root,
        public Canvas UIpanelCanvas;
        int siblingIndex = 0;

     
        void OnEnable()
        {
            siblingIndex = transform.GetSiblingIndex();
            if (null != UIpanelCanvas)
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();
                int count = renders.Length;
                for (int i = 0; i < count; i++)
                {
                    Renderer render = renders[i];
                    render.sortingLayerID = UIpanelCanvas.sortingLayerID;
                    render.sortingOrder = UIpanelCanvas.sortingOrder + siblingIndex;
                }
            }
        }

        void OnDestroy()
        {
            UIpanelCanvas = UIpanelCanvas.TryDispose();
        }
    }
}
