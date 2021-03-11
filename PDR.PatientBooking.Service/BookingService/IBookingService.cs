using System;
using System.Threading.Tasks;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingService.Requests;

namespace PDR.PatientBooking.Service.BookingService
{
    public interface IBookingService
    {
        Task AddBookingAsync(BookingRequest request);

        Task CancelBookingAsync(Guid bookingId);

        Order GetPatientNextAppointment(long identificationNumber);
    } 
}
