using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Configuration;
using Boxup.Data.Bases;
using Shmzh.Components.SystemComponent;
using Boxup.Entities;

namespace Boxup.Data
{
    public sealed class DataRepository
    {
        #region Field
        private static object SyncRoot = new object();
        private static DataProvider _provider = null;
        private static Shmzh.Components.SystemComponent.ProviderCollection _providers = null;
        private static volatile Configuration _config = null;
        private static volatile Section _section = null;

        #endregion

        #region CTOR
        private DataRepository()
        {
        }
        #endregion

        #region LoadProvider
        /// <summary>
        /// Enables the DataRepository to programatically create and 
        /// pass in a <c>NetTiersProvider</c> during runtime.
        /// </summary>
        /// <param name="provider">An instatiated NetTiersProvider.</param>
        public static void LoadProvider(Shmzh.Components.SystemComponent.Provider provider)
        {
            LoadProvider(provider, false);
        }

        /// <summary>
        /// Enables the DataRepository to programatically create and 
        /// pass in a <c>NetTiersProvider</c> during runtime.
        /// </summary>
        /// <param name="provider">An instatiated NetTiersProvider.</param>
        /// <param name="setAsDefault">ability to set any valid provider as the default provider for the DataRepository.</param>
        public static void LoadProvider(Shmzh.Components.SystemComponent.Provider provider, bool setAsDefault)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (_providers == null)
            {
                lock (SyncRoot)
                {
                    if (_providers == null)
                        _providers = new Shmzh.Components.SystemComponent.ProviderCollection();
                }
            }

            if (_providers[provider.Name] == null)
            {
                lock (_providers.SyncRoot)
                {
                    _providers.Add(provider);
                }
            }

            if (_provider == null || setAsDefault)
            {
                lock (SyncRoot)
                {
                    if (_provider == null || setAsDefault)
                        _provider = provider as DataProvider;
                }
            }
        }

        ///<summary>
        /// Configuration based provider loading, will load the providers on first call.
        ///</summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (SyncRoot)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Load registered providers and point _provider to the default provider
                        _providers = new Shmzh.Components.SystemComponent.ProviderCollection();
                        var thisSection = Section();
                        ProvidersHelper.InstantiateProviders(thisSection.Providers, _providers, typeof(Shmzh.Components.SystemComponent.Provider));
                        _provider = _providers[thisSection.DefaultProvider] as DataProvider;

