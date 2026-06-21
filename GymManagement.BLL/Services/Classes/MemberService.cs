using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper , IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var email = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email);
            var phone = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone);
            if (email || phone) return false;

            var StoredPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream() ,model.PhotoFile.FileName ,"MembersPhoto");
            if (string.IsNullOrWhiteSpace(StoredPhotoName)) return false;

            var member = _mapper.Map<Member>(model);
            member.Photo = StoredPhotoName;

            _unitOfWork.GetRepository<Member>().AddAsync(member);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            if (Result > 0)
                return true;
            else
            {

                return false;
            }
        }

        public async Task<bool> DeleteMemberAsync(int Id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id,ct);
            if (member == null) return false;

            var ActiveBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(B => B.MemberId == Id && B.Session.StartDate > DateTime.Now);
            if(ActiveBooking) return false;

            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
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
            var HelthRecord = await _unitOfWork.GetRepository<HelthRecord>().FirstOrDefaultAsync(X => X.Id == HealthId ,ct:ct);

            if (HelthRecord is null) return null;
            else
                return _mapper.Map<HealthRecordViewModel>(HelthRecord);

        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId , ct);
            if (member == null) return null;

            var Model = _mapper.Map<Member ,MemberViewModel>(member);

            var activeMemberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(X => X.Id == memberId && X.EndDate > DateTime.Now); 
            
            if (activeMemberShip is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMemberShip.PlanId , ct);

                Model.PlanName = activePlan.Name; 
                Model.MembershipStartDate = activeMemberShip.CreatedAt.ToString();
                Model.MembershipEndDate = activeMemberShip.EndDate.ToString();
                
            }
            return Model;
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateasync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync (memberId , ct);

            if (member == null) return null;
            else
                return _mapper.Map<MemberToUpdateViewModel>(member);

        }

        public async Task<bool> UpdateMemberAsync(int Id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id , ct);

            var EmailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X => X.Email == model.Email && X.Id != Id);
            var PhoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X => X.Phone == model.Phone && X.Id != Id);
            if (EmailExist || PhoneExist)return false;
            
            _mapper.Map<Member>(model); 
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;

        }
    }
}
