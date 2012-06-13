using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using Shmzh.Components.SystemComponent;
using Boxup.Data.Bases;
using Boxup.Entities;


namespace Boxup.Data.AccessClient
{
    class AccessBoxContractProvider:BoxContractProvider
    {
        #region Field
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string _connectionString;
        string _providerInvariantName;

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

        #endregion

        #region CTOR
        /// <summary>
        /// Creates a new <see cref="AccessBoxContractProvider"/> instance.
        /// </summary>
        public AccessBoxContractProvider()
        {
        }
        /// <summary>
        /// Creates a new <see cref="AccessBoxContractProvider"/> instance.
        /// Uses connection string to connect to datasource.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="providerInvariantName">Name of the invariant provider use by the DbProviderFactory.</param>
        public AccessBoxContractProvider(string connectionString, string providerInvariantName)
        {
            this._connectionString = connectionString;
            this._providerInvariantName = providerInvariantName;
        }
        #endregion

        #region private method
        /// <summary>
        /// 将dr转变到箱号与合同关系实体。
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>箱号与合同关系实体。</returns>
        private static BoxContractInfo ConvertToBoxContractInfo(IDataRecord dr)
        {
            var obj = new BoxContractInfo();
            obj.Id = long.Parse(dr["Id"].ToString());
            obj.BoxDate = DateTime.Parse(dr["BoxDate"].ToString());
            obj.BoxNo = int.Parse(dr["BoxNo"].ToString());
            obj.ContractNo = dr["ContractNo"].ToString();
            return obj;
        }

        /// <summary>
        /// 获取新插入记录的Id。
        /// </summary>
        /// <returns>新插入记录的Id。</returns>
        private long GetIdentity(TransactionManager trans)
        {
            var oRet = AccessHelper.ExecuteScalar(trans.TransactionObject as OleDbTransaction, "Select max(Id) From BoxContract ");
            return long.Parse(oRet.ToString());
        }
        #endregion

        public override long Insert(BoxContractInfo obj, TransactionManager trans)
        {
            var sqlStatement = "Insert Into BoxContract(BoxDate,BoxNo,ContractNo) Values (@BoxDate,@BoxNo,@ContractNo)";
            var parms = new[]{
                new OleDbParameter(){Value = obj.BoxDate},
                new OleDbParameter(){Value = obj.BoxNo},
                new OleDbParameter(){Value = obj.ContractNo},
            };
            
            try
            {
                if (trans == null)
                    AccessHelper.ExecuteNonQuery(this.ConnectionString, sqlStatement, parms);
                else
                    AccessHelper.ExecuteNonQuery(trans.TransactionObject as OleDbTransaction, sqlStatement, parms);
                var id = GetIdentity(trans);
                return id;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return 0;
            }
            
        }

        public override bool Delete(long id, TransactionManager trans)
        {
            var sqlStatement = "Delete From BoxContract Where Id = @Id";
            var parms = new[] { new OleDbParameter("@Id", OleDbType.BigInt) { Value = id } };
            try
            {
                if (trans == null)
                    AccessHelper.ExecuteNonQuery(this.ConnectionString, sqlStatement, parms);
                else
                    AccessHelper.ExecuteNonQuery(trans.TransactionObject as OleDbTransaction, sqlStatement, parms);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return false ;
            }
            
        }

        public override IList<BoxContractInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans)
        {
            var sqlStatement = "Select * From BoxContract Where BoxDate = @BoxDate";
            var parms = new[] { new OleDbParameter("@BoxDate", OleDbType.Date) { Value = boxDate } };
            OleDbDataReader dr;
            if (trans == null)
                dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement, parms);
            else
                dr = AccessHelper.ExecuteReader(trans.TransactionObject as OleDbTransaction, sqlStatement, parms);

            var objs = new List<BoxContractInfo>();
            while (dr.Read())
            {
                objs.Add(ConvertToBoxContractInfo(dr));
            }
            dr.Close();
            return objs;
        }
    }
}
