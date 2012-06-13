using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Boxup.Entities
{
    [Serializable]
    public class BoxContractInfo
    {
        #region Property
        [Bindable(BindableSupport.Yes)]
        public long Id { get; set; }
        [Bindable(BindableSupport.Yes)]
        public DateTime BoxDate { get; set; }
        [Bindable(BindableSupport.Yes)]
        public int BoxNo { get; set; }
        [Bindable(BindableSupport.Yes)]
        public string ContractNo { get; set; }
        #endregion

        public BoxContractInfo() { }
    }
}
