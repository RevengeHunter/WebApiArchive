# Configuración Inicial del Proyecto

Este proyecto requiere configurar los parámetros de entorno locales antes de poder ejecutarse. Sigue estos pasos para la configuración inicial:

## Paso 1: Configurar el archivo de ajustes

1. Busca el archivo `appsettings.json.sample` en la raíz del proyecto.
2. Crea una copia de ese archivo en la misma carpeta.
3. Cambia el nombre de la copia eliminando el sufijo `.sample`. El nuevo archivo debe llamarse exactamente:
   ```text
   appsettings.json
   ```

## Paso 2: Configurar la Base de Datos

1. Abre el nuevo archivo `appsettings.json` en tu editor de código.
2. Localiza la sección de la cadena de conexión (usualmente bajo `ConnectionStrings`).
3. Modifica los valores con los datos correspondientes a tu servidor de base de datos local.

---
⚠️ **Nota importante:** El archivo `appsettings.json` contiene credenciales locales y está excluido del control de versiones para proteger la seguridad del proyecto. Asegúrate de añadir `appsettings.json` a tu archivo `.gitignore` y dejar únicamente el archivo `.sample` expuesto públicamente en [GitHub](https://github.com).