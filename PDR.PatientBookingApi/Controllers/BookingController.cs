using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingService;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PDR.PatientBooking.Service.BookingService.Requests;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(PatientBookingContext context, IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointment(long identificationNumber)
        {
            if (!ModelState.IsValid || identificationNumber <=0)
                return BadRequest($"Please pass a valid booking id");

            var booking = _bookingService.GetPatientNextAppointment(identificationNumber);

            if (booking == null)
                return NotFound("No bookings where found");

            return Ok(new
            {
                booking.Id,
                booking.DoctorId,
                booking.StartTime,
                booking.EndTime
            });
        }

        [HttpPost()]
        public async Task<IActionResult> AddBooking(BookingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Invalid request object - {GetModelStateMessages()}");

            try
            {
                await _bookingService.AddBookingAsync(request);
                return Ok();
            }
            catch (ArgumentException argumentEx)
            {
                return BadRequest(argumentEx.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry, an internal error occurred");
            }
        }

        [HttpPut("patient/Cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Please pass a valid booking id");

            try
            {
                await _bookingService.CancelBookingAsync(bookingId);
                return Ok("Booking Canceled");
            }
            catch (ArgumentException argumentEx)
            {
                return BadRequest(argumentEx.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry, an internal error occurred");
            }
        }
        
        [NonAction]
        public string GetModelStateMessages()
        {
            return string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}