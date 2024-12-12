using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Pages.BookingRoomUser
{
    public class ViewBookedRoomModel : PageModel
    {
        private readonly DataAcessObject.Models.BookingRoomContext _context;
        public ViewBookedRoomModel(DataAcessObject.Models.BookingRoomContext context)
        {
            _context = context;
        }

        public List<RoomViewModel> AvailableRooms { get; set; } = new List<RoomViewModel>();
        public void OnGet()
        {
            if (int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
            {
                AvailableRooms = _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.UserId == userId)
                    .Select(b => new RoomViewModel
                    {
                        RoomId = b.RoomId,
                        RoomName = b.Room.RoomName,
                        Category = b.Room.Category,
                        BasePrice = b.Room.BasePrice,
                        TotalCapacity = b.Room.TotalCapacity,
                        CheckInDate = (DateOnly)b.CheckInDate,
                        Slot = b.Slot
                        
                    })
                    .ToList();
            }
        }
    }
}
