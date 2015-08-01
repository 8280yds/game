using Freamwork;
using System.Xml;

public class BbbWwwDBVO : DBVO
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name;

	/// <summary>
	/// 年龄
	/// </summary>
	public int age;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		name = xmlelement.GetAttribute("name");
		age = int.Parse(xmlelement.GetAttribute("age"));
	}
}