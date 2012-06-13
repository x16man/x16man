using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Boxup.Entities
{
    [Serializable]
    public class PathInfo
    {
        #region Property
        [Bindable(BindableSupport.Yes)]
        public string LastPath { get; set; }
        #endregion
        public PathInfo() { }
    }
}
