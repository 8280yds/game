namespace Freamwork.MVC
{
    public class Command<TClass> : MVCObject<TClass>, ICommand where TClass : Command<TClass>
    {
        public Command()
            : base()
        {

        }

        internal void doExecute<TParam>(TParam param)
        {
            execute<TParam>(param);
            dispose();
        }

        virtual public void execute<TParam>(TParam param)
        {

        }
    }
}
