using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var email = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email);
            var phone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone);
            if (email || phone) return false;
            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Specialty = model.Specialties,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City,
                }
            };

            _unitOfWork.GetRepository<Trainer>().AddAsync(trainer);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return false;

            var FutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(B => B.TrainerId == id && B.StartDate > DateTime.Now);
            if (FutureSessions) return false;

            _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainer);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!trainers.Any())
                return [];

            List<TrainerViewModel> result = new List<TrainerViewModel>();

            foreach (var trainer in trainers)
            {
                var trainerViewModel = new TrainerViewModel()
                {
                    Name = trainer.Name,
                    Phone = trainer.Phone,
                    Email = trainer.Email,
                    Id = trainer.Id,
                    Gender = trainer.Gender.ToString(),
                    Specialties = trainer.Specialty.ToString()
                };
                result.Add(trainerViewModel);
            }
            return result;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid , ct);
            if (trainer == null) return null;

            var model = new TrainerViewModel()
            {
                Phone = trainer.Phone,
                Specialties = trainer.Specialty.ToString(),
                Email = trainer.Email,
                Name = trainer.Name,
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };

            return model;
        }

        public async Task<TrainerToUpdateViewModel> GetTrainerToUpdate(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid , ct);
            if (trainer == null) return null;
            else
                return new TrainerToUpdateViewModel()
                {
                    Name = trainer.Name,
                    Phone = trainer.Phone,
                    Email = trainer.Email,
                    BuildingNumber = trainer.Address.BuildingNumber,
                    City = trainer.Address.City,
                    Street = trainer.Address.Street,
                    Specialties = trainer.Specialty

                };

        }

        public async Task<bool> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);

            var EmailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Email == model.Email && X.Id != id);
            var PhoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Phone == model.Phone && X.Id != id);
            if (EmailExist || PhoneExist) return false;
            else
            {
                trainer.Phone = model.Phone;
                trainer.Email = model.Email;
                trainer.Specialty = model.Specialties;
                trainer.Address.City = model.City;
                trainer.Address.Street = model.Street;
                trainer.Address.BuildingNumber = model.BuildingNumber;
                trainer.UpdatedAt = DateTime.Now;
            }
            _unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;
        }
    }
}
