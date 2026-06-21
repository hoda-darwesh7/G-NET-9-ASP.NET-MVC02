using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.VeiwModels.Common
{
    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
