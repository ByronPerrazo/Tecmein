# --- Configuración ---
$DbHost = "localhost"
$DbPort = 3306
$DbUser = "root"
$DbPassword = "T3cm31n*2O2S"
$DbName = "tecmeindb"
$BackupDir = "C:\Users\Server TEC\TecmeinWebApp\BackupsDB"
$MysqldumpPath = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe"

# --- Verificación de Directorio y mysqldump ---
if (-not (Test-Path -Path $BackupDir)) {
    Write-Host "Creando directorio de backup: $BackupDir"
    try {
        New-Item -Path $BackupDir -ItemType Directory -ErrorAction Stop | Out-Null
    } catch {
        Write-Host "ERROR: No se pudo crear el directorio de backup. Por favor, verifica los permisos o la ruta."
        exit 1
    }
}

if (-not (Test-Path -Path $MysqldumpPath)) {
    Write-Host "ERROR: No se encontró mysqldump.exe en la ruta: $MysqldumpPath"
    exit 1
}

# --- Generación de Rutas ---
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFileName = "${DbName}_backup_${timestamp}.sql"
$backupFilePath = Join-Path -Path $BackupDir -ChildPath $backupFileName
$zipFilePath = "${backupFilePath}.zip"
$errorLogPath = Join-Path -Path $BackupDir -ChildPath "mysqldump_error.log"

# --- Realizar el Backup ---
Write-Host "Iniciando backup de la base de datos '$DbName' a '$backupFilePath'..."

# Construir argumentos para mysqldump. Evitar -p en favor de --password para claridad.
# Importante: El uso de --password puede ser inseguro. Una mejor práctica es usar archivos de configuración de MySQL (my.cnf/my.ini).
$dumpArgs = @(
    "--host=$DbHost",
    "--port=$DbPort",
    "--user=$DbUser",
    "--password=$DbPassword",
    "--single-transaction",
    "--result-file=$backupFilePath", # Redirige la salida directamente al archivo
    $DbName
)

try {
    # Usar Start-Process para un mejor control y captura de errores
    $process = Start-Process -FilePath $MysqldumpPath -ArgumentList $dumpArgs -Wait -PassThru -RedirectStandardError $errorLogPath
    
    # Comprobar el código de salida del proceso de mysqldump
    if ($process.ExitCode -ne 0) {
        $errorLogContent = Get-Content $errorLogPath -Raw
        # Filtrar la advertencia de contraseña común para no tratarla como un error fatal si es el único mensaje.
        $realError = $errorLogContent | Where-Object { $_ -notlike "*Using a password on the command line*" }
        if ($realError) {
             Write-Host "ERROR: El proceso de mysqldump falló. Código de salida: $($process.ExitCode)"
             Write-Host "Mensaje de error:"
             Write-Host $errorLogContent
             Remove-Item -Path $backupFilePath -ErrorAction SilentlyContinue
             exit 1
        }
    }
    
    # Doble verificación: asegurar que el archivo de backup no esté vacío.
    if ((Get-Item $backupFilePath).Length -eq 0) {
        Write-Host "ERROR: El archivo de backup se creó pero está vacío."
        $errorLogContent = Get-Content $errorLogPath -Raw
        if($errorLogContent) { Write-Host $errorLogContent }
        Remove-Item -Path $backupFilePath -ErrorAction SilentlyContinue
        exit 1
    }

    Write-Host "Backup completado exitosamente: $backupFilePath"

} catch {
    Write-Host "ERROR: Ocurrió una excepción al ejecutar mysqldump."
    Write-Host $_.Exception.Message
    if (Test-Path $errorLogPath) { Get-Content $errorLogPath -Raw }
    exit 1
} finally {
     if (Test-Path $errorLogPath) { Remove-Item $errorLogPath -ErrorAction SilentlyContinue }
}

# --- Compresión ---
if ($PSVersionTable.PSVersion.Major -lt 5) {
    Write-Host "ADVERTENCIA: Se requiere PowerShell 5.0 o superior para comprimir. Saltando este paso."
} else {
    Write-Host "Comprimiendo archivo de backup a '$zipFilePath'..."
    try {
        Compress-Archive -LiteralPath $backupFilePath -DestinationPath $zipFilePath -Force -ErrorAction Stop
        Write-Host "Compresión exitosa."
        # Eliminar el archivo .sql original si la compresión fue exitosa
        Remove-Item -LiteralPath $backupFilePath
        Write-Host "Archivo .sql original eliminado."
    } catch {
        Write-Host "ERROR: Falló la compresión."
        Write-Host $_.Exception.Message
        exit 1
    }
}

Write-Host "Proceso de backup finalizado."
exit 0
