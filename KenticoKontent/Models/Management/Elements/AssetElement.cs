namespace KenticoKontent.Models.Management.Elements
{
    public class AssetElement : AbstractReferenceListElement
    {
        public AssetElement(AbstractReferenceListElement element)
        {
            Element = element.Element;
            Value = element.Value;
        }
    }
}