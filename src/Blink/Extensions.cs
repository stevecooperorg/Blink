using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    internal static class Extensions
    {
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
