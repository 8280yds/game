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
    /// HP文本
    /// </summary>
    public Text hpTxt
    {
        get
        {
            if (m_hpTxt == null)
            {
                m_hpTxt = transform.FindChild("HPText").GetComponent<Text>();
            }
            return m_hpTxt;
        }
    }
    private Text m_hpTxt;

    /// <summary>
    /// Star文本
    /// </summary>
    public Text starTxt
    {
        get
        {
            if (m_starTxt == null)
            {
                m_starTxt = transform.FindChild("StarText").GetComponent<Text>();
            }
            return m_starTxt;
        }
    }
    private Text m_starTxt;

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
            //starTxt.color = hpTxt.color = CellConstant.CAMP_COLOR_ARR[(int)m_camp];
            image.color = CellConstant.CAMP_COLOR_ARR[(int)m_camp];
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
            hpTxt.text = "" + m_hp;
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

    public int index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
        }
    }
    private int m_index = -1;

    /// <summary>
    /// 细胞数据
    /// </summary>
    public CellData cellData
    {
        get;
        private set;
    }

    //===================================更新==========================================
    public void UpdateByData(CellData data)
    {
        cellData = data;
        camp = data.camp;
        hp = data.hp;

        //显示星级
        int maxNum = data.vo.tentacle;
        int tentacleNum = data.tentacleList.Count;
        string str = "";
        for (int i = 0; i < maxNum; i++ )
        {
            str += i < tentacleNum ? "★" : "☆";
        }
        starTxt.text = str;
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

        //进行攻击或支援
        if (SelectedCell != null && DestCell != null)
        {
            if (SelectedCell.cellData.vo.tentacle > SelectedCell.cellData.tentacleList.Count)
            {
                view.attack(SelectedCell, DestCell);
            }
            else
            {
                PopUpTextManager.instance.addText("<color=red>触手数量已达上限</color>");
            }
        }

        //清除
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
        view.hideMouseTentacle();
    }

    protected override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (DestCell == null && SelectedCell != null)
        {
            //显示细胞和鼠标之间的触手
            Vector2 pos;
            Canvas canvas = UIManager.instance.canvas;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, eventData.position, Camera.main, out pos))
            {
                view.updateMouseTentacle(SelectedCell, pos);
            }
        }
    }

    protected override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (SelectedCell == this)
        {
            view.hideMouseTentacle();
        }
        else if (SelectedCell != null)
        {
            selectImage.gameObject.SetActive(true);
            DestCell = this;

            //显示两细胞间的触手
            view.updateMouseTentacle(SelectedCell, DestCell);
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
            selectImage.transform.eulerAngles += new Vector3(0, 0, 2f);
        }
    }
}
