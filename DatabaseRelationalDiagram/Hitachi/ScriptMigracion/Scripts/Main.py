import os
import traceback
import sys
import Migrations as funcs

if __name__ == "__main__":
    try:
        ficheros = [
            'limpiar_BBDD.sql',
            'hitachi_empresas.sql',
            'hitachi_plantas.sql',
            'hitachi_centrosdecostes.sql',
            'hitachi_localizaciones.sql',
            'hitachi_tiposincidencias.sql',
            'hitachi_incidencias.sql',
            'hitachi_resoluciones.sql',
            'hitachi_activos.sql',
            'hitachi_componentes.sql',
            'hitachi_activo_componentes.sql',
            'hitachi_estados.sql',
            'hitachi_tipos_ordenes.sql',
            'hitachi_ordenes.sql',
            'hitachi_orden_incidencias_resolucion.sql',
            'hitachi_usuarios_orden.sql',
            'hitachi_historial_modificaciones_usuarios_ordenes.sql'
        ]

        conexion = funcs.conexionMySQL()
        for fichero in ficheros:
            script_sql = funcs.leer_fichero_sql(fichero)

            # Generar documento de INSERT por cada fichero con las modificaciones pertinentes
            if fichero != 'limpiar_BBDD.sql':
                script_sql = funcs.generarScript(script_sql)

            if fichero == 'hitachi_ordenes.sql':
                script_sql = funcs.Ordenes(script_sql)

            if fichero == 'hitachi_orden_incidencias_resolucion.sql':
                script_sql = funcs.IncidenciasOrdenes(script_sql)

            if fichero == 'hitachi_usuarios_orden.sql':
                script_sql = funcs.UsuariosOrdenes(script_sql)

            if fichero == 'hitachi_historial_modificaciones_usuarios_ordenes.sql':
                script_sql = funcs.HistorialUsuariosOrdenes(script_sql)

            if fichero == 'hitachi_componentes.sql':
                script_sql = funcs.Componentes(script_sql)

            if fichero == 'hitachi_plantas.sql':
                script_sql = funcs.Plantas(script_sql)

            if fichero == 'hitachi_incidencias.sql':
                script_sql = funcs.Incidencias(script_sql)

            if fichero == 'hitachi_estados.sql':
                script_sql = funcs.EstadosOrden(script_sql)

            if fichero == 'hitachi_tipos_ordenes.sql':
                script_sql = funcs.TiposOrdenes(script_sql)

            if fichero == 'hitachi_resoluciones.sql':
                script_sql = funcs.Resoluciones(script_sql)

            if fichero == 'hitachi_centrosdecostes.sql':
                script_sql = funcs.CentrosCostes(script_sql)

            if fichero == 'hitachi_localizaciones.sql':
                script_sql = funcs.Localizaciones(script_sql)

            if fichero == 'hitachi_tiposincidencias.sql':
                script_sql = funcs.MecanismosDeFallo(script_sql)
                fichero = 'hitachi_mecanismosdefallo.sql'

            if fichero == 'hitachi_activos.sql':
                script_sql = funcs.Activos(script_sql)

            # Guardar el fichero modificado
            if fichero != "limpiar_BBDD.sql":
                ruta_guardar = funcs.obtener_ruta('FicherosModificados_Datos', fichero)
                ruta_guardar = ruta_guardar.replace('.sql', '_modificado.sql')
                with open(ruta_guardar, 'w', encoding='utf-8') as file:
                    file.write(script_sql)

            # Ejecutar el script modificado en la base de datos
            response = funcs.ejecutar_script_mysql(conexion, script_sql)
            if fichero == 'limpiar_BBDD.sql':
                print(f"Se ha terminado la ejecucion del fichero {fichero}.")
            else:
                print(f"Se ha terminado la ejecucion del fichero {os.path.basename(ruta_guardar)}.")

            if not response:
                print(response)
                sys.exit(1)  # Detener la ejecución con un código de error
            
        print("SCRIPT COMPLETADO")
    except Exception as ex:
        print(f"Excepcion: {traceback.print_exc()}")
        print()
        print("ERROR INESPERADO. TERMINANDO LA EJECUCIÓN.\nPresione cualquier tecla para finalizar...")
        sys.exit(1)  # Detener la ejecución con un código de error