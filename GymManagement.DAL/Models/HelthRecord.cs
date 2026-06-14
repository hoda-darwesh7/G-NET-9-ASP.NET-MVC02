using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class HelthRecord :BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodType { get; set; } = default!;
        public string? Note { get; set; }
        #region Relations

        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        #endregion
    }
}
