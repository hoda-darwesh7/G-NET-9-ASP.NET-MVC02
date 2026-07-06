using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.Common;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using GymManagement.DAL;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MembershipsViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.MembershipRepository.GetMembershipsWithMemberAndPlanAsync(x=>x.EndDate > DateTime.Now , ct);
            return _mapper.Map<IEnumerable<MembershipsViewModel>>(memberships); 
        }

        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var memberExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Id == model.MemberId , ct);
            if (!memberExists) return Result.NotFound("Member Not Found");

            var planExists = await _unitOfWork.GetRepository<Plan>().AnyAsync(x => x.Id == model.PlanId, ct);
            if (!planExists) return Result.NotFound("Plan Not Found");

            var activeMempership = await _unitOfWork.MembershipRepository.AnyAsync(x=>x.MemberId == model.MemberId && x.EndDate > DateTime.Now , ct);
            if (activeMempership) return Result.Fail("This Member Has An Active Membership");

            var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId , ct);
            if (!activePlan.IsActive) return Result.Fail("Plan Is Not Active");

            var membership = _mapper.Map<MemberShip>(model);
            membership.EndDate = (model.StartDate ?? DateTime.Now).AddDays(activePlan.DurationDays);

            _unitOfWork.MembershipRepository.AddAsync(membership);

            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create MemberShip!");

        }

        public async Task<Result> DeleteActiveMembership(int memberId, CancellationToken ct = default)
        {
            var activeMembership = await _unitOfWork.MembershipRepository.FirstOrDefaultAsync(x => x.MemberId == memberId && x.EndDate > DateTime.Now);
            if (activeMembership is null) return Result.Fail("Can Not Delete Member That Has Active Membrship");

            _unitOfWork.MembershipRepository.DeleteAsync(activeMembership);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Membership");
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDown(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDown(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(plans);
        }
    }
}
