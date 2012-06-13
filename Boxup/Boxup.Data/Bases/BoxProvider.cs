using System;
using System.Collections.Generic;
using System.Text;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.Bases
{
    public abstract class BoxProvider:IDAL.IBox
    {
        #region IBox 成员

        public abstract long Insert(Boxup.Entities.BoxInfo obj, TransactionManager trans);
        
        
        public bool Delete(Boxup.Entities.BoxInfo obj)
        {
            return this.Delete(obj.Id);
        }
        public bool Delete(Boxup.Entities.BoxInfo obj, TransactionManager trans)
        {
            return this.Delete(obj.Id, trans);
        }
        public bool Delete(long id)
        {
            return this.Delete(id, null);
        }
        public abstract bool Delete(long id, TransactionManager trans);
        
        public abstract IList<Boxup.Entities.BoxInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);
        
        public abstract Boxup.Entities.BoxInfo GetById(long id);
        
        public abstract Boxup.Entities.BoxInfo GetByBoxNo(int boxNo);
        
        #endregion
    }
}
