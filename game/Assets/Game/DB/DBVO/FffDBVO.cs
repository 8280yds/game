using Freamwork;
using System.Xml;

public class FffDBVO : DBVO
{
	//id
	public int id;
	//名字
	public string name;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		name = xmlelement.GetAttribute("name");
	}
}