@echo off
setlocal enabledelayedexpansion
title Planner WinUI 3 - Build Tool

:: Se foi passado um argumento por linha de comandos, salta o menu interativo
if not "%~1"=="" goto HANDLE_ARG

:MENU
cls
echo =======================================================
echo          Planner WinUI 3 - Build & Dev Tool
echo =======================================================
echo.
echo   [1] Compilar Rapido (Debug / Incremental)
echo   [2] Compilar e Executar (Build & Run)
echo   [3] Limpeza Completa e Rebuild (Clean Rebuild)
echo   [4] Compilar em Modo Release
echo   [5] Executar sem Compilar (Instant Run)
echo   [6] Sair
echo.
echo =======================================================
set /p OPT="Escolha uma opcao [1-6]: "

if "%OPT%"=="1" goto DO_BUILD_DEBUG
if "%OPT%"=="2" goto DO_RUN
if "%OPT%"=="3" goto DO_CLEAN
if "%OPT%"=="4" goto DO_BUILD_RELEASE
if "%OPT%"=="5" goto DO_INSTANT_RUN
if "%OPT%"=="6" exit /b 0

echo [!] Opcao invalida. Prima qualquer tecla para tentar de novo...
pause >nul
goto MENU

:HANDLE_ARG
if /i "%~1"=="run" goto DO_RUN
if /i "%~1"=="clean" goto DO_CLEAN
if /i "%~1"=="release" goto DO_BUILD_RELEASE
if /i "%~1"=="start" goto DO_INSTANT_RUN
goto DO_BUILD_DEBUG

:DO_BUILD_DEBUG
echo.
echo [*] A compilar projeto Planner (Debug / x64)...
dotnet build Planner.csproj -c Debug -p:Platform=x64 --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo [!] Build incremental falhou. A tentar com restore...
    dotnet build Planner.csproj -c Debug -p:Platform=x64
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo [ERRO] A compilacao falhou!
        if "%~1"=="" pause
        exit /b 1
    )
)
echo.
echo [SUCESSO] Compilacao concluida com sucesso!
if "%~1"=="" pause
exit /b 0

:DO_RUN
echo.
echo [*] A compilar projeto Planner (Debug / x64)...
dotnet build Planner.csproj -c Debug -p:Platform=x64 --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo [!] Build incremental falhou. A tentar com restore...
    dotnet build Planner.csproj -c Debug -p:Platform=x64
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo [ERRO] A compilacao falhou!
        if "%~1"=="" pause
        exit /b 1
    )
)
echo [SUCESSO] Compilacao concluida!
goto LAUNCH_APP

:DO_CLEAN
echo.
echo [*] A remover pastas bin e obj...
if exist "bin" rd /s /q "bin"
if exist "obj" rd /s /q "obj"
echo [OK] Pastas limpas.
echo [*] A recompilar projeto do zero (Debug / x64)...
dotnet build Planner.csproj -c Debug -p:Platform=x64
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERRO] A recompilacao limpa falhou!
    if "%~1"=="" pause
    exit /b 1
)
echo.
echo [SUCESSO] Rebuild limpo concluido com sucesso!
if "%~1"=="" pause
exit /b 0

:DO_BUILD_RELEASE
echo.
echo [*] A compilar projeto Planner (Release / x64)...
dotnet build Planner.csproj -c Release -p:Platform=x64
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERRO] A compilacao em Release falhou!
    if "%~1"=="" pause
    exit /b 1
)
echo.
echo [SUCESSO] Compilacao em Release concluida com sucesso!
if "%~1"=="" pause
exit /b 0

:DO_INSTANT_RUN
goto LAUNCH_APP

:LAUNCH_APP
set EXE_PATH=bin\x64\Debug\net10.0-windows10.0.26100.0\win-x64\Planner.exe
if not exist "!EXE_PATH!" (
    set EXE_PATH=bin\x64\Release\net10.0-windows10.0.26100.0\win-x64\Planner.exe
)
if exist "!EXE_PATH!" (
    echo [*] A iniciar Planner.exe...
    start "" "!EXE_PATH!"
) else (
    echo [!] Executavel nao encontrado. Por favor, compile o projeto primeiro.
)
if "%~1"=="" (
    echo.
    echo Prima qualquer tecla para voltar ao menu...
    pause >nul
    goto MENU
)
exit /b 0
