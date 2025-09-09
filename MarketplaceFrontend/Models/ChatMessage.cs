using System;

namespace MarketplaceAPI.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string MessageText { get; set; } = "";
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }

        public Guid? ListingId { get; set; }
        public Listing Listing { get; set; }
    }
}
