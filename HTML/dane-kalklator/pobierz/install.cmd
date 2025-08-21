@echo off
title Instalator Kalklator
color 0A

:: Sprawdzenie uprawnień administratora
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Instalator nie zostal uruchomiony jako administrator.
    echo Uruchom ponownie plik jako administrator.
    echo.
    echo Nacisnij dowolny klawisz aby zamknac okno instalatora
    pause >nul
    exit /b
)

setlocal enabledelayedexpansion
set "defaultDir=C:\Program Files\Kalklator"
set "exePath=%defaultDir%\Kalklator.exe"

echo.
call :echo-align center "Witaj w Instalatorze Kalklator"
call :echo-align center "Programix - Producent Programow Watpliwej Jakosci"
echo.

if exist "!exePath!" (
    echo Wykryto juz zainstalowany Kalklator w:
    echo !exePath!
    echo.
    echo [1] Napraw - ponownie zainstaluj
    echo [2] Odinstaluj
    echo [3] Anuluj
    set /p choice=Wybierz opcje: 
    if "!choice!"=="1" (
        goto install
    ) else if "!choice!"=="2" (
        echo Odinstalowywanie...
		rmdir %defaultDir%
        del /f /q "!exePath!" >nul 2>&1
        reg delete "HKCU\Software\Classes\.kalk" /f >nul 2>&1
        reg delete "HKCU\Software\Classes\Kalklator.File" /f >nul 2>&1
        echo Odinstalowano pomyslnie.
        echo.
        pause
        exit
    ) else (
        echo Anulowano.
        exit
    )
) else (
	goto install
)

:install

echo.
set /p installDir=Podaj sciezke instalacji [domyslnie: !defaultDir!]: 

if "!installDir!"=="" (
    set "installDir=!defaultDir!"
)
set "exePath=!installDir!\Kalklator.exe"

echo.
echo Instalacja w: !installDir!
echo.

REM Tworzenie folderu
if not exist "!installDir!" (
    mkdir "!installDir!"
) else (
    echo Folder istnieje, kontynuuje...
)

echo.
echo Pobieranie pliku kalklator.exe z serwera...
powershell -Command "Invoke-WebRequest -Uri 'https://pobierz-kalklator.netlify.app/pobierz/kalklator.exe' -OutFile '!exePath!'" >nul 2>&1

cls
echo Instalacja zakonczona.
echo.

if exist "!exePath!" (
    echo Plik zapisano jako: !exePath!
    echo.

    echo Rejestracja rozszerzenia .kalk w rejestrze...
    reg add "HKCU\Software\Classes\.kalk\OpenWithProgids" /v Kalklator.File /t REG_NONE /f >nul
    reg add "HKCU\Software\Classes\Kalklator.File" /ve /d "Plik Kalklatora" /f >nul
    reg add "HKCU\Software\Classes\Kalklator.File\DefaultIcon" /ve /d "!exePath!,0" /f >nul
    reg add "HKCU\Software\Classes\Kalklator.File\shell\open\command" /ve /d "\"!exePath!\" \"%%1\"" /f >nul

    echo Rejestracja zakonczona pomyslnie!
    echo.
    echo Uruchom aplikacje wpisujac:
    echo "!exePath!"
) else (
    echo Wystapil blad podczas pobierania pliku!
    echo Sprawdz polaczenie z internetem lub adres URL.
)

echo.
echo Nacisnij dowolny klawisz aby zamknac okno instalatora
pause >nul
exit


:echo-align <align> <text>
	setlocal EnableDelayedExpansion
	(set^ tmp=%~2)
	if defined tmp (
		set "len=1"
		for %%p in (4096 2048 1024 512 256 128 64 32 16 8 4 2 1) do (
			if "!tmp:~%%p,1!" neq "" (
				set /a "len+=%%p"
				set "tmp=!tmp:~%%p!"
			)
		)
	) else (
		set len=0
	)

	for /f "skip=4 tokens=2 delims=:" %%i in ('mode con') do (
		set /a cols=%%i
		goto loop_end
	)
	:loop_end

	if /i "%1" equ "center" (
		set /a offsetnum=^(%cols% / 2^) - ^(%len% / 2^)
		set "offset="
		for /l %%i in (1 1 !offsetnum!) do set "offset=!offset! "
	) else if /i "%1" equ "right" (
		set /a offsetnum=^(%cols% - %len%^)
		set "offset="
		for /l %%i in (1 1 !offsetnum!) do set "offset=!offset! "
	)

	echo %offset%%~2
	endlocal
	exit /b