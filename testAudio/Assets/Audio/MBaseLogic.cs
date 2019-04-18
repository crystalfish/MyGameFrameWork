//joi

using UnityEngine;
using System.Collections;
namespace COMMON
{ 
public   abstract class MBaseLogic  {

        public virtual void DoUpdate()
        {
        }

        public virtual void DoFixedUpdate()
        {

        }

        public virtual void DoLateUpdate()
        {
        }

        public virtual void DoOnGUI()
        {
        }

        public virtual void DoClean()
        {
        }

        public virtual void DoDestroy()
        {
            this.DoClean();
        }

        public virtual void DoOnDrawGizmos()
        {
        }

    }
}
