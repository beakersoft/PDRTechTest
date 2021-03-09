using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.DoctorServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PDR.PatientBooking.Service.DoctorServices.Validation
{
    public class AddDoctorRequestValidator : IAddDoctorRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddDoctorRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddDoctorRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (DoctorAlreadyInDb(request, ref result))
                return result;

            return result;
        }

        public bool MissingRequiredFields(AddDoctorRequest request, ref PdrValidationResult result)
        {
            var validationContext = new ValidationContext(request);
            var results = new List<ValidationResult>(); 
            bool isValid = Validator.TryValidateObject(request, validationContext, results, true);

            if (!isValid)
            {
                result.PassedValidation = false;
                result.Errors.AddRange(results.Select(x =>x.ErrorMessage).ToList());
                return true;
            }

            return false;
        }

        private bool DoctorAlreadyInDb(AddDoctorRequest request, ref PdrValidationResult result)
        {
            if (_context.Doctor.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor with that email address already exists");
                return true;
            }

            return false;
        }
    }
}
