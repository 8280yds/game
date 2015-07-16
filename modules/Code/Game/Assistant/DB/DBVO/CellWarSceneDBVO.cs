using Freamwork;
using System.Xml;

public class CellWarSceneDBVO : DBVO
{
	/// <summary>
	/// 细胞阵营
	/// </summary>
	public string cellCamp;

	/// <summary>
	/// 细胞生命
	/// </summary>
	public string cellHP;

	/// <summary>
	/// 细胞x位置
	/// </summary>
	public string cellX;

	/// <summary>
	/// 细胞y位置
	/// </summary>
	public string cellY;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		cellCamp = xmlelement.GetAttribute("cellCamp");
		cellHP = xmlelement.GetAttribute("cellHP");
		cellX = xmlelement.GetAttribute("cellX");
		cellY = xmlelement.GetAttribute("cellY");
	}
}