using Microsoft.Extensions.DependencyInjection;
using PDR.PatientBooking.Service.BookingService;
using PDR.PatientBooking.Service.BookingService.Validation;
using PDR.PatientBooking.Service.ClinicServices;
using PDR.PatientBooking.Service.ClinicServices.Validation;
using PDR.PatientBooking.Service.DoctorServices;
using PDR.PatientBooking.Service.DoctorServices.Validation;
using PDR.PatientBooking.Service.PatientServices;
using PDR.PatientBooking.Service.PatientServices.Validation;

namespace PDR.PatientBooking.Service.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterPatientBookingServices(this IServiceCollection collection)
        {
            collection
                .AddScoped<IPatientService, PatientService>()
                .AddScoped<IAddPatientRequestValidator, AddPatientRequestValidator>()
                .AddScoped<IDoctorService, DoctorService>()
                .AddScoped<IAddDoctorRequestValidator, AddDoctorRequestValidator>()
                .AddScoped<IClinicService, ClinicService>()
                .AddScoped<IAddClinicRequestValidator, AddClinicRequestValidator>()
                .AddScoped<IBookingService, BookingService.BookingService>()
                .AddScoped<IAddBookingRequestValidator, AddBookingRequestValidator>();
        }
    }
}
