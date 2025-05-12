import os
import re
import traceback
import sys
import csv
import mysql.connector
from mysql.connector import Error

# Función que configura la conexión de MySQL
def conexionMySQL():
    # Configuración de la conexión MySQL
    mysql_config = {
        'host': 'localhost',
        'user': 'gsmao',
        'password': 'Suit3l3c.HitaCor!',
        'database': 'demo_gsmao_migracion',
        "connect_timeout": 300
    }

    return mysql_config

# Función para abrir el fichero en modo lectura
def leer_fichero_sql(fichero):
    try:
        ruta_fichero = obtener_ruta('DumpFolder' if fichero != "limpiar_BBDD.sql" else 'Scripts', fichero)

        with open(ruta_fichero, 'r', encoding='utf-8') as file:
            contenido = file.read()

        return contenido
    except Exception:
        print(f"\n\nError al leer el fichero\n{ruta_fichero}\nExcepcion: {traceback.print_exc()}")
        print("\n\nERROR INESPERADO. TERMINANDO LA EJECUCIÓN.\nPresione cualquier tecla para finalizar...")
        sys.exit(1)  # Detener la ejecución con un código de error

# Función que obtiene la ruta completa del fichero
def obtener_ruta(carpeta, ruta_fichero):
    directorio_actual = os.path.dirname(os.path.abspath(__file__))

    # Subir un nivel desde el directorio actual
    carpeta_superior = os.path.dirname(directorio_actual)
    ruta_raiz = os.path.join(carpeta_superior, carpeta)

    # Comprobar que la ruta para los ficheros modificados exista, si no, crearla
    if not os.path.exists(ruta_raiz):
        os.makedirs(ruta_raiz)

    return os.path.join(ruta_raiz, ruta_fichero)

