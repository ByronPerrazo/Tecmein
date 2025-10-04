### **Plan de Implementación y Puesta en Producción: Sistema Tecmein en Windows Server**

**Objetivo:** Detallar todos los requerimientos y pasos necesarios para desplegar la aplicación Tecmein de forma segura, eficiente y robusta en un servidor local (on-premise) basado en Windows Server.

---

### **1. Requerimientos de Infraestructura (Hardware)**

Se presentan dos configuraciones para el servidor físico o virtual.

| Componente | Configuración Recomendada (Óptima) | Configuración Mínima (Viable) |
| :--- | :--- | :--- |
| **CPU** | Procesador de 6-8 núcleos (Intel Xeon E-series / AMD EPYC) | Procesador de 4 núcleos / 8 hilos (Intel Core i5/i7 / AMD Ryzen 5) |
| **RAM** | **32 GB DDR4 ECC** | **16 GB DDR4** |
| **Almacenamiento** | **Disco 1 (SO/App):** 512 GB NVMe SSD<br>**Disco 2 (DB):** 1 TB NVMe SSD | 1 TB NVMe SSD (Particionado si es posible) |
| **Red** | Doble puerto de red 1 Gbps | Un puerto de red 1 Gbps |
| **SO** | Windows Server 2022 Standard | Windows Server 2019 Standard |

---

### **2. Pila de Software Requerida**

Esta es la lista de todo el software que debe ser descargado e instalado en el servidor.

1.  **Sistema Operativo:**
    *   **Windows Server 2022 Standard** (o 2019).
2.  **Servidor Web:**
    *   **Rol de IIS (Internet Information Services)**, que se instala desde el Administrador del Servidor de Windows.
3.  **Framework de Aplicación:**
    *   **ASP.NET Core 8.0 Hosting Bundle**. Se descarga desde el sitio web de Microsoft.
4.  **Base de Datos:**
    *   **MySQL Server 8.x for Windows (Community Edition)**. Se descarga a través del "MySQL Installer".
5.  **Herramienta de Gestión de Base de Datos (Opcional):**
    *   **MySQL Workbench**. Se recomienda instalar en una estación de trabajo de un administrador, no directamente en el servidor.

---

### **3. Configuración de Seguridad (Hardening)**

La seguridad es un proceso en capas. Se deben implementar todas las siguientes medidas.

1.  **Seguridad del Sistema Operativo:**
    *   **Windows Update:** Mantener el servidor siempre actualizado.
    *   **Contraseñas:** Utilizar contraseñas largas y complejas para todas las cuentas, especialmente la de Administrador y la de `root` de MySQL.
    *   **Mínimo Privilegio:** No utilizar cuentas de administrador para tareas rutinarias.

2.  **Seguridad de Red (Perimetral y Local):**
    *   **Firewall Perimetral (Router):** Configurar **Port Forwarding (NAT)** para redirigir el tráfico de la IP pública de la empresa hacia la IP privada del servidor, únicamente en los puertos **TCP 80 (HTTP) y 443 (HTTPS)**.
    *   **Firewall del Servidor (Windows Defender):** Configurar reglas de entrada para permitir tráfico **únicamente** en los puertos **TCP 80 y 443**. El resto de los puertos, incluido el **3306 (MySQL)**, deben permanecer bloqueados para el acceso desde el exterior.

3.  **Seguridad de la Aplicación (SSL/TLS):**
    *   **Certificado SSL:** Es **obligatorio** para cifrar la comunicación. Se puede comprar a una Autoridad de Certificación (CA) o generar uno gratuito y de confianza usando **Let's Encrypt** con la herramienta `win-acme` para Windows/IIS.
    *   **Binding en IIS:** El certificado debe ser asignado (bind) al sitio web en el puerto 443.

4.  **Seguridad de la Base de Datos:**
    *   **Instalación Segura:** Durante la instalación de MySQL, establecer una contraseña de `root` muy segura.
    *   **Usuario de Aplicación:** No usar `root` en la aplicación. Crear un usuario específico para Tecmein con permisos limitados solo a su base de datos (`tecmeindb`).

---

### **4. Plan de Implementación (Paso a Paso)**

Sigue estos pasos en orden para la instalación y configuración.

