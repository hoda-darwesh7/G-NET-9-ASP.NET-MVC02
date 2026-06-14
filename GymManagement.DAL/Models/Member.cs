using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        #region Relations

        public HelthRecord HelthRecord { get; set; } = default!;
        public ICollection<MemberShip> Plans { get; set; }
        public ICollection<Booking>  MemberSession { get; set; }

        #endregion
    }
}
