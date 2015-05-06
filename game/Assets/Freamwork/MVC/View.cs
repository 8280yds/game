using System;

namespace Freamwork.MVC
{
    public class View<TClass> : MVCObject<TClass>, IView where TClass : View<TClass>
    {
        public View()
            : base()
        {

        }


    }
}