1.  **Instalar Software Base:**
    *   Instala Windows Server y todas sus actualizaciones.
    *   Instala el rol de IIS desde el Administrador del Servidor.
    *   Instala el .NET 8 Hosting Bundle.
    *   Instala MySQL Server usando el MySQL Installer.

2.  **Configurar la Base de Datos:**
    *   Conéctate a MySQL y ejecuta los siguientes comandos para crear la base de datos y el usuario:
        ```sql
        CREATE DATABASE tecmeindb;
        CREATE USER 'tecmein_app'@'localhost' IDENTIFIED BY 'UNA_CONTRASENA_MUY_SEGURA_Y_UNICA';
        GRANT ALL PRIVILEGES ON tecmeindb.* TO 'tecmein_app'@'localhost';
        FLUSH PRIVILEGES;
        ```
    *   Desde tu PC de desarrollo, genera el script de migración de la base de datos: `dotnet ef migrations script -o deploy.sql -i`.
    *   Ejecuta el archivo `deploy.sql` en la base de datos del servidor para crear la estructura de tablas.

3.  **Desplegar la Aplicación Web:**
    *   En Visual Studio, publica el proyecto `TecmeinAplicacionWeb` en una carpeta local (Configuración: `Release`).
    *   Copia la carpeta de publicación al servidor en `C:\inetpub\wwwroot\tecmein`.
    *   En el Administrador de IIS, crea un nuevo sitio web:
        *   **Nombre:** `tecmein`
        *   **Ruta física:** `C:\inetpub\wwwroot\tecmein`
        *   **Binding:** Tipo `https`, Puerto `443`, Nombre de host `tecmein.tuempresa.com`, y selecciona tu certificado SSL.
    *   Ve a `Grupos de aplicaciones`, busca el de tu sitio, y en `Configuración avanzada`, establece la "Versión de .NET CLR" en **"Sin código administrado"**.
    *   Crea el archivo `appsettings.Production.json` en la carpeta raíz del sitio con la cadena de conexión final:
        ```json
        {
          "ConnectionStrings": {
            "ConexionDB": "server=localhost;port=3306;user=tecmein_app;password=LA_CONTRASENA_SEGURA_QUE_CREASTE;database=tecmeindb"
          }
        }
        ```

4.  **Verificación Final:**
    *   Accede a `https://tecmein.tuempresa.com` y verifica que la aplicación carga y funciona correctamente.

---

### **5. Configuración de Mantenimiento y Backups**

Un sistema en producción no está completo sin un plan de respaldo.

1.  **Backup de la Base de Datos (Diario):**
    *   **Estrategia:** Realizar un backup completo de la base de datos todos los días en un horario de baja actividad (ej. 3:00 AM).
    *   **Herramienta:** `mysqldump.exe` y el Programador de Tareas de Windows.
    *   **Pasos:**
        1.  Crea una carpeta para los backups, ej. `C:\Backups`.
        2.  Crea un archivo de script llamado `backup_db.bat` en `C:\Scripts` con el siguiente contenido (reemplaza la contraseña de root):
            ```batch
            @echo off
            "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe" -u root -pTU_PASSWORD_ROOT --databases tecmeindb > "C:\Backups\tecmeindb_backup_%DATE:~10,4%-%DATE:~4,2%-%DATE:~7,2%.sql"
            ```
        3.  Abre el **Programador de Tareas**, crea una nueva tarea que se ejecute diariamente y que su acción sea iniciar el programa `C:\Scripts\backup_db.bat`.

2.  **Backup de los Archivos de la Aplicación:**
    *   **Estrategia:** Realizar un backup de los archivos de la aplicación después de cada nuevo despliegue.
    *   **Método:** Comprimir la carpeta `C:\inetpub\wwwroot\tecmein` en un archivo `.zip` y guardarlo en una ubicación segura.

3.  **Almacenamiento de Backups:**
    *   **CRÍTICO:** Los backups no deben residir únicamente en el mismo servidor. Implementa una política para copiar los archivos de backup (`.sql` y `.zip`) a una ubicación externa: un NAS, otro servidor, o un servicio de almacenamiento en la nube.

---

Este plan integral proporciona todos los elementos necesarios para una puesta en producción exitosa y segura del sistema Tecmein.
