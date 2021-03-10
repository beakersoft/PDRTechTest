using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingService.Requests;
using PDR.PatientBooking.Service.BookingService.Validation;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{

    [TestFixture]
    public class AddBookingRequestValidatorFixture
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            _validator = new AddBookingRequestValidator(_context);
        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddMinutes(1);
            request.EndTime = DateTime.UtcNow.AddMinutes(15);
            SeedDummyDatabase(request);

            var res = _validator.ValidateRequest(request);
            
            //assert
            res.PassedValidation.Should().BeTrue();
        }

        [Test]
        public void ValidateRequest_EndBeforeStart_ReturnsFailedValidationResult()
        {
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddDays(1);
            request.EndTime = DateTime.UtcNow.AddDays(-7);

            SeedDummyDatabase(request);
            
            var res = _validator.ValidateRequest(request);
            
            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Count().Should().Be(1);
            res.Errors.First().Should().Be("Please make sure the booking end time is after the start time");
        }

        [Test]
        public void ValidateRequest_DatesAlreadyTaken_ReturnsFailedValidationResult()
        {
            var request = GetValidRequest();
            request.DoctorId = 9999;
            request.StartTime = DateTime.UtcNow;
            request.EndTime = DateTime.UtcNow.AddMinutes(15);

            SeedDummyDatabase(request);
            
            var updateOrder = _context.Order.First(x => x.DoctorId == request.DoctorId);
            updateOrder.StartTime = request.StartTime.AddMinutes(1);
            updateOrder.EndTime = request.EndTime.AddMinutes(1);

            _context.Order.Update(updateOrder);
            _context.SaveChanges();

            var res = _validator.ValidateRequest(request);

        }

        private void SeedDummyDatabase(BookingRequest request)
        {
            var patient = _fixture
                .Build<Patient>()
                .With(x =>  x.Id, request.PatientId)
                .Create();

            var doctor = _fixture
                .Build<Doctor>()
                .With(x => x.Id, request.DoctorId)
                .Create();

            var order = _fixture
                .Build<Order>()
                .With(x => x.DoctorId, request.DoctorId)
                .With(x => x.StartTime, request.StartTime.AddMinutes(1))
                .With(x => x.EndTime, request.EndTime.AddMinutes(1))
                .Create();

            _context.Add(order);
            _context.Add(patient);
            _context.Add(doctor);
            _context.SaveChanges();

        }



        private BookingRequest GetValidRequest()
        {
            var request = _fixture.Create<BookingRequest>();
            return request;
        }
    }
}
