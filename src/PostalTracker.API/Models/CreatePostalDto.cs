using System.ComponentModel.DataAnnotations;

namespace PostalTracker.API.Models;

public class CreatePostalDto
{
    [Required]
    public Guid PostalId { get; set; }

    [Required, StringLength(250)]
    public string AddressDelivery { get; set; }
    
    [Required, StringLength(250)]
    public string AddressSender { get; set; }
}