namespace KenticoKontent.Models.Management.Elements
{
    public class TaxonomyElement : AbstractReferenceListElement
    {
        public TaxonomyElement(AbstractReferenceListElement element)
        {
            Element = element.Element;
            Value = element.Value;
        }
    }
}