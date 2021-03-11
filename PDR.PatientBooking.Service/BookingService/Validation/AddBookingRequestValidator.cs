using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingService.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingService.Validation
{
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;
        private PdrValidationResult _validationResult;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(BookingRequest request)
        {
            _validationResult = new PdrValidationResult(true);
;
            //run all the checks then return the errors back, that way if there's
            //lots of errors the user can fix them all at once instead of keep submitting again
            CheckDoctorExists(request);
            CheckPatientExists(request);
            CheckRequestedDatesAreValid(request);
            
            return _validationResult;
        }

        public PdrValidationResult ValidateRequestCancelBooking(Guid bookingId)
        {
            _validationResult = new PdrValidationResult(true);

            CheckBookingCanBeCanceled(bookingId);
            
            return _validationResult;
        }

        public void CheckBookingCanBeCanceled(Guid bookingId)
        {
            var booking = _context.Order.Find(bookingId);

            if (booking == null)
            {
                _validationResult.PassedValidation = false;
                _validationResult.Errors.Add($"Booking on id {bookingId} not found");
                return;
            }

            //if we are operating across times zones i think we need to hold the booking times at UTC or or as offsets
            if (booking.StartTime < DateTime.Now)
            {
                _validationResult.PassedValidation = false;
                _validationResult.Errors.Add($"Booking is in the past and so cannot be canceled");
            }

            if (booking.CanceledTimeUtc != null)
            {
                _validationResult.PassedValidation = false;
                _validationResult.Errors.Add($"Booking has already been canceld");
            }

            //LPN also int the real world if we has some auth then check the person has access to cancel the booking
        }

        private void CheckRequestedDatesAreValid(BookingRequest request)
        {
            if (request.EndTime < request.StartTime)
            {
                _validationResult.PassedValidation = false;
                _validationResult.Errors.Add($"Please make sure the booking end time is after the start time");
                return;
            }

            var doctorNotAvailable = _context.Order
                .Any(order => 
                        order.DoctorId == request.DoctorId
                        && order.StartTime > DateTime.UtcNow    //no point looking in the past
                        && request.StartTime < order.EndTime && order.StartTime < request.EndTime
                    );

            if (doctorNotAvailable)
            {
                _validationResult.PassedValidation = false;
                _validationResult.Errors.Add($"Doctor is not available at that time");
            }
        }

        private void CheckDoctorExists(BookingRequest request)
        {
            if (_context.Doctor.Any(x => x.Id == request.DoctorId)) 
                return;

            _validationResult.PassedValidation = false;
            _validationResult.Errors.Add($"No Doctor record found for id {request.DoctorId}");
        }
        

        private void CheckPatientExists(BookingRequest request)
        {
            if (_context.Patient.Any(x => x.Id == request.PatientId)) 
                return;

            _validationResult.PassedValidation = false;
            _validationResult.Errors.Add($"No Patient record found for id {request.PatientId}");
        }

        
    }
}
