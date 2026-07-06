using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.BookingViewModels;
using GymManagement.BLL.VeiwModels.Common;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{

    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId , ct);
            if (sessions is null) return Result.NotFound("Session Not Found");
            if (sessions.StartDate <= DateTime.Now) return Result.Validation("You Can Not Book Ongoing Session");

            var activeMembership = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now , ct);
            if (!activeMembership) return Result.Validation("This Member Do Not Have Activ Membership");

            var bookedSession = await _unitOfWork.BookingRepository.AnyAsync(m => m.MemberId == model.MemberId && m.SessionId == model.SessionId, ct);
            if (bookedSession) return Result.Validation("This Member Has Already Booked This Session");

            var availableSlots = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(model.SessionId , ct);
            if (availableSlots >= sessions.Capacity) return Result.Validation("Session Has Full Capacity");

            var booking = new Booking
            {
                SessionId = model.SessionId,
                MemberId = model.MemberId,
                IsAttened = false
            };

            _unitOfWork.BookingRepository.AddAsync(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Member");


        }

        public async Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(ct);
            var activeSessions = sessions.Where(s => s.EndDate > DateTime.Now).ToList();

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(activeSessions);

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(session.Id , ct);
            }

            return Result<IEnumerable<SessionViewModel>>.Ok(mappedSessions);
        }

        public async Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMemberForDropDown(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(ct: ct);
            var bookedMemberIds = bookings
                .Where(b => b.SessionId == sessionId)
                .Select(b => b.MemberId)
                .ToList(); // Materialize as a list for memory efficiency

            // 2. Get all members
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            // 3. CORRECTED: Filter the members (the objects), not the booleans
            var availableMembers = members
                .Where(m => !bookedMemberIds.Contains(m.Id))
                .ToList();

            // 4. Now map the list of Member objects to the ViewModel
            var mappedMembers = _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);

            return Result<IEnumerable<MemberSelectListViewModel>>.Ok(mappedMembers);

        }

        public async Task<Result<IEnumerable<MemberForSessionViewModel>>> GetMemberForSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var memberBooking = await _unitOfWork.BookingRepository.GetBookingBySessionIdAsync(sessionId, ct);

            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            var bookingMapp = memberBooking.Select(bm => new MemberForSessionViewModel
            {
                MemberId = bm.MemberId,
                MemberName = bm.Member.Name,
                BookingDate = bm.CreatedAt,
                SessionId = bm.SessionId,
                IsAttended = session ? .StartDate > DateTime.Now ? false : bm.IsAttened
            }).ToList();

            return Result<IEnumerable<MemberForSessionViewModel>>.Ok(bookingMapp);
        }

        public async Task<Result> IsAttendedAsync(int sessionId, int memberId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId , true , ct);
            if (booking == null) return Result.NotFound("Booking Not Found");

            booking.UpdatedAt = DateTime.Now;
            booking.IsAttened = true;

            _unitOfWork.BookingRepository.UpdateAsync(booking);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed To Update To Attended") ;

        }

        public async Task<Result> CancelBookingAsync(int sessionId, int memberId, CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId , ct);
            if (sessions is null) return Result.NotFound("Session Not Found");
            if (sessions.StartDate <=  DateTime.Now)return Result.Validation("Can Not Cancel Ongoing Or Completed Session");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, true, ct);
            if (booking is null) return Result.NotFound("Booking Not Found");

            _unitOfWork.BookingRepository.DeleteAsync(booking);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed To Cancel Booking");

        }

    }
}
