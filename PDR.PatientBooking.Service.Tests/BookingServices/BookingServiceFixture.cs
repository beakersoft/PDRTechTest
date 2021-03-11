using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingService.Requests;
using PDR.PatientBooking.Service.BookingService.Validation;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    [TestFixture]
    public class BookingServiceFixture
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;
        private PatientBookingContext _context;
        private Mock<IAddBookingRequestValidator> _validator;
        private BookingService.BookingService _service;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            //Prevent fixture from generating circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _validator = _mockRepository.Create<IAddBookingRequestValidator>();

            // Mock default
            SetupMockDefaults();

            _service = new BookingService.BookingService(
                _context,
                _validator.Object
            );
        }

        private void SetupMockDefaults()
        {
            _validator.Setup(x => x.ValidateRequest(It.IsAny<BookingRequest>()))
                .Returns(new PdrValidationResult(true));
        }

        [Test]
        public async Task AddBookingAsync_ValidatesRequest()
        {
            var request = _fixture.Create<BookingRequest>();
            var doctor = _fixture.Create<Doctor>();
            doctor.Id = request.DoctorId;
            var patient = _fixture.Create<Patient>();
            patient.Id = request.PatientId;
            _context.Doctor.Add(doctor);
            _context.Patient.Add(patient);
            _context.SaveChanges();

            await _service.AddBookingAsync(request);

            _validator.Verify(x => x.ValidateRequest(request), Times.Once);
        }

        [Test]
        public void AddBookingAsync_ValidatorFails_ThrowsArgumentException()
        {
            var failedValidationResult = new PdrValidationResult(false, _fixture.Create<string>());

            _validator.Setup(x => x.ValidateRequest(It.IsAny<BookingRequest>())).Returns(failedValidationResult);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddBookingAsync(_fixture.Create<BookingRequest>()));

            exception.Message.Should().Be(failedValidationResult.Errors.First());
        }

        [Test]
        public async Task AddBookingAsync_AddsBookingToContext()
        {
            var request = _fixture.Create<BookingRequest>();
            var doctor = _fixture.Create<Doctor>();
            doctor.Id = request.DoctorId;
            var patient = _fixture.Create<Patient>();
            patient.Id = request.PatientId;
            _context.Doctor.Add(doctor);
            _context.Patient.Add(patient);
            _context.SaveChanges();

            var expected = new Order
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId
            };

            await _service.AddBookingAsync(request);

            var order = await _context.Order.FirstOrDefaultAsync(x=>x.PatientId == request.PatientId
                            && x.DoctorId == request.DoctorId && x.StartTime == request.StartTime && x.EndTime == request.EndTime);

            order.Should().NotBeNull();
        }
    }
}
