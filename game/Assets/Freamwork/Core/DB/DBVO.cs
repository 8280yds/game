using System.Xml;

public abstract class DBVO
{
    public int id;
    public abstract void xmlToVo(XmlNode node);
}
