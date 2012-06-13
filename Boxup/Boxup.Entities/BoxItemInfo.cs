using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Boxup.Entities
{
    [Serializable]
    public class BoxItemInfo
    {
        #region Property
        [Bindable(BindableSupport.Yes)]
        public long Id { get; set; }
        [Bindable(BindableSupport.Yes)]
        public DateTime BoxDate{get;set;}
        [Bindable(BindableSupport.Yes)]
        public int BoxNo { get; set; }
        [Bindable(BindableSupport.Yes)]
        public string ItemName{get;set;}
        [Bindable(BindableSupport.Yes)]
        public string ItemModel{get;set;}
        [Bindable(BindableSupport.Yes)]
        public int ItemNum{get;set;}
        #endregion

        public BoxItemInfo() { }
    }
}
