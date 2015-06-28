using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : GMB
{
    /// <summary>
    /// 被选中的cell
    /// </summary>
    public static Cell SelectedCell = null;

    /// <summary>
    /// 目标cell
    /// </summary>
    public static Cell DestCell = null;

    //==============================================================
    /// <summary>
    /// 细胞图片
    /// </summary>
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

    /// <summary>
    /// 细胞文本
    /// </summary>
    public Text txt
    {
        get
        {
            if (m_txt == null)
            {
                m_txt = transform.FindChild("Text").GetComponent<Text>();
            }
            return m_txt;
        }
    }
    private Text m_txt;

    /// <summary>
    /// 选中图片
    /// </summary>
    public Transform selectImage
    {
        get
        {
            if (m_selectImage == null)
            {
                m_selectImage = transform.FindChild("Select");
            }
            return m_selectImage;
        }
    }
    private Transform m_selectImage;

    /// <summary>
    /// 阵营
    /// </summary>
    public Camp camp
    {
        get
        {
            return m_camp;
        }
        set
        {
            m_camp = value;
        }
    }
    private Camp m_camp;

    /// <summary>
    /// 生命
    /// </summary>
    public int hp
    {
        get
        {
            return m_hp;
        }
        set
        {
            m_hp = value;
            txt.text = "" + m_hp;
        }
    }
    private int m_hp;

    public CellWarView view
    {
        get
        {
            if (m_view == null)
            {
                m_view = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
            } 
            return m_view;
        }
    }
    private CellWarView m_view;

    //==============================================================================
    /// <summary>
    /// 细胞基类
    /// </summary>
    /// <param name="camp">阵营</param>
    /// <param name="hp">生命值</param>
    public Cell(Camp camp, int hp)
    {
        m_camp = camp;
        m_hp = hp;
    }

    public override void init(UnityEngine.GameObject gameObject)
    {
        base.init(gameObject);

        //更新状态
        camp = m_camp;
        hp = m_hp;
    }

    //===================================事件==========================================
    protected override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (camp == Camp.GREEN)
        {
            selectImage.gameObject.SetActive(true);
            SelectedCell = this;
        }
    }

    protected override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (SelectedCell != null)
        {
            SelectedCell.selectImage.gameObject.SetActive(false);
            SelectedCell = null;
        }
        if (DestCell != null)
        {
            DestCell.selectImage.gameObject.SetActive(false);
            DestCell = null;
        }
        view.removeMouseHands();
    }

    protected override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (DestCell == null && SelectedCell != null)
        {
            //显示细胞和鼠标之间的触手
            Vector3 p = SelectedCell.transform.position;
            Vector2 sour = new Vector2(p.x, p.y);
            Vector2 dest = eventData.position;
            float d = Vector2.Distance(sour, dest);
            if (d <= CellConstant.CELL_R + CellConstant.HAND_D)
            {
                view.removeMouseHands();
                return;
            }
            float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
            float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
            view.updateMouseHands(new Vector2(sourX, sourY), dest);
        }
    }

    protected override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        //进行攻击
    }

    protected override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (SelectedCell == this)
        {
            view.removeMouseHands();
        }
        else if (SelectedCell != null)
        {
            selectImage.gameObject.SetActive(true);
            DestCell = this;

            //显示两细胞间的触手
            Vector3 p = SelectedCell.transform.position;
            Vector2 sour = new Vector2(p.x, p.y);
            p = DestCell.transform.position;
            Vector2 dest = new Vector2(p.x, p.y);

            float d = Vector2.Distance(sour, dest);
            float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
            float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
            float destX = dest.x + CellConstant.CELL_R * (sour.x - dest.x) / d;
            float destY = dest.y + CellConstant.CELL_R * (sour.y - dest.y) / d;
            view.updateMouseHands(new Vector2(sourX, sourY), new Vector2(destX, destY));
        }
    }

    protected override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (DestCell == this)
        {
            selectImage.gameObject.SetActive(false);
            DestCell = null;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (selectImage.gameObject.activeInHierarchy)
        {
            selectImage.transform.eulerAngles += new Vector3(0, 0, 1f);
        }
    }

}
