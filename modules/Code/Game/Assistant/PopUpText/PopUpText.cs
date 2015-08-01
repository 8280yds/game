using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : GMB
{
    private int count = 0;

    public Text txt
    {
        get
        {
            if (m_txt == null)
            {
                m_txt = transform.GetComponent<Text>();
            }
            return m_txt;
        }
    }
    private Text m_txt;

    protected override void Update()
    {
        base.Update();

        count++;
        RectTransform rectTF = transform as RectTransform;
        if (count < 60)
        {
            rectTF.anchoredPosition += new Vector2(0, 1);
        }
        else
        {
            float dy = Mathf.Max(0, 80 - count) / 20f;
            rectTF.anchoredPosition += new Vector2(0, dy);
            txt.color -= new Color(0, 0, 0, 0.02f);
        }

        if (txt.color.a <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
