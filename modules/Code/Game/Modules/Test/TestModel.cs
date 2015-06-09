using Freamwork;

public class TestModel : Model
{
    public int count
    {
        get
        {
            return m_count;
        }
        set
        {
            m_count = value;
            dispatch(TestCostant.COUNT_CHANGE);
        }
    }
    private int m_count;

}
