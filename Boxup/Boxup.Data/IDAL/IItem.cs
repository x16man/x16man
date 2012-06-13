using System;
using System.Collections.Generic;
using System.Text;
using Boxup.Entities;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.IDAL
{
    public interface IItem
    {
        bool Insert(ItemInfo obj, TransactionManager trans);

        bool Update(ItemInfo obj, TransactionManager trans);

        bool Delete(ItemInfo obj);

        bool Delete(string  itemName, TransactionManager trans);

        IList<ItemInfo> GetAll(TransactionManager trans);

    }
}
