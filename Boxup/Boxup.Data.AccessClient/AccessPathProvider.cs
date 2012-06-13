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
    class AccessPathProvider:PathProvider
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
        /// Creates a new <see cref="AccessPathProvider"/> instance.
        /// </summary>
        public AccessPathProvider()
        {
        }
        /// <summary>
        /// Creates a new <see cref="AccessPathProvider"/> instance.
        /// Uses connection string to connect to datasource.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="providerInvariantName">Name of the invariant provider use by the DbProviderFactory.</param>
        public AccessPathProvider(string connectionString, string providerInvariantName)
        {
            this._connectionString = connectionString;
            this._providerInvariantName = providerInvariantName;
        }
        #endregion

        #region private method
        /// <summary>
        /// 将dr转变到路径实体。
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>路径实体。</returns>
        private static PathInfo ConvertToPathInfo(IDataRecord dr)
        {
            var obj = new PathInfo();
            obj.LastPath = dr["LastPath"].ToString();

            return obj;
        }

        /// <summary>
        /// 获取新插入记录的Id。
        /// </summary>
        /// <returns>新插入记录的Id。</returns>
        private long GetIdentity(OleDbTransaction trans)
        {
            var oRet = AccessHelper.ExecuteScalar(trans, "Select max(Id) From Claims ");
            return long.Parse(oRet.ToString());
        }
        #endregion

        public override bool Insert(PathInfo obj)
        {
            var sqlStatement = "Insert Into Path ([LastPath]) Values (@LastPath)";
            var parms = new[]
                            {
                                new OleDbParameter("@LastPath",OleDbType.VarWChar,255){Value = obj.LastPath},
                            };
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    AccessHelper.ExecuteNonQuery(trans, sqlStatement, parms);
                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Logger.Error(ex.Message, ex);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override bool Delete()
        {
            var sqlStatement = "Delete From Path ";
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    AccessHelper.ExecuteNonQuery(trans, sqlStatement);
                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Logger.Error(ex.Message, ex);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override PathInfo Get()
        {
            var sqlStatement = "Select * From Path";
            var dr = AccessHelper.ExecuteReader(this.ConnectionString, sqlStatement);
            PathInfo obj = null;
            while (dr.Read())
            {
                obj = ConvertToPathInfo(dr);
                break;
            }
            dr.Close();
            return obj;
        }
    }
}
