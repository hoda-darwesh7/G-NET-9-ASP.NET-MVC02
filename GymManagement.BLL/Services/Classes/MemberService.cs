using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL;
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
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<MemberShip> _memberShipRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<HelthRecord> _healthRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public MemberService(IGenericRepository<Member> memberRepo ,
                             IGenericRepository<MemberShip> membershipRepo , 
                             IGenericRepository<Plan> planRepo , 
                             IGenericRepository<HelthRecord> HealthRepo , 
                             IGenericRepository<Booking> BookingRepo)
        {
            _memberRepo = memberRepo;
            _memberShipRepo = membershipRepo;
            _planRepo = planRepo;
            _healthRepo = HealthRepo;
            _bookingRepo = BookingRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var email = await _memberRepo.AnyAsync(x => x.Email == model.Email);
            var phone = await _memberRepo.AnyAsync(x => x.Phone == model.Phone);
            if (email || phone) return false;
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City,
                },
                HelthRecord = new HelthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note
                }

            };

            var Result = await _memberRepo.AddAsync(member);

            return Result > 0;

        }

        public async Task<bool> DeleteMemberAsync(int Id, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(Id,ct);
            if (member == null) return false;

            var ActiveBooking = await _bookingRepo.AnyAsync(B => B.MemberId == Id && B.Session.StartDate > DateTime.Now);
            if(ActiveBooking) return false;

            var Result = await _memberRepo.DeleteAsync(member);
            return Result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _memberRepo.GetAllAsync(ct:ct);
            if (!members.Any())
                return [];
            
            List<MemberViewModel> result = new List<MemberViewModel>();

            foreach (var member in members)
            {
                var memberViewModel = new MemberViewModel()
                {
                    Name = member.Name,
                    Phone = member.Phone,
                    Photo = member.Photo,
                    Email = member.Email,
                    Id = member.Id,
                    Gender = member.Gender.ToString()

                };
                result.Add(memberViewModel);
            }
            return result;
        }

        public async Task<HealthRecordViewModel> GetHealthRecordDetailsAsync(int HealthId, CancellationToken ct = default)
        {
            var HelthRecord = await _healthRepo.FirstOrDefaultAsync(X => X.Id == HealthId ,ct:ct);

            if (HelthRecord is null) return null;
            else
                return new HealthRecordViewModel()
                {
                    Weight = HelthRecord.Weight,
                    Height = HelthRecord.Height,
                    BloodType = HelthRecord.BloodType,
                    Note = HelthRecord.Note,
                };

        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(memberId , ct);
            if (member == null) return null;

            var Model = new MemberViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                Photo = member.Photo,
                Email = member.Email,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"
            };

            var activeMemberShip = await _memberShipRepo.FirstOrDefaultAsync(X => X.Id == memberId && X.EndDate > DateTime.Now); 
            
            if (activeMemberShip is not null)
            {
                var activePlan = await _planRepo.GetByIdAsync(activeMemberShip.PlanId , ct);

                Model.PlanName = activePlan.Name; 
                Model.MembershipStartDate = activeMemberShip.CreatedAt.ToString();
                Model.MembershipEndDate = activeMemberShip.EndDate.ToString();
                
            }
            return Model;
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateasync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync (memberId , ct);

            if (member == null) return null;
            else
                return new MemberToUpdateViewModel()
                {

                    Name = member.Name,
                    Phone = member.Phone,
                    Photo = member.Photo,
                    Email = member.Email,
                    City = member.Address.City,
                    Street = member.Address.Street,
                    BuildingNumber = member.Address.BuildingNumber,
                };


        }

        public async Task<bool> UpdateMemberAsync(int Id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(Id , ct);

            var EmailExist = await _memberRepo.AnyAsync(X => X.Email == model.Email && X.Id != Id);
            var PhoneExist = await _memberRepo.AnyAsync(X => X.Phone == model.Phone && X.Id != Id);
            if (EmailExist || PhoneExist)return false;
            
            member.Phone = model.Phone;
            member.Photo = model.Photo;
            member.Email = model.Email;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            var Result = await _memberRepo.UpdateAsync(member);
            return Result > 0;

        }
    }
}
