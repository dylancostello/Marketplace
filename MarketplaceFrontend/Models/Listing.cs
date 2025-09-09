using System;

namespace MarketplaceAPI.Models
{
    public class Listing
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to User
        public Guid UserId { get; set; }
        public User User { get; set; }
    }

    public class ListingResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } = ""; // Include only the info you need
    }

    public class ListingCreateDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public Guid UserId { get; set; }
    }

    public class ListingUpdateDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public Guid UserId { get; set; }
    }
}
