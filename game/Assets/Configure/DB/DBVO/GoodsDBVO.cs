using System.Xml;

public class GoodsDBVO : DBVO
{
	public string prefabName;
	public float speed;
	public float liftTime;
	public int score;
	public int type;
    public string materialName;
    public string musicName;

    public override void xmlToVo(XmlNode node)
    {
        XmlElement xmlelement = (XmlElement)node;
        id = int.Parse(xmlelement.GetAttribute("id"));
        prefabName = xmlelement.GetAttribute("prefabName");
        speed = float.Parse(xmlelement.GetAttribute("speed"));
        liftTime = float.Parse(xmlelement.GetAttribute("liftTime"));
        score = int.Parse(xmlelement.GetAttribute("score"));
        type = int.Parse(xmlelement.GetAttribute("type"));
        materialName = xmlelement.GetAttribute("materialName");
        musicName = xmlelement.GetAttribute("musicName");
    }
}