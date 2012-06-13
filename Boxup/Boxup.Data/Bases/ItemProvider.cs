using System;
using System.Collections.Generic;
using System.Text;
using Shmzh.Components.SystemComponent;
namespace Boxup.Data.Bases
{
    public abstract class ItemProvider :IDAL.IItem
    {
        #region IItem 成员

        public abstract bool Insert(Boxup.Entities.ItemInfo obj, TransactionManager trans);
        
        public abstract bool Update(Boxup.Entities.ItemInfo obj, TransactionManager trans);
        
        public bool Delete(Boxup.Entities.ItemInfo obj)
        {
            return this.Delete(obj.ItemName,null);
        }

        public abstract bool Delete(string itemName, TransactionManager trans);
        
        public abstract IList<Boxup.Entities.ItemInfo> GetAll(TransactionManager trans);
        
        #endregion
    }
}
