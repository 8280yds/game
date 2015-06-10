using CLRSharp;
using UnityEngine.EventSystems;

namespace Freamwork
{
    public class UIMonoBehaviour : GMonoBehaviour,
        IPointerDownHandler,IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
    {
        virtual public void OnPointerDown(PointerEventData eventData)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(PointerEventData));
            object[] paramList = new object[] { eventData };
            doFun(GMBEventMethod.OnPointerDown, paramTypeList, paramList);
        }

        virtual public void OnPointerUp(PointerEventData eventData)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(PointerEventData));
            object[] paramList = new object[] { eventData };
            doFun(GMBEventMethod.OnPointerUp, paramTypeList, paramList);
        }

        virtual public void OnPointerClick(PointerEventData eventData)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(PointerEventData));
            object[] paramList = new object[] { eventData };
            doFun(GMBEventMethod.OnPointerClick, paramTypeList, paramList);
        }

        virtual public void OnPointerEnter(PointerEventData eventData)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(PointerEventData));
            object[] paramList = new object[] { eventData };
            doFun(GMBEventMethod.OnPointerEnter, paramTypeList, paramList);
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(PointerEventData));
            object[] paramList = new object[] { eventData };
            doFun(GMBEventMethod.OnPointerExit, paramTypeList, paramList);
        }







    }
}
