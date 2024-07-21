namespace Auctio.Items.DTOs;
public record CreateItem(string Name, 
    string? Description, 
    double? Cost);
