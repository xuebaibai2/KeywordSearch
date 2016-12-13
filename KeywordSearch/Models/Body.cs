using System.Xml.Serialization;

namespace KeywordSearch.Models
{
    public class Body
    {
        public Body()
        {
            Div = new Div();
            Table = new Table();
        }
        [XmlElement(ElementName = "div")]
        public Div Div { get; set; }
        [XmlElement(ElementName = "table")]
        public Table Table { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string _class { get; set; }
        [XmlAttribute(AttributeName = "bgcolor")]
        public string Bgcolor { get; set; }
        [XmlAttribute(AttributeName = "marginheight")]
        public string Marginheight { get; set; }
        [XmlAttribute(AttributeName = "marginwidth")]
        public string Marginwidth { get; set; }
        [XmlAttribute(AttributeName = "topmargin")]
        public string Topmargin { get; set; }
    }

    public class Table
    {

    }

    public class Div
    {
        [XmlAttribute(AttributeName ="id")]
        public string id { get; set; }
        [XmlAttribute(AttributeName ="class")]
        public string _class { get; set; }
    }
}