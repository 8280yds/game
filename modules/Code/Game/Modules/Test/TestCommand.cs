using CLRSharp;
using Freamwork;

public class TestCommand : Command
{
    public override void execute(object param)
    {
        TestModel testModel = MVCCharge.instance.getInstance(typeof(TestModel) as ICLRType) as TestModel;
        testModel.count = (int)param;
    }
    
}
