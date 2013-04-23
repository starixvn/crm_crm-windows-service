@ECHO OFF

REM The following directory is for .NET 2.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

InstallUtil /u "D:\NexcelProjects\Rome\CRM\Dev\CRMUpdateDataService\bin\Debug\Fanex.CRM.CRMReplication.exe"


pause