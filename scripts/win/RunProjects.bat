@echo off
:menu
cls
echo ================================
echo Select an option to run all LHA projects:
echo ================================
echo 1. dotnet run --no-build
echo 2. dotnet run
echo 3. dotnet watch run
echo 4. Exit
echo ================================
set /p choice=Enter your choice (1-4): 

if %choice%==1 goto run_no_build
if %choice%==2 goto run
if %choice%==3 goto watch_run
if %choice%==4 goto exit

:run_no_build
wt -w 0 nt --title "ApiGateway" cmd /k "cd /d %~dp0..\..\src\gateways\LHA.ApiGateway && dotnet run --no-build"; nt --title "Account.API" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.HttpApi.Host && dotnet run --no-build"; nt --title "Mega.API" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.HttpApi.Host && dotnet run --no-build"; nt --title "Movie.API" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.HttpApi.Host && dotnet run --no-build"; nt --title "Notification.API" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.HttpApi.Host && dotnet run --no-build"; nt --title "Account.Cron" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Cron && dotnet run --no-build"; nt --title "Mega.Cron" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Cron && dotnet run --no-build"; nt --title "Movie.Cron" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Cron && dotnet run --no-build"; nt --title "Notification.Cron" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Cron && dotnet run --no-build"; nt --title "Account.Consumer" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Consumer && dotnet run --no-build"; nt --title "Mega.Consumer" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Consumer && dotnet run --no-build"; nt --title "Movie.Consumer" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Consumer && dotnet run --no-build"; nt --title "Notification.Consumer" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Consumer && dotnet run --no-build"; nt --title "Test.API" cmd /k "cd /d %~dp0..\..\test\Test.API && dotnet run --no-build"
pause
goto menu

:run
wt -w 0 nt --title "ApiGateway" cmd /k "cd /d %~dp0..\..\src\gateways\LHA.ApiGateway && dotnet run"; nt --title "Account.API" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.HttpApi.Host && dotnet run"; nt --title "Mega.API" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.HttpApi.Host && dotnet run"; nt --title "Movie.API" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.HttpApi.Host && dotnet run"; nt --title "Notification.API" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.HttpApi.Host && dotnet run"; nt --title "Account.Cron" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Cron && dotnet run"; nt --title "Mega.Cron" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Cron && dotnet run"; nt --title "Movie.Cron" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Cron && dotnet run"; nt --title "Notification.Cron" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Cron && dotnet run"; nt --title "Account.Consumer" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Consumer && dotnet run"; nt --title "Mega.Consumer" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Consumer && dotnet run"; nt --title "Movie.Consumer" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Consumer && dotnet run"; nt --title "Notification.Consumer" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Consumer && dotnet run"; nt --title "Test.API" cmd /k "cd /d %~dp0..\..\test\Test.API && dotnet run"
pause
goto menu

:watch_run
wt -w 0 nt --title "ApiGateway" cmd /k "cd /d %~dp0..\..\src\gateways\LHA.ApiGateway && dotnet watch run"; nt --title "Account.API" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.HttpApi.Host && dotnet watch run"; nt --title "Mega.API" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.HttpApi.Host && dotnet watch run"; nt --title "Movie.API" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.HttpApi.Host && dotnet watch run"; nt --title "Notification.API" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.HttpApi.Host && dotnet watch run"; nt --title "Account.Cron" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Cron && dotnet watch run"; nt --title "Mega.Cron" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Cron && dotnet watch run"; nt --title "Movie.Cron" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Cron && dotnet watch run"; nt --title "Notification.Cron" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Cron && dotnet watch run"; nt --title "Account.Consumer" cmd /k "cd /d %~dp0..\..\src\services\account\LHA.Account.Consumer && dotnet watch run"; nt --title "Mega.Consumer" cmd /k "cd /d %~dp0..\..\src\services\meaga\LHA.Mega.Consumer && dotnet watch run"; nt --title "Movie.Consumer" cmd /k "cd /d %~dp0..\..\src\services\movie\LHA.Movie.Consumer && dotnet watch run"; nt --title "Notification.Consumer" cmd /k "cd /d %~dp0..\..\src\services\notifications\LHA.Notification.Consumer && dotnet watch run"; nt --title "Test.API" cmd /k "cd /d %~dp0..\..\test\Test.API && dotnet watch run"
pause
goto menu

:exit
exit
