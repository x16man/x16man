using System;
using System.Collections.Generic;
using System.Text;
using Boxup.Entities;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.IDAL
{
    public interface IBox
    {
        long Insert(BoxInfo obj,TransactionManager trans);
        bool Delete(BoxInfo obj);
        bool Delete(long id, TransactionManager trans);
        IList<BoxInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);

        BoxInfo GetById(long id);

        BoxInfo GetByBoxNo(int boxNo);
    }
}
