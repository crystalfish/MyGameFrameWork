using UnityEngine;
using System.Collections;

public class SetObjNotDestory : MonoBehaviour {

	// Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
	
}
