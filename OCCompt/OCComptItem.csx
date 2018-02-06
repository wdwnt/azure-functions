public class OCComptItem
{
    public string Id { get; set; }
    
    private string _grantor;
    public string Grantor
    {
        get => _grantor;
        set => _grantor = value.Replace("&nbsp", String.Empty)
                                .Replace("<b>Grantor: </b>  ", String.Empty);
    }
    
    private string _grantee;
    public string Grantee
    {
        get => _grantee;
        set => _grantee = value.Replace("&nbsp", String.Empty)
                                .Replace("<b>Grantee: </b>  ", String.Empty);
    }
    
    public DateTime ReceivedDate { get; set; }
    public string PdfLink { get; set; }
}
