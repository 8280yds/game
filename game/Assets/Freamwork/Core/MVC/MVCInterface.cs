namespace Freamwork.MVC
{
    public interface IMVCObject
    {
        void sendCommand<TCommand>(object param = null) where TCommand : Command, new();
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

    }

}
