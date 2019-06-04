using UnityEngine;

namespace COMMON
{
    public class Uimanager : MonoBehaviour
    {
        public static Uimanager Ins { get; private set; }

        //显示飘字的canvas
        public Transform NumberTextParent;

        

        void Awake()
        {
            Ins = this;
        }

        

        public void DestoryLifeBar()
        {
           

        }

        public void DestoryBattleNumItem()
        {
            if (null == NumberTextParent)
            {
                return;
            }
            GameObject go = null;
            for (int i = 0; i < NumberTextParent.childCount; i++)
            {
                go = NumberTextParent.GetChild(i).gameObject;
                if (go != null)
                    Destroy(go);
            }

        }

        /// <summary>
        /// 交换两个面板层级，换完后 upobj在上，downobj在下  需要面板上有CANVAS
        /// </summary>
        /// <param name="upObj"></param>
        /// <param name="downObj"></param>
        public void ExChangePanelIndex(Transform upObj, Transform downObj)
        {
            Canvas upCanvas = upObj.GetComponent<Canvas>();
            Canvas downCanvas = downObj.GetComponent<Canvas>();
            int Index = upCanvas.sortingOrder ;
            int Index1 = downCanvas.sortingOrder;
            int UpIndex = Index > Index1 ? Index : Index1;
            int DownIndex = Index > Index1 ? Index1 : Index;
            upCanvas.sortingOrder = UpIndex;
            downCanvas.sortingOrder = DownIndex;
            //upObj.SetSiblingIndex(UpIndex);
            //downObj.SetSiblingIndex(DownIndex);

        }
    }
}