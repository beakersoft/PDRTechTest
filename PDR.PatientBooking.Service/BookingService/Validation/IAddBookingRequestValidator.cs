using PDR.PatientBooking.Service.BookingService.Requests;
using PDR.PatientBooking.Service.Validation;
using System;

namespace PDR.PatientBooking.Service.BookingService.Validation
{
    public interface IAddBookingRequestValidator
    {
        PdrValidationResult ValidateRequest(BookingRequest request);
        PdrValidationResult ValidateRequestCancelBooking(Guid bookingId);
    }
}
