using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Author:joi
/// Desc:播放序列帧
/// </summary>
namespace COMMON
{
    public class SequenceFrameController : MonoBehaviour
    {
        public bool IsClockwise = true;
        public float Speed = 10;
        public int Rows = 1;
        public int Columns = 1;

        float time = 1;
        int curIndex = -1;
        // Use this for initialization

        float width = 0;
        float height = 0;

        int mTotalFrameCount = 0;

        RawImage mIcon;
        void Start()
        {
            width = 1f / Columns;
            height = 1f / Rows;
            mIcon = GetComponent<RawImage>();
            mTotalFrameCount = Rows * Columns;
        }

        // Update is called once per frame
        void Update()
        {
            if (Speed <= 0 || mTotalFrameCount == 0)
            {
                return;
            }
            time += Time.deltaTime;
            if (time >= 1 / Speed)
            {
                time = 0;
                PlayIcon();
            }
        }


        void PlayIcon()
        {
            curIndex++;
            int curFrameIndex = 0;
            int x= 0;
            int y = 0;

            curIndex %= mTotalFrameCount;
            if (IsClockwise)
            {
                curFrameIndex = curIndex;
            }
            else
            {
                curFrameIndex = (mTotalFrameCount - curIndex-1);
            }
            x = curFrameIndex % Columns;
            y = curFrameIndex / Columns;
            mIcon.uvRect = new Rect(x * width, (Rows- y-1) * height, width, height);
        }

    }
}

