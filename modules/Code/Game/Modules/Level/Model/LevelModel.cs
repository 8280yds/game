using Freamwork;

public class LevelModel : Model
{
    public int enemyLevel
    {
        get
        {
            return m_enemyLevel;
        }
        set
        {
            m_enemyLevel = value;
            dispatch(LevelUpdateConstant.UPDATE_LEVEL_VIEW);
        }
    }
    private int m_enemyLevel = 1;

}
