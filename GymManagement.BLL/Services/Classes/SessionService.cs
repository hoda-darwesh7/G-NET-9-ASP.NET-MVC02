using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.Common;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be Greater Than Start Date");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialties>(category.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Trainer and Category Must Be The Same");

            var session = _mapper.Map<Session>(model);
            _unitOfWork .GetRepository<Session>().AddAsync(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var Sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(ct);
            if (Sessions == null || !Sessions.Any()) return null;

            var MappedSession = Sessions.Select(S => new SessionViewModel()
            {
                Id = S.Id,
                Capacity = S.Capacity,
                CategoryName = S.Category.CategoryName,
                TrainerName = S.Trainer.Name,
                StartDate = S.StartDate,
                EndDate = S.EndDate,
                Description = S.Description
            });

            foreach(var Session in MappedSession)
            {
                Session.AvailableSlots = Session.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(Session.Id, ct);
            }

            return MappedSession;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int Sessionid, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(Sessionid , ct);
            if (session is null) return Result<SessionViewModel>.NotFound("Session Not Found");
            else
            {
                var MappedSession = _mapper.Map<Session, SessionViewModel>(session);
                MappedSession.AvailableSlots = MappedSession.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(session.Id, ct);
                return Result<SessionViewModel>.Ok(MappedSession);
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int Sessionid, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(Sessionid, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found!");
            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Cannot Edit Completed Or Ongoing Sessions!");
            var BookingCount = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(Sessionid, ct);
            if (BookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cannot Edit Session Already Booked!");
            var MappedSession = _mapper.Map<Session ,  UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(MappedSession);
        }

        public async Task<Result> UpdateSessionAsync(int Sessionid, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.SessionRepository.GetByIdAsync(Sessionid, ct);
            if (Session is null) return Result.NotFound("Session Not Found!");
            if (Session.StartDate <= DateTime.Now) return Result.Fail("Cannot Edit Session That Already Started!");
            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be After Start Date");

            var BookedCount = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(Sessionid, ct);
            if(BookedCount > 0 ) return Result.Fail("Cannot Edit Session Already Booked!");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Most Be in the Future");

            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (Trainer is null) return Result.Fail("Trainer Not Found");

            var Category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(Session.CategoryId);

            var IsValid = Enum.TryParse<Specialties>(Category?.CategoryName, true, out var CategorySpecilty);
            if (!IsValid || Trainer.Specialty != CategorySpecilty)
                return Result.Validation("Trainer and Category Not Matching");

            _mapper.Map(model , Session);
            Session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.UpdateAsync(Session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Session");

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<Result> DeleteSessionAsync(int Sessionid, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(Sessionid ,ct);
            if (session == null) return Result.NotFound("Session is Not Found!");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can Not Delete An Ongoing Or Upcomming Session ! ");

            var hasBooking = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(Sessionid, ct);
            if(hasBooking > 0 ) return Result.Fail("Can Not Delete Session That Has Booking");

            _unitOfWork.SessionRepository.DeleteAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session");

        } 
    }
}
