using Freamwork;
using System.Xml;

public class BbbWwwDBVO : DBVO
{
	//id
	public int id;
	//名字
	public string name;
	//年龄
	public int age;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		name = xmlelement.GetAttribute("name");
		age = int.Parse(xmlelement.GetAttribute("age"));
	}
}