﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blink.SQL {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SqlScripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlScripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Blink.SQL.SqlScripts", typeof(SqlScripts).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BACKUP DATABASE @sourceDb TO DISK = @backupPath
        ///.
        /// </summary>
        internal static string Backup {
            get {
                return ResourceManager.GetString("Backup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @path NVARCHAR(4000) 
        ///
        ///EXEC master.dbo.xp_instance_regread 
        ///        N&apos;HKEY_LOCAL_MACHINE&apos;, 
        ///        N&apos;Software\Microsoft\MSSQLServer\MSSQLServer&apos;,N&apos;BackupDirectory&apos;, 
        ///        @path OUTPUT,  
        ///        &apos;no_output&apos; 
        ///select @path .
        /// </summary>
        internal static string GetBackupDirectory {
            get {
                return ResourceManager.GetString("GetBackupDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RESTORE DATABASE @databaseName 
        ///FROM DISK = @backupPath
        ///.
        /// </summary>
        internal static string Restore {
            get {
                return ResourceManager.GetString("Restore", resourceCulture);
            }
        }
    }
}
