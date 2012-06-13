using System;
using System.Collections.Generic;
using System.Text;
using Boxup.Entities;

namespace Boxup.Data.IDAL
{
    public interface IPath
    {
        bool Insert(PathInfo obj);

        bool Delete();

        PathInfo Get();
    }
}
