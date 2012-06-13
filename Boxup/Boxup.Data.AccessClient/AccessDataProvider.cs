using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text;
using Shmzh.Components.SystemComponent;
using Boxup.Data.Bases;
using Boxup.Entities;

namespace Boxup.Data.AccessClient
{
    public sealed partial class AccessDataProvider
    {
        #region Field
        private AccessPathProvider innerAccessPathProvider;
        private AccessItemProvider innerAccessItemProvider;
        private AccessBoxProvider innerAccessBoxProvider;
        private AccessBoxItemProvider innerAccessBoxItemProvider;
        private AccessBoxContractProvider innerAccessBoxContractProvider;
        #endregion

        #region Property

        ///<summary>
        /// Path对象的数据访问对象。
        ///</summary>
        /// <value></value>
        public override PathProvider PathProvider
        {
            get
            {
                if (innerAccessPathProvider == null)
                {
                    lock (syncRoot)
                    {
                        if (innerAccessPathProvider == null)
                        {
                            this.innerAccessPathProvider = new AccessPathProvider(_connectionString, _providerInvariantName);
                        }
                    }
                }
                return innerAccessPathProvider;
            }
        }
        /// <summary>
        /// Item对象的数据访问对象。
        /// </summary>
        public override ItemProvider ItemProvider
        {
            get
            {
                if (innerAccessItemProvider == null)
                {
                    lock (syncRoot)
                    {
                        if (innerAccessItemProvider == null)
                        {
                            this.innerAccessItemProvider = new AccessItemProvider(_connectionString, _providerInvariantName);
                        }
                    }
                }
                return innerAccessItemProvider;
            }
        }
        /// <summary>
        /// Box对象的数据访问对象。
        /// </summary>
        public override BoxProvider BoxProvider
        {
            get
            {
                if (innerAccessBoxProvider == null)
                {
                    lock (syncRoot)
                    {
                        if (innerAccessBoxProvider == null)
                        {
                            this.innerAccessBoxProvider = new AccessBoxProvider(_connectionString, _providerInvariantName);
                        }
                    }
                }
                return innerAccessBoxProvider;
            }
        }
        /// <summary>
        /// BoxContract对象的数据访问对象。
        /// </summary>
        public override BoxContractProvider BoxContractProvider
        {
            get
            {
                if (innerAccessBoxContractProvider == null)
                {
                    lock (syncRoot)
                    {
                        if (innerAccessBoxContractProvider == null)
                        {
                            this.innerAccessBoxContractProvider = new AccessBoxContractProvider(_connectionString, _providerInvariantName);
                        }
                    }
                }
                return innerAccessBoxContractProvider;
            }
        }
        /// <summary>
        /// BoxItem对象的数据访问对象。
        /// </summary>
        public override BoxItemProvider BoxItemProvider
        {
            get
            {
                if (innerAccessBoxItemProvider == null)
                {
                    lock (syncRoot)
                    {
                        if (innerAccessBoxItemProvider == null)
                        {
                            this.innerAccessBoxItemProvider = new AccessBoxItemProvider(_connectionString, _providerInvariantName);
                        }
                    }
                }
                return innerAccessBoxItemProvider;
            }
        }
        #endregion
    }
}
