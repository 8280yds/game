using Freamwork;
using System.Xml;

public class AaaSssDdDBVO : DBVO
{
	//id
	public int id;
	//年龄
	public int age;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		age = int.Parse(xmlelement.GetAttribute("age"));
	}
}