using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using Boxup.Data.Bases;
using Boxup.Entities;
using Shmzh.Components.SystemComponent;

namespace Boxup.Data.AccessClient
{
    class AccessItemProvider:ItemProvider
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
        /// Creates a new <see cref="AccessItemProvider"/> instance.
        /// </summary>
        public AccessItemProvider()
        {
        }
        /// <summary>
        /// Creates a new <see cref="AccessItemProvider"/> instance.
        /// Uses connection string to connect to datasource.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="providerInvariantName">Name of the invariant provider use by the DbProviderFactory.</param>
        public AccessItemProvider(string connectionString, string providerInvariantName)
        {
            this._connectionString = connectionString;
            this._providerInvariantName = providerInvariantName;
        }
        #endregion

        #region private method
        /// <summary>
        /// 将dr转变到保险理赔单实体。
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>保险理赔单实体。</returns>
        private static ItemInfo ConvertToItemInfo(IDataRecord dr)
        {
            var obj = new ItemInfo();
            obj.OldItemName = obj.ItemName = dr["ItemName"].ToString();
            obj.d = dr["d"] == DBNull.Value ? 0 : decimal.Parse(dr["d"].ToString());
            obj.c = dr["c"] == DBNull.Value ? 0 : int.Parse(dr["c"].ToString());
            return obj;
        }

        /// <summary>
        /// 获取新插入记录的Id。
        /// </summary>
        /// <returns>新插入记录的Id。</returns>
        private long GetIdentity(TransactionManager trans)
        {
            var oRet = AccessHelper.ExecuteScalar(trans.TransactionObject as OleDbTransaction, "Select max(Id) From Claims ");
            return long.Parse(oRet.ToString());
        }
        #endregion

        public override bool Insert(ItemInfo obj ,TransactionManager trans)
        {
            var sqlStatement = "Insert Into Item ([ItemName],[d],[c]) Values (@ItemName,@d,@c)";
            var parms = new[]
                            {
                                new OleDbParameter("@ItemName",OleDbType.VarWChar,50){Value = obj.ItemName},
                                new OleDbParameter("@d",OleDbType.Decimal){Value = obj.d==0?DBNull.Value:(object)obj.d},
                                new OleDbParameter("@c",OleDbType.Integer){Value = obj.c==0?DBNull.Value:(object)obj.c},
                            };
            
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
                return false;
            }
        }

        public override bool Update(ItemInfo obj, TransactionManager trans)
        {
            var sqlStatement = "Update Item Set ItemName = @ItemName,d=@d,c=@c Where ItemName = @OldItemName";
            var parms = new[] { 
                new OleDbParameter("@ItemName",OleDbType.VarWChar,50){Value = obj.ItemName},
                new OleDbParameter("@OldItemName",OleDbType.VarWChar,50){Value = obj.OldItemName},
                new OleDbParameter("@d",OleDbType.Decimal){Value = obj.d ==0?DBNull.Value:(object)obj.d},
                new OleDbParameter("@c",OleDbType.Integer){Value = obj.c==0?DBNull.Value:(object)obj.c},
            };
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
                return false;
            }
            
        }

        public override bool Delete(string itemName, TransactionManager trans)
        {
            var sqlStatement = "Delete From Item Where ItemName = @ItemName";
            var parms = new[] { 
                new OleDbParameter("@ItemName",OleDbType.VarWChar,50){Value = itemName},
            };
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
                return false;
            }
            
        }

        public override IList<ItemInfo> GetAll(TransactionManager trans)
        {
            var sqlStatement = "Select * From Item";
            var objs = new List<ItemInfo>();
            OleDbDataReader dr ;
            if (trans == null)
                dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement);
            else
                dr = AccessHelper.ExecuteReader(trans.TransactionObject as OleDbTransaction, sqlStatement);
            while (dr.Read())
            {
                objs.Add(ConvertToItemInfo(dr));
            }
            dr.Close();
            return objs;
        }
    }
}
