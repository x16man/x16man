using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Boxup.Data;
using Boxup.Data.Bases;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.AccessClient
{
    public sealed partial class AccessDataProvider : DataProvider
    {
        #region Field
        private static object syncRoot = new Object();
        private string _applicationName;
        private string _connectionString;
        string _providerInvariantName;
        private string _useGzip;
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return this._connectionString; }
            set { this._connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the invariant provider name listed in the DbProviderFactories machine.config section.
        /// </summary>
        /// <value>The name of the provider invariant.</value>
        public string ProviderInvariantName
        {
            get { return this._providerInvariantName; }
            set { this._providerInvariantName = value; }
        }
        ///<summary>
        /// Indicates if the current <see cref="Provider"/> implementation supports Transacton.
        ///</summary>
        public override bool IsTransactionSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 是否使用GZip.
        /// </summary>
        public string UseGZip
        {
            get { return this._useGzip; }
            set { this._useGzip = value; }
        }
        #endregion

        #region CTOR
        /// <summary>
		/// Initializes a new instance of the <see cref="SqlDataProvider"/> class.
		///</summary>
		public AccessDataProvider()
		{	
		}	
        #endregion

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
            {
                name = "AccessDataProvider";
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            _applicationName = config["applicationName"];

            if (string.IsNullOrEmpty(_applicationName))
            {
                _applicationName = "/";
            }
            config.Remove("applicationName");

            #region ConnectionString

            // Initialize _connectionString
            _connectionString = config["connectionString"];
            config.Remove("connectionString");

            string connect = config["connectionStringName"];
            config.Remove("connectionStringName");

            if (String.IsNullOrEmpty(_connectionString))
            {
                if (String.IsNullOrEmpty(connect))
                {
                    throw new ProviderException("Empty or missing connectionStringName");
                }

                if (DataRepository.ConnectionStrings[connect] == null)
                {
                    throw new ProviderException("Missing connection string");
                }

                _connectionString = DataRepository.ConnectionStrings[connect].ConnectionString;
            }

            if (String.IsNullOrEmpty(_connectionString))
            {
                throw new ProviderException("Empty connection string");
            }

            #endregion

            #region _providerInvariantName

            // initialize _providerInvariantName
            this._providerInvariantName = config["providerInvariantName"];

            if (String.IsNullOrEmpty(_providerInvariantName))
            {
                throw new ProviderException("Empty or missing providerInvariantName");
            }
            config.Remove("providerInvariantName");

            #endregion

            #region _useGZip

            // initialize _useGZip
            this._useGzip = config["useGZip"];

            if (String.IsNullOrEmpty(_useGzip))
            {
                _useGzip = "false";
            }
            config.Remove("useGZip");
            #endregion
        }

        /// <summary>
        /// Creates a new <see cref="TransactionManager"/> instance from the current datasource.
        /// </summary>
        /// <returns></returns>
        public override TransactionManager CreateTransaction()
        {
            return new TransactionManager(this._connectionString,"System.Data.OleDb");
        }
    }
}
