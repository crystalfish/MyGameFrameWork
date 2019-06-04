using UnityEngine;
using System.Collections;

/// <summary>
/// Author:joi
/// Desc:加载到登录场景后生成UICanvas，
/// </summary>
namespace COMMON
{
    public class CreateUICanvas : MonoBehaviour
    {
        public GameObject[] DontDestoryObjs;

        static bool IsCloned = false;
        // Use this for initialization
        void Awake()
        {
            if(!IsCloned)
            {
                for(int i=0;i<DontDestoryObjs.Length;++i)
                {
                    GameObject canvasObj = DontDestoryObjs[i];
                    if(null != canvasObj)
                    {
                        ///把各Canvas分开不同位置，避免重复渲染相同UI，也可以减少UIlayer种类
                        GameObject obj = GameObject.Instantiate<GameObject>(canvasObj);
                        if (canvasObj.name == "TopCanvas")
                        {
                            obj.transform.position = Vector3.left * 200f;
                        }
                        if (canvasObj.name == "Canvas2")
                        {
                            obj.transform.position = Vector3.right * 200f;
                        }
                        if (canvasObj.name == "Canvas")
                        {
                            obj.transform.position = Vector3.down * 200f;
                        }
                        obj.name = canvasObj.name;
                    }
                }

            }
            IsCloned = true;
        }
    }

}

