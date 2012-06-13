using System;
using System.Collections.Generic;
using System.Text;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.Bases
{
    public abstract class BoxContractProvider:IDAL.IBoxContract
    {
        #region IBoxContract 成员

        public abstract long Insert(Boxup.Entities.BoxContractInfo obj,TransactionManager trans);
        

        public bool Delete(Boxup.Entities.BoxContractInfo obj)
        {
            return this.Delete(obj,null);
        }
        public bool Delete(Boxup.Entities.BoxContractInfo obj, TransactionManager trans)
        {
            return this.Delete(obj.Id, trans);
        }
        public bool Delete(long id)
        {
            return this.Delete(id, null);
        }
        public abstract bool Delete(long id, TransactionManager trans);
        
        public abstract IList<Boxup.Entities.BoxContractInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);
        
        #endregion
    }
}
