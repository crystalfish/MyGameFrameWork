using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFocus : MonoBehaviour
{
    public Camera mCamera;
    // Start is called before the first frame update
    private Vector3 lastCaPos = Vector3.zero;
    private Vector3 curCaPos = Vector3.zero;
    private Vector3 curCubeTar = Vector3.zero;
    public GameObject mCube;
    void Start()
    {
        mCamera = Camera.main;
        lastCaPos = mCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if(GUI.Button( new Rect(0,0,20,20),"重置相机位置"))
        {
            lastCaPos = mCamera.transform.position;
        }
    }
    private void LateUpdate()
    {
        
        if( caIsMove())
        {

            mCube.transform.position = Vector3.Lerp(mCube.transform.position,curCubeTar,Time.deltaTime * 5f);
        }
        //else
        //{

        //}
        Vector3 one = mCamera.transform.position;
        
        mCube.transform.position = new Vector3(one.x , one.y , 0) ;
    }

    public bool caIsMove() {

        Vector3 curCamPos = mCamera.transform.position;
        float dis = Vector3.Distance(curCamPos ,lastCaPos);
        if(dis > 10)
        {
            this.curCaPos = curCamPos;
            this.curCubeTar = curCamPos + mCamera.transform.forward * 10;
            return true;
        }
        curCaPos = Vector3.zero;
        return false;
    }
}
