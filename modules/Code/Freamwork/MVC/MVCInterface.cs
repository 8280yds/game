using CLRSharp;
using UnityEngine;

namespace Freamwork
{
    public interface IMVCObject
    {
        ICLRType getCLRType { get; }
        void sendCommand(ICLRType clrType, object param);
        void dispose();
    }

    public interface ICommand : IMVCObject
    {
        void execute(object param);
    }

    public interface IModel : IMVCObject
    {
        void clearAll();
    }

    public interface IView : IMVCObject
    {
        void init(GameObject gameObject, string[] funNames);
    }

}
