using MarketplaceAPI.Data;
using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;

        public ListingsController(MarketplaceDbContext context)
        {
            _context = context;
        }

        // GET: api/listings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListingResponseDto>>> GetListings()
        {
            var listings = await _context.Listings
                .Include(l => l.User)
                .Select(l => new ListingResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Category = l.Category,
                    ImageUrl = l.ImageUrl,
                    CreatedAt = l.CreatedAt,
                    UserId = l.UserId,
                    UserName = l.User.UserName,
                    IsSold = l.IsSold
                })
                .ToListAsync();

            return listings;
        }

        // GET: api/listings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ListingResponseDto>> GetListing(Guid id)
        {
            var listing = await _context.Listings
                .Include(l => l.User)
                .Where(l => l.Id == id)
                .Select(l => new ListingResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Category = l.Category,
                    ImageUrl = l.ImageUrl,
                    CreatedAt = l.CreatedAt,
                    UserId = l.UserId,
                    UserName = l.User.UserName,
                    IsSold = l.IsSold
                })
                .FirstOrDefaultAsync();

            if (listing == null) return NotFound();
            return listing;
        }

        // POST: api/listings
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ListingResponseDto>> CreateListing(ListingCreateDto dto)
        {
            // Get current user ID from JWT
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);

            var listing = new Listing
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();

            var response = new ListingResponseDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                Category = listing.Category,
                ImageUrl = listing.ImageUrl,
                CreatedAt = listing.CreatedAt,
                UserId = listing.UserId,
                UserName = (await _context.Users.FindAsync(userId))?.UserName ?? ""
            };

            return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, response);
        }

        // PUT: api/listings/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateListing(Guid id, ListingUpdateDto dto)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return NotFound();

            // Only allow the owner to update
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || listing.UserId != Guid.Parse(userIdClaim))
                return Forbid();

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.Price = dto.Price;
            listing.Category = dto.Category;
            listing.ImageUrl = dto.ImageUrl;
            listing.IsSold = dto.IsSold;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/listings/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteListing(Guid id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return NotFound();

            // Only allow the owner to delete
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || listing.UserId != Guid.Parse(userIdClaim))
                return Forbid();

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ListingResponseDto>>> GetUserListings()
        {
            // Get current user ID from JWT
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);

            var listings = await _context.Listings
                .Where(l => l.UserId == userId)
                .Select(l => new ListingResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Category = l.Category,
                    ImageUrl = l.ImageUrl,
                    CreatedAt = l.CreatedAt,
                    IsSold = l.IsSold,
                    UserId = l.UserId,
                    UserName = l.User.UserName
                })
                .ToListAsync();

            return listings;
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ListingResponseDto>>> GetActiveListings()
        {

            var listings = await _context.Listings
                .Where(l => !l.IsSold)
                .Select(l => new ListingResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Category = l.Category,
                    ImageUrl = l.ImageUrl,
                    CreatedAt = l.CreatedAt,
                    IsSold = l.IsSold,
                    UserId = l.UserId,
                    UserName = l.User.UserName
                })
                .ToListAsync();

            return listings;
        }

        [HttpPatch("{id}/toggle-sold")]
        public async Task<IActionResult> ToggleSold(Guid id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return NotFound();

            listing.IsSold = !listing.IsSold;
            await _context.SaveChangesAsync();

            return Ok(new { listing.Id, listing.IsSold });
        }
    }
}
