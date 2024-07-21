namespace Auctio.Items.DTOs;
public record Item(
    int Id, 
    string Name, 
    string UserId,
    string UserName,
    DateTime CreatedAt,
    string? Description, 
    double? Cost
    );