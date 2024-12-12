using DataAcessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingRoom.Pages.BookingRoomUser
{
    public class BookingRoomPageModel : PageModel
    {
        private readonly BookingRoomContext _context;

        public BookingRoomPageModel(BookingRoomContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateOnly CheckInDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateOnly CheckOutDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RoomId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SlotId { get; set; }

        [BindProperty(SupportsGet = true)]

        [Required]
        public string Email { get; set; }
        public Room? Room { get; set; }
        public TimeSlot? Slot { get; set; }
        [BindProperty]
        public Booking Booking { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            if(HttpContext.Session.GetString("UserId") == null)
            {
                TempData["error"] = "Please login to book a room.";
                return RedirectToPage("/Auth/Login");
            }

            Room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RoomId == RoomId);
            Slot = await _context.TimeSlots.FirstOrDefaultAsync(s => s.SlotId == SlotId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {   
                var userId = _context.Users.FirstOrDefault(u => u.Email == Email).UserId;

                var a = Booking.PaymentMethod;

                var booking = new Booking
                {
                    UserId = userId,
                    RoomId = RoomId,
                    CheckInDate = CheckInDate,
                    PaymentMethod = Booking.PaymentMethod,
                    Status = "pending",
                    PaymentStatus = "unpaid",
                    SlotId = SlotId
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Page();
            }


            TempData["success"] = "Booking successful!";
            return RedirectToPage("/BookingRoomUser/Index");
        }
    }
   
}
