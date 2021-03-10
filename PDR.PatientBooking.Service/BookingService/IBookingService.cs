using System.Threading.Tasks;
using PDR.PatientBooking.Service.BookingService.Requests;

namespace PDR.PatientBooking.Service.BookingService
{
    public interface IBookingService
    {
        Task<bool> AddBookingAsync(BookingRequest request);
    }
}
