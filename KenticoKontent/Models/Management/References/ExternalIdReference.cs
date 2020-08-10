namespace KenticoKontent.Models.Management.References
{
    public class ExternalIdReference : Reference
    {
        public ExternalIdReference(string external_id) : base(ReferenceType.ExternalId, external_id)
        {
        }
    }
}