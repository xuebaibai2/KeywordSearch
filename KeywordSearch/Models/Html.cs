using System.Xml.Serialization;

namespace KeywordSearch.Models
{
    [XmlRoot(ElementName ="html")]
    public class Html
    {
        public Html()
        {
            Head = new Head();
            Body = new Body();
        }
        [XmlElement(ElementName ="head")]
        public Head Head { get; set; }
        [XmlElement(ElementName = "body")]
        public Body Body { get; set; }

        [XmlAttribute(AttributeName = "itemscope")]
        public string Itemscope { get; set; }
        [XmlAttribute(AttributeName = "itemtype")]
        public string Itemtype { get; set; }
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
    }
}