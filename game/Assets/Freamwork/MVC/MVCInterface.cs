namespace Freamwork.MVC
{
    public interface IMVCObject
    {
        void sendCommand<TCommand, TParam>(TParam param) where TCommand : Command<TCommand>, new();
        void dispose();
    }

    public interface ICommand : IMVCObject
    {
        void execute<TParam>(TParam param);
    }

    public interface IModel : IMVCObject
    {
        void clearAll();
    }

    public interface IView : IMVCObject
    {

    }

}
