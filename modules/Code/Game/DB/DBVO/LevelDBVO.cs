using Freamwork;
using System.Xml;

public class LevelDBVO : DBVO
{
	/// <summary>
	/// UFO资源
	/// </summary>
	public string ufoImage;

	/// <summary>
	/// 星球X坐标
	/// </summary>
	public string starX;

	/// <summary>
	/// 星球Y坐标
	/// </summary>
	public string starY;

	/// <summary>
	/// 星球资源
	/// </summary>
	public string starImage;

	/// <summary>
	/// 对应关卡id
	/// </summary>
	public string war;

	/// <summary>
	/// 下个星球索引
	/// </summary>
	public string next;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		ufoImage = xmlelement.GetAttribute("ufoImage");
		starX = xmlelement.GetAttribute("starX");
		starY = xmlelement.GetAttribute("starY");
		starImage = xmlelement.GetAttribute("starImage");
		war = xmlelement.GetAttribute("war");
		next = xmlelement.GetAttribute("next");
	}
}