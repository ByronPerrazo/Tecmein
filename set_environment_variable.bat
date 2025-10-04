@echo off
REM -----------------------------------------------------------------
REM Script para configurar la variable de entorno de la cadena de conexión para la aplicación Tecmein.
REM
REM MODO DE USO:
REM 1. Edita la variable CONN_STRING con la cadena de conexión completa y real.
REM 2. Edita la variable VAR_NAME para que coincida con la conexión que quieres usar 
REM    (ej. ConnectionStrings__ConexionDBAWS o ConnectionStrings__ConexionDBNube).
REM 3. Guarda el archivo.
REM 4. Ejecuta este script como ADMINISTRADOR.
REM -----------------------------------------------------------------

ECHO.
ECHO *** Configurador de Variable de Entorno para Tecmein ***
ECHO.

REM --- PASO 1: Edita la cadena de conexión aquí ---
SET CONN_STRING="server=databasews.cp6yi4wyscn1.us-east-1.rds.amazonaws.com;user=admin;password=TU_PASSWORD_SECRETO_AQUI;database=tecmeindb"

REM --- PASO 2: Edita el nombre de la variable a establecer ---
SET VAR_NAME=ConnectionStrings__ConexionDBAWS


ECHO.
ECHO Se va a establecer la siguiente variable de entorno a nivel de maquina:
ECHO.
ECHO   Variable: %VAR_NAME%
ECHO   Valor   : %CONN_STRING%
ECHO.

CHOICE /C SN /M "¿Estas seguro de que deseas continuar (S/N)?"

IF ERRORLEVEL 2 GOTO END
IF ERRORLEVEL 1 GOTO APPLY

:APPLY
ECHO.
ECHO Estableciendo variable...
setx %VAR_NAME% %CONN_STRING% /M

ECHO.
ECHO ¡Hecho! La variable de entorno ha sido establecida.
ECHO.
ECHO IMPORTANTE: Reinicia el servicio de la aplicacion (IIS) o el servidor
ECHO para que los cambios tomen efecto.
ECHO.
GOTO END

:END
ECHO.
pause
