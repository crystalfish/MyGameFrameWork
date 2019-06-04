using UnityEngine;
using System.Collections;
using System;

namespace COMMON
{
    /// <summary>
    /// 继承自BasePanel的适配器
    /// 实现了抽象类的方法，子类继承不必重复实现
    /// Author :joi
    /// </summary>
    public class BasePanelAdapter : BasePanel
    {
        /// <summary>
        /// 初始化方法
        /// </summary>
        public override void Init()
        {
            
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        public override void InitEvent()
        {
            
        }

        /// <summary>
        /// 界面回退时调用
        /// </summary>
        public override void OnBack()
        {
            
        }

        /// <summary>
        /// 界面销毁时调用
        /// </summary>
        public override void OnDestroy()
        {
            
        }

        /// <summary>
        /// 界面关闭时调用
        /// </summary>
        public override void OnHide()
        {
            
        }

        /// <summary>
        /// 界面打开时调用
        /// </summary>
        public override void OnShow()
        {
            
        }

        /// <summary>
        /// 界面每帧更新调用（目前暂时好像没啥用）
        /// </summary>
        public override void Update()
        {
            
        }
    }
}