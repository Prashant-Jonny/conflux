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
    echo Building Libptx using Debug configuration...
) else ( if "%BUILDCONFIG%"=="Release" (
    SET BUILDCONFIG=Release
    echo Building Libptx using Release configuration...
) else (
    echo [Fatal error] Unsupported build configuration "%BUILDCONFIG%". Only "Debug" or "Release" are supported.
    pause
    exit /B 1
))

if exist Sources rmdir Sources /s /q
mkdir Sources
cd Sources

echo.
echo ^>^>^>^>^> Downloading Libptx sources from https://libptx.googlecode.com/hg/
hg clone https://libptx.googlecode.com/hg/ Libptx -r 816c0d215f07
if not exist Libptx (
    echo [Fatal error] Failed to get Libptx sources.
    pause
    cd ..\
    exit /B 1
)

echo.
echo ^>^>^>^>^> Building binary dependencies: Libcuda...
cd Libptx\ThirdPartyStuff\libcuda
call build-libcuda %BUILDCONFIG%
if not exist Binaries\%BUILDCONFIG%\Libcuda.dll (
    echo [Fatal error] Failed to build Libcuda.
    pause
    cd ..\..\..\..\
    exit /B 1
) else (
    cd ..\..\..\
)

echo.
echo ^>^>^>^>^> Building Libptx...
cd Libptx\Libptx
"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild" /t:Rebuild /p:Configuration=%BUILDCONFIG%
if not exist bin\%BUILDCONFIG%\Libptx.dll (
    echo [Fatal error] Failed to build Libptx.
    pause
    cd ..\..\..\
    exit /B 1
)

echo.
echo ^>^>^>^>^> Provisioning Libptx...
cd ..\..\..\
if not exist Binaries mkdir Binaries
cd Binaries
if exist %BUILDCONFIG% rmdir %BUILDCONFIG% /s /q
mkdir %BUILDCONFIG%
cd %BUILDCONFIG%
copy ..\..\Sources\Libptx\Libptx\bin\%BUILDCONFIG%\Libptx.* Libptx.* /Y
if not exist Libptx.dll (
    echo [Fatal error] Failed to provision Libptx.
    pause
    cd ..\..
    exit /B 1
)

cd ..\..
rmdir Sources /s /q