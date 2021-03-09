using PDR.PatientBooking.Service.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.DoctorServices.Requests
{
    public class AddDoctorRequest
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "FirstName must be populated")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName must be populated")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; }
    }
}
