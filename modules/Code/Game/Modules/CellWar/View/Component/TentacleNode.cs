using Freamwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TentacleNode : GMB
{
    /// <summary>
    /// 所在的触手
    /// </summary>
    public Tentacle tentacle;

    public int index;

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

    protected override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        tentacle.cut(this);
    }

    public void clear()
    {
        tentacle = null;
    }
}
