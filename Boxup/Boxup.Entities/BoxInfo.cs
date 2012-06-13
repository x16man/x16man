using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Boxup.Entities
{
    [Serializable]
    public class BoxInfo
    {
        #region Property
        [Bindable(BindableSupport.Yes)]
        public long Id { get; set; }
        [Bindable(BindableSupport.Yes)]
        public DateTime BoxDate { get; set; }
        [Bindable(BindableSupport.Yes)]
        public int BoxNo { get; set; }
        [Bindable(BindableSupport.Yes)]
        public string BoxName { get; set; }
        [Bindable(BindableSupport.Yes)]
        public string BoxModel { get; set; }
        [Bindable(BindableSupport.Yes)]
        public string BoxSpec { get; set; }
        #endregion

        public BoxInfo() { }
    }
}
