@echo off
REM Script para ejecutar una consulta SQL en la base de datos de desarrollo.
REM ADVERTENCIA: Las credenciales de la base de datos de desarrollo están en texto plano en este archivo.

SET DB_HOST=localhost
SET DB_USER=root
SET DB_PASS=BMPC
SET DB_NAME=tecmeindb

SET QUERY=%1

if [%1]==[] (
    echo Error: Debes proporcionar una consulta SQL como argumento entre comillas.
    echo Ejemplo: query_db.bat "SELECT * FROM AuditoriaEventos LIMIT 10;"
    goto :eof
)

echo Ejecutando consulta en la base de datos '%DB_NAME%'...
echo.
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -h %DB_HOST% -u %DB_USER% -p%DB_PASS% -D %DB_NAME% -e %QUERY%
