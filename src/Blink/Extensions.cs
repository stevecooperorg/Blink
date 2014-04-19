﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    internal static class Extensions
    {
        public static void ExecuteSqlAsMaster(this DbContext context, string sql, object paramObject)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            var properties = paramObject.GetType().GetProperties();

            foreach (var property in properties)
            {
                var name = "@" + property.Name;
                var value = property.GetValue(paramObject);
                var sqlParameter = new SqlParameter(name, value);
                parameters.Add(sqlParameter);
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(context.Database.Connection.ConnectionString);

            connectionStringBuilder.InitialCatalog = "Master";
            var connectionString = connectionStringBuilder.ToString();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 60;
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }                
            }

            //context.Database.ExecuteSqlCommand(
            //    TransactionalBehavior.DoNotEnsureTransaction, 
            //    sql, 
            //    parameters.ToArray());
        }

        public static string ToEdmx(this DbContext context)
        {
            var sb = new StringBuilder();

            using (var textWriter = new StringWriter(sb))
            using (var xmlWriter = System.Xml.XmlWriter.Create(textWriter, new System.Xml.XmlWriterSettings { Indent = true, IndentChars = "    " }))
            {
                System.Data.Entity.Infrastructure.EdmxWriter.WriteEdmx(context, xmlWriter);
                textWriter.Flush();
            }

            return sb.ToString();
        }

        public static string DbContextHash(this DbContext context)
        {
            var edmx = context.ToEdmx();
            var hashBytes = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(edmx));
            var hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }

    }
}
