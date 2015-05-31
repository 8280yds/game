using Freamwork;

public class TestModel : Model
{
    private int m_clickCount;

    public TestModel()
    {
        
    }

    protected override void init()
    {
        m_clickCount = 0;
    }

    public int clickCount
    {
        get
        {
            return m_clickCount;
        }
        set
        {
            m_clickCount = value;
            dispatch(TestConstant.COUNT_CHANGE);
        }
    }

}
