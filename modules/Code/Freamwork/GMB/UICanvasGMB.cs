using UnityEngine;
using CLRSharp;

namespace Freamwork
{
    public class UICanvasGMB : GMB
    {
        protected override void Update()
        {
            base.Update();

            // 返回键
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitAskView view = MVCCharge.instance.getInstance(typeof(QuitAskView) as ICLRType) as QuitAskView;
                view.show();
            }

            // Home键
            if (Input.GetKeyDown(KeyCode.Home))
            {
                //...
            }
        }
        
    }
}