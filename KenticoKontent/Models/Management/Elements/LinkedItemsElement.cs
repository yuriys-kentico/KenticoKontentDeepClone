namespace KenticoKontent.Models.Management.Elements
{
    public class LinkedItemsElement : AbstractReferenceListElement
    {
        public LinkedItemsElement(AbstractReferenceListElement element)
        {
            Element = element.Element;
            Value = element.Value;
        }
    }
}