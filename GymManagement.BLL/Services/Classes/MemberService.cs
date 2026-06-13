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

        public MemberService(IGenericRepository<Member> memberRepo ,
                             IGenericRepository<MemberShip> membershipRepo , 
                             IGenericRepository<Plan> planRepo , 
                             IGenericRepository<HelthRecord> HealthRepo)
        {
            _memberRepo = memberRepo;
            _memberShipRepo = membershipRepo;
            _planRepo = planRepo;
            _healthRepo = HealthRepo;
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
    }
}
