@echo off

if not "%2"=="" (
    echo [Fatal error] Too many arguments! Please, provide a single argument to this batch file: desired build configuration ^(it can be either "Debug" or "Release"^).
    pause
    exit /B 1
)

SET BUILDCONFIG=%1
if "%1"=="" SET /P BUILDCONFIG=Specify build configuration ^(it can be either "Debug" or "Release"^): 

if "%BUILDCONFIG%"=="Debug" (
    SET BUILDCONFIG=Debug
    echo Building Truesight using Debug configuration...
) else ( if "%BUILDCONFIG%"=="Release" (
    SET BUILDCONFIG=Release
    echo Building Truesight using Release configuration...
) else (
    echo [Fatal error] Unsupported build configuration "%BUILDCONFIG%". Only "Debug" or "Release" are supported.
    pause
    exit /B 1
))

if exist Sources rmdir Sources /s /q
mkdir Sources
cd Sources

echo.
echo ^>^>^>^>^> Downloading Truesight sources from https://truesight-lite.googlecode.com/hg/
hg clone https://truesight-lite.googlecode.com/hg/ Truesight -r e37be224679f
if not exist Truesight (
    echo [Fatal error] Failed to get Truesight sources.
    pause
    cd ..\
    exit /B 1
)

echo.
echo ^>^>^>^>^> Building Truesight...
cd Truesight\Truesight
"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild" /t:Rebuild /p:Configuration=%BUILDCONFIG%
if not exist bin\%BUILDCONFIG%\Truesight.dll (
    echo [Fatal error] Failed to build Truesight.
    pause
    cd ..\..\..\
    exit /B 1
)

echo.
echo ^>^>^>^>^> Provisioning Truesight...
cd ..\..\..\
if not exist Binaries mkdir Binaries
cd Binaries
if exist %BUILDCONFIG% rmdir %BUILDCONFIG% /s /q
mkdir %BUILDCONFIG%
cd %BUILDCONFIG%
ilmerge /t:library /targetplatform:v4,C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319 /keyfile:..\..\Sources\Truesight\Truesight\Truesight.snk /out:Truesight.dll ..\..\Sources\Truesight\Truesight\bin\%BUILDCONFIG%\Truesight.dll ..\..\Sources\Truesight\Truesight\bin\%BUILDCONFIG%\QuickGraph.dll ..\..\Sources\Truesight\Truesight\bin\%BUILDCONFIG%\QuickGraph.GraphViz.dll /allowDup /internalize /log
if not exist Truesight.dll (
    echo [Fatal error] Failed to provision Truesight.
    pause
    cd ..\..
    exit /B 1
)

cd ..\..
rmdir Sources /s /q