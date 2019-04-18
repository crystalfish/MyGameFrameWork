using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COMMON;
public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TableManager.Instance.TableLoad();
        //SimpleSingleton<MAudioManager>.Instance.Play(MAudioIdType.BackGround, transform, 999, 1, true, 5);
        TBL.WOLF_BASE.Data.Values wolf_base;
      if(  TBL.WOLF_BASE.Data.map.TryGetValue(1 ,out wolf_base)){
            COMMON.Logger.LogError("测试数据"+wolf_base.GRAVITY + wolf_base.WOLF_NAME );
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
