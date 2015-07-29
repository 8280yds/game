using CLRSharp;
using Freamwork;
using UnityEngine.UI;

public class OpenGateView : Window
{
    private Button beginBtn
    {
        get
        {
            if (m_beginBtn == null)
            {
                m_beginBtn = transform.FindChild("BeginBtn").GetComponent<Button>();
            }
            return m_beginBtn;
        }
    }
    private Button m_beginBtn;

    private Button quitBtn
    {
        get
        {
            if (m_quitBtn == null)
            {
                m_quitBtn = transform.FindChild("QuitBtn").GetComponent<Button>();
            }
            return m_quitBtn;
        }
    }
    private Button m_quitBtn;

    //==========================================================================
    protected override void onShow()
    {
        base.onShow();
        beginBtn.onClick.AddListener(onBeginBtnClock);
        quitBtn.onClick.AddListener(onQuitBtnClock);
    }

    private void onBeginBtnClock()
    {
        close();
        LevelView view = mvcCharge.getInstance(typeof(LevelView) as ICLRType) as LevelView;
        view.show();
    }

    private void onQuitBtnClock()
    {
        QuitAskView view = mvcCharge.getInstance(typeof(QuitAskView) as ICLRType) as QuitAskView;
        view.show();
    }

    public override void dispose()
    {
        beginBtn.onClick.RemoveListener(onBeginBtnClock);
        quitBtn.onClick.RemoveListener(onQuitBtnClock);
        base.dispose();
    }
    
}
