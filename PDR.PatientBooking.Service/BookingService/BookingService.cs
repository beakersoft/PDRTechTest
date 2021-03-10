using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingService.Requests;
using PDR.PatientBooking.Service.BookingService.Validation;

namespace PDR.PatientBooking.Service.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _validator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<bool> AddBookingAsync(BookingRequest request)
        {
            var validationResult = _validator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
                throw new ArgumentException(validationResult.Errors.Join(", "));

            var doctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            var patient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            var order = request.ToNewOrder(patient, doctor);
            await _context.Order.AddRangeAsync(new List<Order> { order });
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
