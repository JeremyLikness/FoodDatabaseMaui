namespace comparison;

public class FoodCategory
{
    public string? Description { get; set; }
    public int Id { get; set; }
    public string? Code { get; set; }
    public List<FoundationFood> FoundationFoods { get; set; }
        = new List<FoundationFood>();

    public override string ToString() => Description ?? string.Empty;        
}
    
