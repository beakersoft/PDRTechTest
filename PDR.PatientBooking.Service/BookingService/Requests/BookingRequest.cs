using System;
using System.ComponentModel.DataAnnotations;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.Attributes;

namespace PDR.PatientBooking.Service.BookingService.Requests
{
    public class BookingRequest
    {
        public Guid Id { get; set; }
        [IsDateInPast(ErrorMessage = "StartTime must not be in the past")]
        public DateTime StartTime { get; set; }
        [IsDateInPast(ErrorMessage = "EndTime must not be in the past")]
        public DateTime EndTime { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public long PatientId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public long DoctorId { get; set; }

        public Order ToNewOrder(Patient patient,Doctor doctor)
        {
            return new Order
            {
                StartTime = StartTime,
                EndTime = EndTime,
                PatientId = PatientId,
                DoctorId = DoctorId,
                Patient = patient,
                Doctor = doctor,
                SurgeryType = (int)patient.Clinic.SurgeryType
            };
        }
    }
}
