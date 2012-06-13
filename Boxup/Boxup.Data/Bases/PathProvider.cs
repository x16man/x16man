using System;
using System.Collections.Generic;
using System.Text;

namespace Boxup.Data.Bases
{
    public abstract class PathProvider :IDAL.IPath
    {

        #region IPath 成员

        public abstract bool Insert(Boxup.Entities.PathInfo obj);
        
        public abstract bool Delete();

        public abstract Boxup.Entities.PathInfo Get();
        
        #endregion
    }
}
