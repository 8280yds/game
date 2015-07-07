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
            m_color = value;
            image.color = m_color;
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

    public TentacleNode(Color color)
    {
        m_color = color;
    }

    public TentacleNode()
    {
    }

    public override void init(UnityEngine.GameObject gameObject)
    {
        base.init(gameObject);

        //更新状态
        color = m_color;
    }
}
