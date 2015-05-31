using UnityEngine;

namespace Freamwork
{
    public class EnterFrameComponent : MonoBehaviour
    {
        private EnterFrame enterFrame;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            enterFrame = EnterFrame.instance;
        }

        void Update()
        {
            enterFrame.doEnterFrame();
        }

        //void OnDestroy()
        //{
        //    enterFrame.destroyError();
        //}
    }
}
