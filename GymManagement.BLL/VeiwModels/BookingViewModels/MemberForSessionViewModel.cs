using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.VeiwModels.BookingViewModels
{
    public class MemberForSessionViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public DateTime BookingDate { get; set; }
        public int SessionId { get; set; }
        public bool IsAttended { get; set; } = false;
    }
}
