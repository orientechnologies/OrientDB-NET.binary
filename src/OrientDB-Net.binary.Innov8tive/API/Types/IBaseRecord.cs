namespace Orient.Client
{
    public interface IBaseRecord
    {
        ORID ORID { get; set; }
        int OVersion { get; set; }
        short OClassId { get; set; }
        string OClassName { get; set; }
    }
}