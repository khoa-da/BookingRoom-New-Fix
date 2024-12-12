using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAcessObject.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingRoom.Pages.BookingRoomUser
{
    public class IndexModel : PageModel
    {
        private readonly BookingRoomContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(BookingRoomContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public DateOnly CheckInDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [BindProperty]
        public DateOnly CheckOutDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [BindProperty]
        public int NumberOfPeople { get; set; }
        [BindProperty]
        public int SlotId { get; set; }
        [BindProperty]
        public int CategoryId { get; set; }

        public List<RoomViewModel> AvailableRooms { get; set; } = new List<RoomViewModel>();

        public async Task OnGetAsync()
        {
            var category = await _context.RoomCategories.ToListAsync();
            ViewData["CategoryId"] = new SelectList(category, "CategoryId", "CategoryName");

            var slot = await _context.TimeSlots.ToListAsync();
            ViewData["SlotId"] = new SelectList(slot, "SlotId", "Description");
        }

        public async Task<IActionResult> OnPostAsync()
        {          

            try
            {
                AvailableRooms = await SearchRoomsAsync(CheckInDate, CategoryId, SlotId);
                AvailableRooms.ForEach(room =>
                {
                    room.CheckInDate = CheckInDate;
                    room.CheckOutDate = CheckOutDate;
                });

                return (IActionResult)OnGetAsync();
            }
            catch (Exception ex)
            {
                return Page();
            }
        }

        private async Task<List<RoomViewModel>> SearchRoomsAsync(DateOnly checkInDate, int categoryId, int slotId)
        {
            // Base query for rooms
            var query = _context.Rooms.AsQueryable();

            // Filter by category
            query = query.Where(r => r.CategoryId == categoryId);

            // Check room availability
            query = query.Where(room =>
                !_context.Bookings.Any(reservation =>
                    reservation.RoomId == room.RoomId &&
                    reservation.CheckInDate == checkInDate &&
                    reservation.SlotId == slotId
                ) && room.Status == "available"
            );

            // Return the list of available rooms
            return await query
                .Select(r => new RoomViewModel
                {
                    RoomId = r.RoomId,
                    RoomName = r.RoomName,
                    TotalCapacity = r.TotalCapacity,
                    BasePrice = r.BasePrice,
                    Category = r.Category,
                    Status = r.Status
                })
                .ToListAsync();
        }

    }
    public class SearchReservationViewModel
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfPeople { get; set; }
        public List<RoomViewModel> AvailableRooms { get; set; } = new List<RoomViewModel>();
    }

    public class RoomViewModel
    {
        public int RoomId { get; set; }

        public int? CategoryId { get; set; }

        public string RoomName { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public int TotalCapacity { get; set; }

        public string? Status { get; set; }
        public TimeSlot? Slot { get; set; }
        public virtual RoomCategory? Category { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }
}