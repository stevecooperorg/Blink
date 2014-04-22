DECLARE @backupDirectory NVARCHAR(4000) = null,
        @dataDirectory nvarchar(4000) = null,
        @logDirectory nvarchar(4000) = null;

-- read the backup directory
EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE'
    ,N'Software\Microsoft\MSSQLServer\MSSQLServer'
    ,N'BackupDirectory'
    ,@backupDirectory OUTPUT
    ,'no_output'

-- read the override of the default data directory (null if not configured yet)
EXEC master.dbo.xp_instance_regread 
    N'HKEY_LOCAL_MACHINE'
    , N'Software\Microsoft\MSSQLServer\MSSQLServer'
    , N'DefaultData'
    , @dataDirectory output;

IF @dataDirectory IS Null
  -- user hasn't overridden the default locations -- find the path for master db data file (file_id=1);
  SELECT @dataDirectory = SUBSTRING([physical_name], 1, LEN([physical_name])-CHARINDEX('\',REVERSE([physical_name])))
  FROM master.sys.database_files WHERE file_id = 1;
  
EXEC master.dbo.xp_instance_regread 
    N'HKEY_LOCAL_MACHINE'
    , N'Software\Microsoft\MSSQLServer\MSSQLServer'
    , N'DefaultLog'
    , @logDirectory output;

IF @logDirectory IS Null
  -- user hasn't overridden the default log locations -- find the path for master db log file (file_id=2);
  SELECT @logDirectory = SUBSTRING([physical_name], 1, LEN([physical_name])-CHARINDEX('\',REVERSE([physical_name])))
  FROM master.sys.database_files WHERE file_id = 2;
  
SELECT @backupDirectory AS [backupDirectory], @dataDirectory as [dataDirectory], @logDirectory as [logDirectory]
