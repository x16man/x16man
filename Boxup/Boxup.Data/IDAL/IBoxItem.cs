using System;
using System.Collections.Generic;
using System.Text;
using Boxup.Entities;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.IDAL
{
    public interface IBoxItem
    {
        long Insert(BoxItemInfo obj, TransactionManager trans);
        //bool Update(BoxItemInfo obj);
        bool Delete(BoxItemInfo obj);
        bool Delete(long id);

        IList<BoxItemInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);
    }
}