                        if (_provider == null)
                        {
                            throw new ProviderException("Unable to load default NetTiersProvider");
                        }
                    }
                }
            }
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Gets a reference to the configured NetTiersServiceSection object.
        /// </summary>
        public static Shmzh.Components.SystemComponent.Section Section()
        {
            if (_section == null)
            {
                // otherwise look for section based on the assembly name
                _section = WebConfigurationManager.GetSection("Boxup.Data") as Section;
            }

            #region Design-Time Support

            if (_section == null)
            {
                // lastly, try to find the specific NetTiersServiceSection for this assembly
                foreach (ConfigurationSection temp in Configuration.Sections)
                {
                    if (temp is Section)
                    {
                        _section = temp as Section;
                        break;
                    }
                }
            }

            #endregion Design-Time Support

            if (_section == null)
            {
                throw new ProviderException("Unable to load NetTiersServiceSection");
            }

            return _section;

        }

        /// <summary>
        /// Gets a reference to the application configuration object.
        /// </summary>
        public static Configuration Configuration
        {
            get
            {
                if (_config == null)
                {
                    // load specific config file
                    if (HttpContext.Current != null)
                    {
                        _config = WebConfigurationManager.OpenWebConfiguration("~");
                    }
                    else
                    {
                        _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    }
                }

                return _config;
            }
        }
        #endregion

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public static DataProvider Provider
        {
            get { LoadProviders(); return _provider; }
        }

        /// <summary>
        /// Gets the provider collection.
        /// </summary>
        /// <value>The providers.</value>
        public static Shmzh.Components.SystemComponent.ProviderCollection Providers
        {
            get { LoadProviders(); return _providers; }
        }

        /// <summary>
        /// Creates a new <see cref="TransactionManager"/> instance from the current datasource.
        /// </summary>
        /// <returns></returns>
        public TransactionManager CreateTransaction()
        {
            return _provider.CreateTransaction();
        }

        #region Connections

        /// <summary>
        /// Gets a reference to the ConnectionStringSettings collection.
        /// </summary>
        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                // use default ConnectionStrings if _section has already been discovered
                if (_config == null && _section != null)
                {
                    return WebConfigurationManager.ConnectionStrings;
                }

                return Configuration.ConnectionStrings.ConnectionStrings;
            }
        }

        // dictionary of connection providers
        private static Dictionary<String, ConnectionProvider> _connections;

        /// <summary>
        /// Gets the dictionary of connection providers.
        /// </summary>
        public static Dictionary<String, ConnectionProvider> Connections
        {
            get
            {
                if (_connections == null)
                {
                    lock (SyncRoot)
                    {
                        if (_connections == null)
                        {
                            _connections = new Dictionary<String, ConnectionProvider>();

                            // add a connection provider for each configured connection string
                            foreach (ConnectionStringSettings conn in ConnectionStrings)
                            {
                                _connections.Add(conn.Name, new ConnectionProvider(conn.Name, conn.ConnectionString));
                            }
                        }
                    }
                }

                return _connections;
            }
        }

        /// <summary>
        /// Adds the specified connection string to the map of connection strings.
        /// </summary>
        /// <param name="connectionStringName">The connection string name.</param>
        /// <param name="connectionString">The provider specific connection information.</param>
        public static void AddConnection(String connectionStringName, String connectionString)
        {
            lock (SyncRoot)
            {
                Connections.Remove(connectionStringName);
                ConnectionProvider connection = new ConnectionProvider(connectionStringName, connectionString);
                Connections.Add(connectionStringName, connection);
            }
        }

        /// <summary>
        /// Provides ability to switch connection string at runtime.
        /// </summary>
        public sealed class ConnectionProvider
        {
            private Shmzh.Components.SystemComponent.Provider _provider;
            private Shmzh.Components.SystemComponent.ProviderCollection _providers;
            private String _connectionStringName;
            private String _connectionString;


            /// <summary>
            /// Initializes a new instance of the ConnectionProvider class.
            /// </summary>
            /// <param name="connectionStringName">The connection string name.</param>
            /// <param name="connectionString">The provider specific connection information.</param>
            public ConnectionProvider(String connectionStringName, String connectionString)
            {
                _connectionString = connectionString;
                _connectionStringName = connectionStringName;
            }

            /// <summary>
            /// Gets the provider.
            /// </summary>
            public Shmzh.Components.SystemComponent.Provider Provider
            {
                get { LoadProviders(); return _provider; }
            }

            /// <summary>
            /// Gets the provider collection.
            /// </summary>
            public Shmzh.Components.SystemComponent.ProviderCollection Providers
            {
                get { LoadProviders(); return _providers; }
            }

            /// <summary>
            /// Instantiates the configured providers based on the supplied connection string.
            /// </summary>
            private void LoadProviders()
            {
                DataRepository.LoadProviders();

                // Avoid claiming lock if providers are already loaded
                if (_providers == null)
                {
                    lock (SyncRoot)
                    {
                        // Do this again to make sure _provider is still null
                        if (_providers == null)
                        {
                            var thisSection = Section();
                            // apply connection information to each provider
                            for (int i = 0; i < thisSection.Providers.Count; i++)
                            {
                                thisSection.Providers[i].Parameters["connectionStringName"] = _connectionStringName;
                                // remove previous connection string, if any
                                thisSection.Providers[i].Parameters.Remove("connectionString");

                                if (!String.IsNullOrEmpty(_connectionString))
                                {
                                    thisSection.Providers[i].Parameters["connectionString"] = _connectionString;
                                }
                            }

                            // Load registered providers and point _provider to the default provider
                            _providers = new Shmzh.Components.SystemComponent.ProviderCollection();

                            ProvidersHelper.InstantiateProviders(thisSection.Providers, _providers, typeof(Shmzh.Components.SystemComponent.Provider));
                            _provider = _providers[thisSection.DefaultProvider];
                        }
                    }
                }
            }
        }

        #endregion Connections

        #region Static properties
        ///<summary>
        /// 获取<see cref="CustomerInfo"/>业务实体的数据访问逻辑组件的当前实例.
        /// 它提供了CRUD方法.
        ///</summary>
        public static PathProvider PathProvider
        {
            get
            {
                LoadProviders();
                return _provider.PathProvider;
            }
        }
        ///<summary>
        /// 获取<see cref="InsuranceTypeInfo"/>业务实体的数据访问逻辑组件的当前实例.
        /// 它提供了CRUD方法.
        ///</summary>
        public static ItemProvider ItemProvider
        {
            get
            {
                LoadProviders();
                return _provider.ItemProvider;
            }
        }
        ///<summary>
        /// 获取<see cref="InsuranceInfo"/>业务实体的数据访问逻辑组件的当前实例.
        /// 它提供了CRUD方法.
        ///</summary>
        public static BoxProvider BoxProvider
        {
            get
            {
                LoadProviders();
                return _provider.BoxProvider;
            }
        }
        ///<summary>
        /// 获取<see cref="ClaimInfo"/>业务实体的数据访问逻辑组件的当前实例.
        /// 它提供了CRUD方法.
        ///</summary>
        public static BoxContractProvider BoxContractProvider
        {
            get
            {
                LoadProviders();
                return _provider.BoxContractProvider;
            }
        }
        ///<summary>
        /// 获取<see cref="ClaimDetailInfo"/>业务实体的数据访问逻辑组件的当前实例.
        /// 它提供了CRUD方法.
        ///</summary>
        public static BoxItemProvider BoxItemProvider
        {
            get
            {
                LoadProviders();
                return _provider.BoxItemProvider;
            }
        }
        #endregion
    }
}
