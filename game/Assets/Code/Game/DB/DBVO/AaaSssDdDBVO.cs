using Freamwork;
using System.Xml;

public class AaaSssDdDBVO : DBVO
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name;

	/// <summary>
	/// 年龄
	/// </summary>
	public int age;

	/// <summary>
	/// 性别
	/// </summary>
	public string sex;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		name = xmlelement.GetAttribute("name");
		age = int.Parse(xmlelement.GetAttribute("age"));
		sex = xmlelement.GetAttribute("sex");
	}
}