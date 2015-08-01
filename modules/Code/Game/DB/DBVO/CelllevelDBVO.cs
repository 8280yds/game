using Freamwork;
using System.Xml;

public class CelllevelDBVO : DBVO
{
	/// <summary>
	/// 等级
	/// </summary>
	public int level;

	/// <summary>
	/// hp最小值
	/// </summary>
	public int minHp;

	/// <summary>
	/// hp最大值
	/// </summary>
	public int maxHp;

	/// <summary>
	/// 触手数量
	/// </summary>
	public int tentacle;

	/// <summary>
	/// 回复速度(ms)
	/// </summary>
	public int addTime;

	/// <summary>
	/// 攻击间隔(ms)
	/// </summary>
	public int attackTime;

	public override void xmlToVo(XmlNode node)
	{
		XmlElement xmlelement = (XmlElement)node;
		id = int.Parse(xmlelement.GetAttribute("id"));
		level = int.Parse(xmlelement.GetAttribute("level"));
		minHp = int.Parse(xmlelement.GetAttribute("minHp"));
		maxHp = int.Parse(xmlelement.GetAttribute("maxHp"));
		tentacle = int.Parse(xmlelement.GetAttribute("tentacle"));
		addTime = int.Parse(xmlelement.GetAttribute("addTime"));
		attackTime = int.Parse(xmlelement.GetAttribute("attackTime"));
	}
}