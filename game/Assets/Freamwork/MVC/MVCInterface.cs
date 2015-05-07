namespace Freamwork.MVC
{
    public interface IMVCObject
    {
        void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new();
        void dispose();
    }

    public interface ICommand : IMVCObject
    {
        void execute<TParam>(TParam param = default(TParam));
    }

    public interface IModel : IMVCObject
    {
        void clearAll();
    }

    public interface IView : IMVCObject
    {

    }

}
