using Freamwork;
using System.Xml;

public class FffDBVO : DBVO
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		name = xmlelement.GetAttribute("name");
	}
}