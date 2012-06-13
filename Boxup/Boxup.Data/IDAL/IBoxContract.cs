using System;
using System.Collections.Generic;
using System.Text;
using Boxup.Entities;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.IDAL
{
    public interface IBoxContract
    {
        long Insert(BoxContractInfo obj,TransactionManager trans);
        //bool Update(BoxContractInfo obj);
        bool Delete(BoxContractInfo obj);
        bool Delete(long id);

        IList<BoxContractInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans);
    }
}
