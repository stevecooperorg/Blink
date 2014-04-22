using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blink.Util;
using Blink.SQL;

namespace Blink
{
    public class SqlServerInfo
    {
        private readonly DbContext context;
        
        public SqlServerInfo(DbContext context)
        {
            this.context = context;                
        }

        public string GetDataDirectory()
        {
            return ReadProperty("dataDirectory");
        }

        public string GetLogDirectory()
        {
            return ReadProperty("logDirectory");
        }

        public string GetBackupDirectory()
        {
            return ReadProperty("backupDirectory");
        }

        private string ReadProperty(string propertyName)
        {
            var ds = this.context.QuerySqlAsMaster(SqlScripts.GetBackupDirectory);
            var dt = ds.Tables[0];
            return dt.Rows[0][propertyName] as string;
        }
    }
}
