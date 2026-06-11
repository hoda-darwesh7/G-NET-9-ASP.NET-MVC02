using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
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

        public MemberService(IGenericRepository<Member> memberrepo)
        {
            _memberRepo = memberrepo;
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
    }
}
