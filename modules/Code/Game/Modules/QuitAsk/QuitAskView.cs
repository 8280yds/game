using Freamwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitAskView : Window
{
    private Button yesBtn
    {
        get
        {
            if (m_yesBtn == null)
            {
                m_yesBtn = transform.FindChild("YesBtn").GetComponent<Button>();
            }
            return m_yesBtn;
        }
    }
    private Button m_yesBtn;

    private Button noBtn
    {
        get
        {
            if (m_noBtn == null)
            {
                m_noBtn = transform.FindChild("NoBtn").GetComponent<Button>();
            }
            return m_noBtn;
        }
    }
    private Button m_noBtn;
    
    //========================================================================
    protected override void onShow()
    {
        base.onShow();
        yesBtn.onClick.AddListener(onYesBtnClock);
        noBtn.onClick.AddListener(onNoBtnClock);
    }

    private void onYesBtnClock()
    {
        Application.Quit();
    }

    private void onNoBtnClock()
    {
        close();
    }

    public override void dispose()
    {
        yesBtn.onClick.RemoveListener(onYesBtnClock);
        noBtn.onClick.RemoveListener(onNoBtnClock);
        base.dispose();
    }

    protected override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onNoBtnClock();
    }

}
