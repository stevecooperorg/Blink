using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blink.Tests.TestDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blink.Tests
{

    [TestClass]
    public class SqlServerInfoTests
    {
        [TestMethod]
        public void ShouldReturnReasonableDataDirectory()
        {
            var context = new TestDbContext();
            var sqlServerInfo = new SqlServerInfo(context);

            var actualBackupDirectory = sqlServerInfo.GetBackupDirectory();
            var actualDataDirectory = sqlServerInfo.GetDataDirectory();
            var acutalLogDirectory = sqlServerInfo.GetLogDirectory();

            var escapedPrefix = Regex.Escape(@"C:\Program Files\Microsoft SQL Server\MSSQL");
            
            RegexAssert.IsMatch(@"^" + escapedPrefix + @".*\.MSSQLSERVER\\MSSQL\\Backup", actualBackupDirectory);
            RegexAssert.IsMatch(@"^" + escapedPrefix + @".*\.MSSQLSERVER\\MSSQL\\DATA", actualDataDirectory);
            RegexAssert.IsMatch(@"^" + escapedPrefix + @".*\.MSSQLSERVER\\MSSQL\\DATA", acutalLogDirectory);
           
        }
    }
}
