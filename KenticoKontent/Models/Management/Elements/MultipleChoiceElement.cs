namespace KenticoKontent.Models.Management.Elements
{
    public class MultipleChoiceElement : AbstractReferenceListElement
    {
        public MultipleChoiceElement(AbstractReferenceListElement element)
        {
            Element = element.Element;
            Value = element.Value;
        }
    }
}