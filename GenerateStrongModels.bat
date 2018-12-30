@echo off
if not "%1"=="am_admin" (powershell start -verb runas '%0' am_admin & exit /b)
SET pathToGenerator=%UserProfile%\source\repos\cloud-generators-net\src\CloudModelGenerator\bin\Release\netcoreapp2.0\win-x64
start /d %pathToGenerator% CloudModelGenerator.exe -p "1b885525-0840-00e6-7ff7-a9837e243cd5" -n "VERSUS.Kentico.Types" -o "C:\inetpub\wwwroot\VERSUS\VERSUS.Kentico.Types\Types" -s=true -t=false
