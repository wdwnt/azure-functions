public class MerchandiseItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string DetailsUrl { get; set; }
    public string Price { get; set; }

    public override string ToString() => $"Item: {Id} - {Name} - {Price}";
}