# Función que genera un script de INSERT para la tabla Empresas
def generarScript(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        # Incluir linea para bloquear y desbloquear tablas
        if "LOCK TABLES" in linea or "INSERT INTO" in linea or "UNLOCK TABLES" in linea:
            # Incluir estas líneas tal y cómo están
            lineas_modificadas.append(linea)
    
    return '\n'.join(lineas_modificadas)

# Función que incluye en la consulta las columnas que se están insertando
def Plantas(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`plantas` (Id, Descripcion, StmpConfig, Latitud, Longitud, IdEmpresa)', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que incluye en la consulta las columnas que se están insertando
def Componentes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`componentes` (Id, Denominacion, DescripcionES, DescripcionEN, IdComponentePadre)', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que incluye en la consulta las columnas que se están insertando
def Incidencias(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`incidencias` (Id, DescripcionES, DescripcionEN, IdMecanismoFallo)', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el orden del centro de coste SAP
def CentrosCostes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            # Modificar el orden del centro de coste SAP
            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = sentencia.split(',')
                centroCosteSAP = valores.pop(1)
                valores.insert(3, centroCosteSAP)
                valores_modificados.append(f"({','.join(valores)})")

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = f"INSERT INTO `centrosdecostes` (Id, DescripcionES, DescripcionEN, CentroCosteSAP, IdPlanta) VALUES " + valores_modificados[0]

            lineas_modificadas.append({','.join(valores_modificados) + ';'})
        else:
            # Incluir estas líneas tal y cómo están
            lineas_modificadas.append(linea)
    
    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el orden de la localizacion SAP
def Localizaciones(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            # Modificar el orden de la localizacion SAP
            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = sentencia.split(',')
                valores_modificados.append(f"({','.join(valores)})")

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = f"INSERT INTO `localizaciones` (Id, DescripcionES, DescripcionEN, LocalizacionSAP, Latitud, Longitud, ContactoRepuestos, IdPlanta) VALUES " + valores_modificados[0]

            lineas_modificadas.append({','.join(valores_modificados) + ';'})
        else:
            # Incluir estas líneas tal y cómo están
            lineas_modificadas.append(linea)
    
    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla TiposIncidencias
def MecanismosDeFallo(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "LOCK TABLE" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`mecanismosdefallo`', linea))
        elif "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`mecanismosdefallo` (Id, DescripcionES, DescripcionEN)', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla Estados
def EstadosOrden(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "LOCK TABLE" in linea or "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`estadosorden`', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla Estados
def UsuariosOrdenes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        linea_insertar = ""
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = sentencia.split(',')
                valores[0] = '"' + modificar_Usuario(valores[0], '') + '"'

                valor_insertar = "(" + ",".join(valores) + ")"
                valores_modificados.append(valor_insertar)

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = "INSERT INTO `usuarios_ordenes` VALUES " + valores_modificados[0]

            linea_insertar = ",".join(valores_modificados) + ';'
            lineas_modificadas.append(linea_insertar)
        else:
            if "WRITE;" in linea:
                lineas_modificadas.append(re.sub(r'`([^`]*)`', '`usuarios_ordenes`', linea))
            else:
                lineas_modificadas.append(linea)

    return "\n".join(linea for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla Estados
def HistorialUsuariosOrdenes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        linea_insertar = ""
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = sentencia.split(',')
                if valores[3] != 'NULL':
                    valores[3] = '"' + modificar_Usuario(valores[3], '') + '"'
                if valores[4] != 'NULL':
                    valores[4] = '"' + modificar_Usuario(valores[4], '') + '"'

                valor_insertar = "(" + ",".join(valores) + ")"
                valores_modificados.append(valor_insertar)

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = "INSERT INTO `historialcambiosusuariosordenes` VALUES " + valores_modificados[0]

            linea_insertar = ",".join(valores_modificados) + ';'
            lineas_modificadas.append(linea_insertar)
        else:
            if "WRITE;" in linea:
                lineas_modificadas.append(re.sub(r'`([^`]*)`', '`historialcambiosusuariosordenes`', linea))
            else:
                lineas_modificadas.append(linea)

    return "\n".join(linea for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla TiposOrdenes
def TiposOrdenes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "LOCK TABLE" in linea or "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`tiposorden`', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que cambia el nombre de la tabla TiposIncidencias
def Resoluciones(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        if "INSERT INTO" in linea:
            lineas_modificadas.append(re.sub(r'`([^`]*)`', '`resoluciones` (Id, DescripcionES, DescripcionEN)', linea))
        else:
            lineas_modificadas.append(linea)

    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que modificar los tipos de datos de los activos y los cambia según los diccionarios correspondientes
def Activos(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = list(csv.reader([sentencia], quotechar="'", skipinitialspace=True))[0]

                for i in range(2, 4):
                    valores[i] = "'" + valores[i] + "'"

                for i in range(4, 8):
                    if valores[i] == "" or valores[i] == "NULL":
                        valores[i] = "0"
                    else:
                        valores[i] = valores[i].strip("'")

                if valores[8] == "NULL" or valores[8] == "":
                    valores[8] = "0"
                else:
                    valores[8] = valores[8].replace('%', '').strip("'")  # Modificar valor de valor_criticidad
                
                valores[9] = modificar_IdCriticidad(valores[9])  # Modificar valor de criticidad
                valores[13] = modificar_Actividad(valores[13])  # Modificar valor de actividad
                valores.pop(12)  # Eliminamos el valor de nivel_maximo

                valores_modificados.append(f"({','.join(valores)})")

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = "INSERT INTO `activos` (Id, ActivoSAP, DescripcionES, DescripcionEN, Redundancia, Hse, Usabilidad, Coste, ValorCriticidad, IdCriticidad, IdLocalizacion, IdCentroCoste, IdEstadoActivo) VALUES " + valores_modificados[0]

            lineas_modificadas.append({','.join(valores_modificados) + ';'})
        else:
            # Incluir estas líneas tal y cómo están
            lineas_modificadas.append(linea)
    
    return '\n'.join(str(linea).strip('{""}') for linea in lineas_modificadas)

# Función que modifica los datos para insertar en la tabla Ordenes
def Ordenes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        linea_insertar = ""
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = list(csv.reader([sentencia], quotechar="'", skipinitialspace=True))[0]

                for i in range(1, 8):
                    if valores[i] != "NULL":
                        valores[i] = '"' + valores[i] + '"'

                if valores[13] != "NULL":
                    valores[13] = '"' + valores[13] + '"'

                valores[11] = '"' + modificar_Usuario(valores[11], 'creador') + '"'
                valores.pop(14)  # Eliminamos el valor de fecha_anulacion

                valor_insertar = "(" + ",".join(valores) + ")"
                valores_modificados.append(valor_insertar)

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = "INSERT INTO `ordenes` (Id, IdSAP, FechaCreacion, FechaApertura, ComentarioOrden, FechaCierre, ComentarioResolucion, TiempoParada, Confirmada, IdActivo, IdEstadoOrden, IdUsuarioCreador, IdTipoOrden, Materiales) VALUES " + valores_modificados[0]

            linea_insertar = ",".join(valores_modificados) + ';'
            lineas_modificadas.append(linea_insertar)
        else:
            # Incluir estas líneas tal y cómo están
            lineas_modificadas.append(linea)
    
    return "\n".join(linea for linea in lineas_modificadas)

# Función que modifica los datos para insertar en la tabla IncidenciasOrden
def IncidenciasOrdenes(script_sql):
    lineas_modificadas = []
    for linea in script_sql.splitlines():
        linea_insertar = ""
        # Incluir linea para bloquear y desbloquear tablas
        if "INSERT INTO" in linea:
            bandera = 0

            sentencias = re.findall(r"\(([^()]*?(?:'[^']*'[^()]*)*?)\)", linea)
            valores_modificados = []
            for sentencia in sentencias:
                if bandera == 0:
                    bandera = 1

                valores = list(csv.reader([sentencia], quotechar="'", skipinitialspace=True))[0]

                valores.pop(0)  # Eliminamos el valor de Id Auto_increment
                valores.pop(-4)  # Eliminamos el valor de repuesto_id

                if valores[0] != "NULL":
                    valores[0] = "'" + valores[0] + "'"

                if valores[5] != "NULL":
                    valores[5] = "'" + valores[5] + "'"

                valor_insertar = "(" + ",".join(valores) + ")"
                valores_modificados.append(valor_insertar)

                if bandera == 1:
                    bandera = 2
                    valores_modificados[0] = "INSERT INTO `incidenciasordenes` (FechaDeteccion, IdOrden, IdComponente, IdIncidencia, IdResolucion, FechaResolucion, ParoMaquina, CambioPieza, AfectaProduccion) VALUES " + valores_modificados[0]

            linea_insertar = ",".join(valores_modificados) + ';'
            lineas_modificadas.append(linea_insertar)
        else:
            if "WRITE;" in linea:
                lineas_modificadas.append(re.sub(r'`([^`]*)`', '`incidenciasordenes`', linea))
            else:
                lineas_modificadas.append(linea)
    
    return "\n".join(linea for linea in lineas_modificadas)

# Función que sustituye los datos de Criticidad por el IdCriticidad
def modificar_IdCriticidad(valor):
    valoresCriticidad = declararReemplazosCriticidad()
    return valoresCriticidad[valor]

# Función que sustituye los datos de Actividad por el IdEstadoActivo
def modificar_Actividad(valor):
    valoresActividad = declararReemplazosActividad()
    return valoresActividad[valor]

# Función que sustituye los datos de Creador por el IdUsuarioCreador
def modificar_Usuario(valor, campo):
    valoresUsuarios = declararReemplazosUsuarios(campo)
    return valoresUsuarios[valor]

# Función que declara el listado de reemplazos para la criticidad
def declararReemplazosCriticidad():
    # Diccionario de palabras a reemplazar para criticidad
    reemplazos = {
        "MC": '1',
        "CA": '2',
        "CM": '3',
        "CB": '4',
        "SC": '5',
        "NULL": '5'
    }

    return reemplazos

# Función que declara el listado de reemplazos para la actividad
def declararReemplazosActividad():
    # Diccionario de palabras a reemplazar para actividad
    reemplazos = {
        '0': '1',
        '1': '2',
        '2': '3'
    }

    return reemplazos

# Función que declara el listado de reemplazos para los identificadores de los usuarios
def declararReemplazosUsuarios(campo):
    if campo == 'creador':
        # Diccionario de palabras a reemplazar para usuarios creadores
        reemplazos = {
            '1': '81e47e59-a3d5-4c7b-b3f7-077da98a369f',
            '2': 'eedc8529-76e8-4dfc-93c8-1dd0dbfa85d1',
            '3': 'bf368d2b-7199-47ad-8524-3f3db3e9461a',
            '4': 'e3649caf-53ee-440b-b887-cae785448c25',
            '5': 'b63bc715-6fb6-4121-8877-9ccc477614b3',
            '6': 'd350b266-910c-4262-8b87-6fcebe3ce465',
            '7': 'bf73fcbe-ba8f-4fc3-8f71-ad28157b5dca',
            '8': '73776e05-11d6-4b04-9c4a-09e00ed90965',
            '9': 'e1fc54bd-605c-4e6b-a314-b10e1f1b073e',
            '10': '83fe4b80-5932-4aed-87e4-bd75d27ce694',
            '11': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '12': '500698e0-5b0a-4e26-88df-957eb018696b',
            '13': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '15': '59d2ef0d-a562-4d56-8429-aeddaf35a546',
            '16': '13525e67-6853-438d-984a-7a6b647aebd9',
            '17': '492b03f9-3bd8-4396-a20f-7eb8d1dcd364',
            '18': '562559aa-fbc4-4f82-a533-224e3d5f8c34',
            '19': 'dec0f3d2-278f-4e5a-8763-d3e275dff091',
            '20': 'ac8fa91b-013f-4f67-aa45-ce6f2cd33339',
            '21': 'c88f4e63-1597-4d80-afda-f8e9906516c5',
            '22': '6b2d48b4-eb40-4c86-b728-330b69bef14f',
            '23': '5a602495-d87f-4823-932e-193484a939df',
            '24': '6078c069-2d1f-44b4-b419-e2ff12d9af05',
            '25': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '26': '1d781f50-7fbc-4109-a715-94ca9cde1b5e',
            '27': 'e94641cf-3886-48a4-8db5-f5bb82ac3fa2',
            '28': 'd6a07a3b-c844-40df-8f44-da616cccd16e',
            '29': '4242dc66-15d0-42aa-bc8c-a9cfaf79bb75',
            '30': 'f7840970-2a26-47b8-a5c9-1289a668ceca',
            '31': '8ba7cbaf-532f-4abd-8c99-c541071e1c0a',
            '32': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            'NULL': 'eedc8529-76e8-4dfc-93c8-1dd0dbfa85d1'
        }
    else:        
        # Diccionario de palabras a reemplazar para usuarios no creadores
        reemplazos = {
            '1': '81e47e59-a3d5-4c7b-b3f7-077da98a369f',
            '2': 'eedc8529-76e8-4dfc-93c8-1dd0dbfa85d1',
            '3': 'bf368d2b-7199-47ad-8524-3f3db3e9461a',
            '4': 'e3649caf-53ee-440b-b887-cae785448c25',
            '5': 'b63bc715-6fb6-4121-8877-9ccc477614b3',
            '6': 'd350b266-910c-4262-8b87-6fcebe3ce465',
            '7': 'bf73fcbe-ba8f-4fc3-8f71-ad28157b5dca',
            '8': '73776e05-11d6-4b04-9c4a-09e00ed90965',
            '9': 'e1fc54bd-605c-4e6b-a314-b10e1f1b073e',
            '10': '83fe4b80-5932-4aed-87e4-bd75d27ce694',
            '11': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '12': '500698e0-5b0a-4e26-88df-957eb018696b',
            '13': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '14': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '15': '59d2ef0d-a562-4d56-8429-aeddaf35a546',
            '16': '13525e67-6853-438d-984a-7a6b647aebd9',
            '17': '492b03f9-3bd8-4396-a20f-7eb8d1dcd364',
            '18': '562559aa-fbc4-4f82-a533-224e3d5f8c34',
            '19': 'dec0f3d2-278f-4e5a-8763-d3e275dff091',
            '20': 'ac8fa91b-013f-4f67-aa45-ce6f2cd33339',
            '21': 'c88f4e63-1597-4d80-afda-f8e9906516c5',
            '22': '6b2d48b4-eb40-4c86-b728-330b69bef14f',
            '23': '5a602495-d87f-4823-932e-193484a939df',
            '24': '6078c069-2d1f-44b4-b419-e2ff12d9af05',
            '25': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829',
            '26': '1d781f50-7fbc-4109-a715-94ca9cde1b5e',
            '27': 'e94641cf-3886-48a4-8db5-f5bb82ac3fa2',
            '28': 'd6a07a3b-c844-40df-8f44-da616cccd16e',
            '29': '4242dc66-15d0-42aa-bc8c-a9cfaf79bb75',
            '30': 'f7840970-2a26-47b8-a5c9-1289a668ceca',
            '31': '8ba7cbaf-532f-4abd-8c99-c541071e1c0a',
            '32': 'a7fac1e5-c319-433a-acbc-52dfa8fc2829'
        }

    return reemplazos

# Función que ejecuta el script de MySQL
def ejecutar_script_mysql(mysql_config, script_sql):
    """ Ejecutar un script SQL en la base de datos MySQL. """
    try:
        conexion = mysql.connector.connect(**mysql_config)
        if conexion.is_connected():
            cursor = conexion.cursor()
            for resultado in cursor.execute(script_sql, multi=True):  # multi=True para múltiples sentencias
                if resultado.with_rows:
                    print(f"Filas devueltas: {resultado.fetchall()}")
                else:
                    print(f"Afectadas: {resultado.rowcount}")
            conexion.commit()
            cursor.close()

        return True
    except Error as e:
        return(f"Error conectando a la base de datos: {e}")

    finally:
        if conexion.is_connected():
            conexion.close()