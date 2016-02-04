using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGX.Lean.DbEntity;

namespace WGX.Lean.IBiz
{
    public interface ICreateModel : IBaseBiz<CreateModel>
    {
        CreateModel GetFirst();

        bool CheckCreateModel(CreateModel model);
    }
}
