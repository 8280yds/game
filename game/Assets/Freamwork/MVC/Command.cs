namespace Freamwork.MVC
{
    public class Command : MVCObject, ICommand
    {
        public Command()
            : base()
        {

        }

        internal void doExecute<TParam>(TParam param = default(TParam))
        {
            execute<TParam>(param);
            dispose();
        }

        virtual public void execute<TParam>(TParam param = default(TParam))
        {

        }
    }
}
