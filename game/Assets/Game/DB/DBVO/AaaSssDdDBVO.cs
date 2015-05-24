using Freamwork;
using System.Xml;

public class AaaSssDdDBVO : DBVO
{
	/// <summary>
	/// 年龄
	/// </summary>
	public int age;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		age = int.Parse(xmlelement.GetAttribute("age"));
	}
}