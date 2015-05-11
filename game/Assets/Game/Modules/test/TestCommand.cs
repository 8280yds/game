using Freamwork.MVC;

public class TestCommand : Command
{
    public override void execute(object param)
    {
        if (MVCCharge.instance.hasView<TestView2>())
        {
            string str = param as string;
            TestView2 view2 = MVCCharge.instance.getView<TestView2>();
            view2.showText(str);
        }
    }

}
