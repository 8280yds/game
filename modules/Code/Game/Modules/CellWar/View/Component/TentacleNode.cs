using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class TentacleNode : GMB
{
    public Color color
    {
        get
        {
            return m_color;
        }
        set
        {
            if (m_color != value)
            {
                m_color = value;
                image.color = m_color;
            }
        }
    }
    private Color m_color;

    public Image image
    {
        get
        {
            if (m_image == null)
            {
                m_image = transform.GetComponent<Image>();
            }
            return m_image;
        }
    }
    private Image m_image;

    public override void init(GameObject gameObject)
    {
        base.init(gameObject);
        m_color = image.color;
    }
}
