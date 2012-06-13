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
    class AccessBoxProvider:BoxProvider
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
        /// Creates a new <see cref="AccessBoxProvider"/> instance.
        /// </summary>
        public AccessBoxProvider()
        {
        }
        /// <summary>
        /// Creates a new <see cref="AccessBoxProvider"/> instance.
        /// Uses connection string to connect to datasource.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="providerInvariantName">Name of the invariant provider use by the DbProviderFactory.</param>
        public AccessBoxProvider(string connectionString, string providerInvariantName)
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
        private static BoxInfo ConvertToBoxInfo(IDataRecord dr)
        {
            var obj = new BoxInfo();
            obj.Id = long.Parse(dr["Id"].ToString());
            obj.BoxDate = DateTime.Parse(dr["BoxDate"].ToString());
            obj.BoxNo = int.Parse(dr["BoxNo"].ToString());
            obj.BoxName = dr["BoxName"].ToString();
            obj.BoxModel = dr["BoxModel"].ToString();
            obj.BoxSpec = dr["BoxSpec"].ToString();
            return obj;
        }

        /// <summary>
        /// 获取新插入记录的Id。
        /// </summary>
        /// <returns>新插入记录的Id。</returns>
        private long GetIdentity(TransactionManager trans)
        {
            var oRet = AccessHelper.ExecuteScalar(trans.TransactionObject as OleDbTransaction, "Select max(Id) From Box ");
            return long.Parse(oRet.ToString());
        }
        #endregion

        public override long Insert(BoxInfo obj, TransactionManager trans)
        {
            var sqlStatement = "Insert Into Box (BoxDate,BoxNo,BoxName,BoxModel,BoxSpec) Values (@BoxDate,@BoxNo,@BoxName,@BoxModel,@BoxSpec)";
            var parms = new[]
                            {
                                new OleDbParameter("@BoxDate",OleDbType.Date){Value = obj.BoxDate},
                                new OleDbParameter("@BoxNo",OleDbType.Integer){Value = obj.BoxNo},
                                new OleDbParameter("@BoxName",OleDbType.VarWChar,50){Value = obj.BoxName},
                                new OleDbParameter("@BoxModel",OleDbType.VarWChar,50){Value = obj.BoxModel},
                                new OleDbParameter("@BoxSpec",OleDbType.VarWChar,50){Value = obj.BoxSpec},
                            };
            try
            {
                AccessHelper.ExecuteNonQuery(trans.TransactionObject as OleDbTransaction, sqlStatement, parms);
                var id = this.GetIdentity(trans);
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
            var sqlStatement = "Delete From Box Where Id = @Id ";
            var parms = new[]{ new OleDbParameter("@Id",OleDbType.BigInt){Value = id},};
                try
                {
                    if (trans == null)
                        AccessHelper.ExecuteNonQuery(this.ConnectionString, sqlStatement,parms);
                    else
                        AccessHelper.ExecuteNonQuery(trans.TransactionObject as OleDbTransaction, sqlStatement,parms);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return false;
                }
            
            
        }

        public override IList<BoxInfo> GetByBoxDate(DateTime boxDate, TransactionManager trans)
        {
            var sqlStatement = "Select * From Box Where BoxDate = @BoxDate";
            var parms = new[]{new OleDbParameter("@BoxDate",OleDbType.Date){Value = boxDate}};
            var objs = new List<BoxInfo>();
            OleDbDataReader dr;
            if (trans == null)
                dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement, parms);
            else
                dr = AccessHelper.ExecuteReader(trans.TransactionObject as OleDbTransaction, sqlStatement, parms);
            while(dr.Read())
            {
                objs.Add(ConvertToBoxInfo(dr));
            }
            dr.Close();
            return objs;
        }

        public override BoxInfo GetById(long id)
        {
            var sqlStatement = "Select * From Box Where Id = @Id";
            var parms = new[]{ new OleDbParameter("@Id",OleDbType.BigInt){Value = id},};
            var dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement, parms);
            BoxInfo obj = null;
            while (dr.Read())
            {
                obj = ConvertToBoxInfo(dr);
                break;
            }
            dr.Close();
            return obj;
        }

        public override BoxInfo GetByBoxNo(int boxNo)
        {
            var sqlStatement = "Select * From Box Where BoxNo = @BoxNo";
            var parms = new[] { new OleDbParameter("@BoxNo", OleDbType.Integer) { Value = boxNo }, };
            var dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement, parms);
            BoxInfo obj = null;
            while (dr.Read())
            {
                obj = ConvertToBoxInfo(dr);
                break;
            }
            dr.Close();
            return obj;
        }
    }
}
