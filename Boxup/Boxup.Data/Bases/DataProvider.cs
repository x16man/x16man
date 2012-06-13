using System;

namespace Boxup.Data.Bases
{
    /// <summary>
    /// Boxup数据库依据配置文件以抽象工厂模式来创建数据访问层.
    /// </summary>
    public abstract class DataProvider : Shmzh.Components.SystemComponent.Provider
    {
        ///<summary>
        /// Current PathProvider instance.
        ///</summary>
        public virtual PathProvider PathProvider { get { throw new NotImplementedException(); } }

        ///<summary>
        /// Current ItemProvider instance.
        ///</summary>
        public virtual ItemProvider ItemProvider { get { throw new NotImplementedException(); } }

        ///<summary>
        /// Current BoxProvider instance.
        ///</summary>
        public virtual BoxProvider BoxProvider { get { throw new NotImplementedException(); } }

        ///<summary>
        /// Current BoxContractProvider instance.
        ///</summary>
        public virtual BoxContractProvider BoxContractProvider { get { throw new NotImplementedException(); } }

        ///<summary>
        /// Current BoxItemProvider instance.
        ///</summary>
        public virtual BoxItemProvider BoxItemProvider { get { throw new NotImplementedException(); } }
    }
}
