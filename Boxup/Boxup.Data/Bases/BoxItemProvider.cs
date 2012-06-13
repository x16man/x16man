using System;
using System.Collections.Generic;
using System.Text;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.Bases
{
    public abstract class BoxItemProvider:IDAL.IBoxItem
    {
        #region IBoxItem 成员

        public abstract long Insert(Boxup.Entities.BoxItemInfo obj, TransactionManager trans);
        
        public bool Delete(Boxup.Entities.BoxItemInfo obj)
        {
            return this.Delete(obj.Id,null);
        }
        public bool Delete(Boxup.Entities.BoxItemInfo obj, TransactionManager trans)
        {
            return this.Delete(obj.Id, trans);
        }
        public bool Delete(long id)
        {
            return this.Delete(id, null);
        }
        public abstract bool Delete(long id, TransactionManager trans);
        
        public abstract IList<Boxup.Entities.BoxItemInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);
        
        #endregion
    }
}
