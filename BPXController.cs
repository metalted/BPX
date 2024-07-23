using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public struct BPXControllerConfiguration
    {

    }

    public class BPXController : MonoBehaviour
    {
        private bool init = false;

        public void Initialize()
        {
            if (init) { return; }

            init = true;
        }

        private bool AllowRun()
        {
            if(BPXManager.central == null)
            {
                return false;
            }

            //If the regular save panel is open we don't want to process any inputs.
            if (BPXManager.central.saveload.gameObject.activeSelf)
            {
                return false;
            }

            //Not in building mode.
            if (BPXManager.central.tool.currentTool != 0)
            {
                return false;
            }

            //Blueprint panel is open.
            if (BPXUIManagement.IsPanelOpen())
            {
                return false;
            }

            //If we are currently dragging on object.
            if(BPXManager.central.gizmos.isDragging)
            {
                return false;
            }

            //If we are currently in GMode
            if(BPXManager.central.gizmos.isGrabbing)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {
            /*if (!init || !AllowRun()) { return; }
            

            if (BPXConfiguration.DoScrollScaling()) { ScrollScaling(); }

            if (BPXConfiguration.DoKeyScaling()) { KeyScaling(); }

            if(BPXConfiguration.Do)*/

        }
    }
}
