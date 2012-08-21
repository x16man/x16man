using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Boxup.Entities
{
    [Serializable]
    public class ItemInfo
    {
        #region Property
        [Bindable(BindableSupport.Yes)]
        public string ItemName { get; set; }

        [Bindable(BindableSupport.Yes)]
        public string OldItemName { get; set; }

        [Bindable(BindableSupport.Yes)]
        public decimal d { get; set; }

        [Bindable(BindableSupport.Yes)]
        public int c { get; set; }
        #endregion
        public ItemInfo() { }

        
    }
}
