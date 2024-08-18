namespace Gastronomy.Dtos;

public class UploadPhotoDto
{
    public required string Extension { get; set; }
    public required Stream FileStream { get; set; }
}