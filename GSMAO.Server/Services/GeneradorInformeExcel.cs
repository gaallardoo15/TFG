using GSMAO.Server.DTOs;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using AutoMapper;
using GSMAO.Server.Database.DAOs;

namespace GSMAO.Server.Services
{
    public class GeneradorExcel
    {
        public async Task<MemoryStream> GenerarInformeExcelAsync(List<InformeExcelDTO> informe, bool antiguo)
        {
            MemoryStream buffer = new MemoryStream();
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear un nuevo archivo Excel
                ExcelPackage package = new ExcelPackage();

                ExcelWorksheet worksheet;
                var columnasTotales = 0;

                if (antiguo)
                {
                    // Agregar una nueva hoja al Excel
                    worksheet = package.Workbook.Worksheets.Add($"Datos");

                    // Definimos los encabezados
                    List<string> encabezados = new List<string>()
                    {
                        "Criticidad",                   // 0
                        "Activo_SAP",                   // 1
                        "Descripción Activo",           // 2
                        "Activo",                       // 3
                        "Centro Coste",                 // 4
                        "Orden SAP",                    // 5
                        "Id Orden",                     // 6
                        "Comentario",                   // 7
                        "Tipo Orden",                   // 8
                        "Fecha Inicio Orden",           // 9
                        "Hora Inicio Orden",            // 10
                        "Fecha Fin Orden",              // 11
                        "Hora Fin Orden",               // 12
                        "Estado",                       // 13
                        "Nivel 1",                      // 14
                        "Componente",                   // 15
                        "KKS",                          // 16
                        "Mecanismo Fallo",              // 17
                        "Incidencia",                   // 18
                        "Resolución",                   // 19
                        "Fecha Inicio Incidencia",      // 20
                        "Hora Inicio Incidencia",       // 21
                        "Fecha Resolución Incidencia",  // 22
                        "Hora Resolución Incidencia",   // 23
                        "Tiempo Parada",                // 24
                        "Materiales",                   // 25
                        "Comentario Resolución",        // 26
                        "Usuarios",                     // 27
                        "Cambio Pieza",                 // 28
                        "Afecta Producción",            // 29
                        "Paro Máquina"                  // 30
                    };

                    // Rellenamos los encabezados en el fichero
                    for (int i = 0; i < encabezados.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = encabezados[i];
                        worksheet.Cells[1, i + 1].Style.Font.Size = 12;
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    List<string> Jerarquia = new List<string>();
                    int j;
                    for (j = 0; j < informe.Count; j++)
                    {
                        Jerarquia = informe[j].JerarquiaComponente.Split(" > ").ToList();

                        worksheet.Cells[j + 2, 1].Value = informe[j].Criticidad;
                        worksheet.Cells[j + 2, 2].Value = informe[j].ActivoSAP;
                        worksheet.Cells[j + 2, 3].Value = informe[j].Activo;
                        worksheet.Cells[j + 2, 4].Value = informe[j].IdActivo;
                        worksheet.Cells[j + 2, 5].Value = informe[j].CentroCosteSAP + "-" + informe[j].CentroCoste;
                        worksheet.Cells[j + 2, 6].Value = informe[j].IdSAP != null && informe[j].IdSAP != "" ? informe[j].IdSAP : "-";
                        worksheet.Cells[j + 2, 7].Value = informe[j].Id;
                        worksheet.Cells[j + 2, 8].Value = informe[j].ComentarioOrden;
                        worksheet.Cells[j + 2, 9].Value = informe[j].TipoOrden;
                        worksheet.Cells[j + 2, 10].Value = informe[j].FechaApertura.ToString("dd/MM/yyyy") ?? "-";
                        worksheet.Cells[j + 2, 11].Value = informe[j].FechaApertura.ToString("HH:mm:ss") ?? "-";
                        worksheet.Cells[j + 2, 12].Value = informe[j].FechaCierre?.ToString("dd/MM/yyyy") ?? "-";
                        worksheet.Cells[j + 2, 13].Value = informe[j].FechaCierre?.ToString("HH:mm:ss") ?? "-";
                        worksheet.Cells[j + 2, 14].Value = informe[j].EstadoOrden;
                        worksheet.Cells[j + 2, 15].Value = Jerarquia[0];
                        worksheet.Cells[j + 2, 16].Value = informe[j].Componente;
                        worksheet.Cells[j + 2, 17].Value = informe[j].KKSComponente;
                        worksheet.Cells[j + 2, 18].Value = informe[j].MecanismoDeFallo;
                        worksheet.Cells[j + 2, 19].Value = informe[j].Incidencia;
                        worksheet.Cells[j + 2, 20].Value = informe[j].Resolucion?.ToString() ?? "-";
                        worksheet.Cells[j + 2, 21].Value = informe[j].FechaDeteccion.ToString("dd/MM/yyyy") ?? "-";
                        worksheet.Cells[j + 2, 22].Value = informe[j].FechaDeteccion.ToString("HH:mm:ss") ?? "-";
                        worksheet.Cells[j + 2, 23].Value = informe[j].FechaResolucion?.ToString("dd/MM/yyyy") ?? "-";
                        worksheet.Cells[j + 2, 24].Value = informe[j].FechaResolucion?.ToString("HH:mm:ss") ?? "-";
                        worksheet.Cells[j + 2, 25].Value = informe[j].TiempoParadaIncidencia.HasValue 
                                                                ? Convert.ToDouble(informe[j].TiempoParadaIncidencia!.Value) 
                                                                : (double?)null;
                        worksheet.Cells[j + 2, 26].Value = informe[j].Materiales?.ToString() ?? "-";
                        worksheet.Cells[j + 2, 27].Value = informe[j].ComentarioResolucion?.ToString() ?? "-";
                        worksheet.Cells[j + 2, 28].Value = informe[j].Usuarios;
                        worksheet.Cells[j + 2, 29].Value = informe[j].CambioPieza ? "Sí" : "No";
                        worksheet.Cells[j + 2, 30].Value = informe[j].AfectaProduccion ? "Sí" : "No";
                        worksheet.Cells[j + 2, 31].Value = informe[j].ParoMaquina ? "Sí" : "No";
                    }

                    // Ajustar automáticamente el ancho de todas las columnas
                    worksheet.Columns.Width = 30;

                    // Establecer el ancho máximo para las columnas de comentario y comentario resolución
                    worksheet.Column(8).Width = 80; // Columna de Comentario
                    worksheet.Column(26).Width = 80; // Columna de Materiales
                    worksheet.Column(27).Width = 80; // Columna de Comentario Resolución
                    worksheet.Column(28).Width = 50; // Columna de Usuarios
                    worksheet.Column(21).Width = 20; // Columnas de FechaDeteccion
                    worksheet.Column(22).Width = 20; // Columnas de HoraDeteccion
                    worksheet.Column(23).Width = 20; // Columnas de FechaResolucion
                    worksheet.Column(24).Width = 20; // Columnas de HoraResolucion
                    worksheet.Column(25).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(10).Width = 20; // Columnas de FechaApertura
                    worksheet.Column(11).Width = 20; // Columnas de HoraApertura
                    worksheet.Column(12).Width = 20; // Columnas de FechaCierre
                    worksheet.Column(13).Width = 20; // Columnas de HoraCierre

                    worksheet.Cells[2, 21, j + 2, 24].Style.Numberformat.Format = "dd/mm/yyyy HH:mm:ss";
                    worksheet.Cells[2, 10, j + 2, 13].Style.Numberformat.Format = "dd/mm/yyyy HH:mm:ss";

                    columnasTotales = 31;
                }
                else
                {
                    // Agregar una nueva hoja al Excel
                    worksheet = package.Workbook.Worksheets.Add($"Informe_{DateTime.Today.ToString("dd/MM/yyyy")}");

                    // Rellenamos las filas en el fichero
                    List<ComponenteDTO> jerarquiaAux = new List<ComponenteDTO>();
                    var niveles = informe.Select(i => i.NivelesComponente);
                    var maxNiveles = niveles.Max();

                    // Definimos los encabezados
                    List<string> EncabezadosNivelesInsertar = new List<string>();
                    for (int f = 1; f <= maxNiveles; f++)
                    {
                        EncabezadosNivelesInsertar.Add($"Componente Nivel {f}");
                    }

                    List<string> encabezados = new List<string>()
                    {
                        "Activo",                       // 0
                        "Activo SAP",                   // 1
                        "Criticidad",                   // 2
                        "Centro de coste",              // 3
                        "Orden",                        // 4
                        "Orden SAP",                    // 5
                        "Tipo orden",                   // 6
                        "Estado orden",                 // 7
                        "Fecha apertura",               // 8
                        "Fecha cierre",                 // 9
                        "Tiempo parada orden",          // 10
                        "Usuarios",                     // 11
                        "Mecanismo de fallo",           // 12
                        "Incidencia",                   // 13
                        "Resolución",                   // 14
                        "Fecha detección",              // 15
                        "Fecha resolución",             // 16
                        "Tiempo parada incidencia",     // 17
                        "Paro máquina",                 // 18
                        "Afecta a producción",          // 19
                        "Cambio pieza",                 // 20
                        "Materiales",                   // 21
                        "Comentario orden",             // 22
                        "Comentario resolución"         // 23
                    };
                    encabezados.InsertRange(15, EncabezadosNivelesInsertar);

                    // Rellenamos los encabezados en el fichero
                    for (int i = 0; i < encabezados.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = encabezados[i];
                        worksheet.Cells[1, i + 1].Style.Font.Size = 12;
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    List<string> Jerarquia = new List<string>();
                    int j;
                    for (j = 0; j < informe.Count; j++)
                    {
                        worksheet.Cells[j + 2, 1].Value = informe[j].IdActivo + " - " + informe[j].Activo;
                        worksheet.Cells[j + 2, 2].Value = informe[j].ActivoSAP;
                        worksheet.Cells[j + 2, 3].Value = informe[j].Criticidad;
                        worksheet.Cells[j + 2, 4].Value = informe[j].CentroCosteSAP + "-" + informe[j].CentroCoste;
                        worksheet.Cells[j + 2, 5].Value = informe[j].Id;
                        worksheet.Cells[j + 2, 6].Value = informe[j].IdSAP != null && informe[j].IdSAP != "" ? informe[j].IdSAP : "-";
                        worksheet.Cells[j + 2, 7].Value = informe[j].TipoOrden;
                        worksheet.Cells[j + 2, 8].Value = informe[j].EstadoOrden;
                        worksheet.Cells[j + 2, 9].Value = informe[j].FechaApertura;
                        worksheet.Cells[j + 2, 10].Value = informe[j].FechaCierre?.ToString() ?? "-";
                        worksheet.Cells[j + 2, 11].Value = informe[j].TiempoParadaOrden.HasValue 
                                                            ? Convert.ToDouble(informe[j].TiempoParadaOrden!.Value) 
                                                            : (double?)null;
                        worksheet.Cells[j + 2, 12].Value = informe[j].Usuarios;
                        worksheet.Cells[j + 2, 13].Value = informe[j].MecanismoDeFallo;
                        worksheet.Cells[j + 2, 14].Value = informe[j].Incidencia;
                        worksheet.Cells[j + 2, 15].Value = informe[j].Resolucion?.ToString() ?? "-";

                        int col = 16;
                        int k;
                        Jerarquia = informe[j].JerarquiaComponente.Split(" > ").ToList();
                        for (k = 0; k < maxNiveles; k++)
                        {
                            if (k >= Jerarquia.Count)
                                worksheet.Cells[j + 2, col + k].Value = "-";
                            else
                                worksheet.Cells[j + 2, col + k].Value = Jerarquia[k];
                        }
                        col += maxNiveles;

                        worksheet.Cells[j + 2, col].Value = informe[j].FechaDeteccion;
                        worksheet.Cells[j + 2, col + 1].Value = informe[j].FechaResolucion?.ToString() ?? "-";
                        worksheet.Cells[j + 2, col + 2].Value = informe[j].TiempoParadaIncidencia.HasValue 
                                                                ? Convert.ToDouble(informe[j].TiempoParadaIncidencia!.Value) 
                                                                : (double?)null;
                        worksheet.Cells[j + 2, col + 3].Value = informe[j].ParoMaquina ? "Sí" : "No";
                        worksheet.Cells[j + 2, col + 4].Value = informe[j].AfectaProduccion ? "Sí" : "No";
                        worksheet.Cells[j + 2, col + 5].Value = informe[j].CambioPieza ? "Sí" : "No";
                        worksheet.Cells[j + 2, col + 6].Value = informe[j].Materiales?.ToString() ?? "-";
                        worksheet.Cells[j + 2, col + 7].Value = informe[j].ComentarioOrden?.ToString() ?? "-";
                        worksheet.Cells[j + 2, col + 8].Value = informe[j].ComentarioResolucion?.ToString() ?? "-";
                    }

                    // Ajustar automáticamente el ancho de todas las columnas
                    worksheet.Columns.Width = 30;

                    // Establecer el ancho máximo para las columnas de comentario y comentario resolución
                    worksheet.Column(16 + maxNiveles + 7).Width = 80; // Columna de Comentario
                    worksheet.Column(16 + maxNiveles + 6).Width = 80; // Columna de Materiales
                    worksheet.Column(16 + maxNiveles + 8).Width = 80; // Columna de Comentario Resolución
                    worksheet.Column(12).Width = 50; // Columna de Usuarios
                    worksheet.Column(16 + maxNiveles).Width = 20; // Columnas de FechaDeteccion
                    worksheet.Column(16 + maxNiveles + 1).Width = 20; // Columnas de FechaResolucion
                    worksheet.Column(9).Width = 20; // Columnas de FechaApertura
                    worksheet.Column(10).Width = 20; // Columnas de FechaCierre
                    worksheet.Column(11).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(16 + maxNiveles + 2).Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells[2, 16 + maxNiveles, j + 2, 16 + maxNiveles + 1].Style.Numberformat.Format = "dd/mm/yyyy HH:mm:ss";
                    worksheet.Cells[2, 9, j + 2, 10].Style.Numberformat.Format = "dd/mm/yyyy HH:mm:ss";

                    columnasTotales = 16 + maxNiveles + 8;
                }

                // Ajustar el texto a varias líneas
                worksheet.Cells.Style.WrapText = true;

                // Establecer el alineamiento horizontal en el centro para todas las celdas
                worksheet.Cells[worksheet.Dimension.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                using (ExcelRange range = worksheet.Cells[1, 1, 1, columnasTotales])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Guardar el libro de Excel en memoria
                await Task.Run(() => package.SaveAs(buffer));

                // Descargar el archivo Excel
                buffer.Position = 0;
                return buffer;
            }
            catch (Exception ex)
            {
                //LOGGEAR ERROR EX COMPLETO
                throw new InvalidOperationException($"Error al generar el informe de Excel: {ex.Message}");
            }
        }
    
        public async Task<MemoryStream> GenerarTablaKPIsPorActivoAsync(List<IndicadoresOTPorActivoDTO> indicadoresPorActivo)
        {
            MemoryStream buffer = new MemoryStream();
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear un nuevo archivo Excel
                ExcelPackage package = new ExcelPackage();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Datos");

                // Definimos los encabezados
                List<string> encabezados = new List<string>()
                {
                    "Activo",           // 1
                    "Ordenes",          // 2
                    "Cumplimiento %",   // 3
                    "Pendientes %",     // 4
                    "Material %"        // 5
                };

                // Rellenamos los encabezados en el fichero
                for (int i = 0; i < encabezados.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = encabezados[i];
                    worksheet.Cells[1, i + 1].Style.Font.Size = 14;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                for (int i = 0; i < indicadoresPorActivo.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = indicadoresPorActivo[i].IdActivo;
                    worksheet.Cells[i + 2, 2].Value = indicadoresPorActivo[i].Indicadores.TotalOrdenes;
                    worksheet.Cells[i + 2, 3].Value = indicadoresPorActivo[i].Indicadores.PorcentajeCompletadas;
                    worksheet.Cells[i + 2, 4].Value = indicadoresPorActivo[i].Indicadores.PorcentajePendientes;
                    worksheet.Cells[i + 2, 5].Value = indicadoresPorActivo[i].Indicadores.PorcentajeMaterial;
                }

                // Ajustar el texto a varias líneas
                worksheet.Cells.Style.WrapText = false;

                // Establecer el alineamiento horizontal en el centro para todas las celdas
                worksheet.Cells[worksheet.Dimension.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Guardar el libro de Excel en memoria
                await Task.Run(() => package.SaveAs(buffer));

                // Descargar el archivo Excel
                buffer.Position = 0;
                return buffer;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al generar el Excel de KPIs por activo: {ex.Message}");
            }
        }

        public async Task<MemoryStream> GenerarTablaKPIsPorActivoAsync(List<IndicadoresConfiabilidadPorActivoDTO> indicadoresPorActivo)
        {
            MemoryStream buffer = new MemoryStream();
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Crear un nuevo archivo Excel
                ExcelPackage package = new ExcelPackage();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Datos");

                // Definimos los encabezados
                List<string> encabezados = new List<string>()
                {
                    "Activo",           // 1
                    "Ordenes",          // 2
                    "MTBF",             // 3
                    "MTTR",             // 4
                    "Disponibilidad",   // 5
                    "Confiabilidad"     // 6
                };

                // Rellenamos los encabezados en el fichero
                for (int i = 0; i < encabezados.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = encabezados[i];
                    worksheet.Cells[1, i + 1].Style.Font.Size = 14;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                for (int i = 0; i < indicadoresPorActivo.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = indicadoresPorActivo[i].IdActivo;
                    worksheet.Cells[i + 2, 2].Value = indicadoresPorActivo[i].Indicadores.TotalOrdenesCerradas;
                    worksheet.Cells[i + 2, 3].Value = indicadoresPorActivo[i].Indicadores.TotalMTBF;
                    worksheet.Cells[i + 2, 4].Value = indicadoresPorActivo[i].Indicadores.TotalMTTR;
                    worksheet.Cells[i + 2, 5].Value = indicadoresPorActivo[i].Indicadores.TotalDisponibilidad;
                    worksheet.Cells[i + 2, 6].Value = indicadoresPorActivo[i].Indicadores.TotalConfiabilidad;
                }

                // Ajustar el texto a varias líneas
                worksheet.Cells.Style.WrapText = false;

                // Establecer el alineamiento horizontal en el centro para todas las celdas
                worksheet.Cells[worksheet.Dimension.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Guardar el libro de Excel en memoria
                await Task.Run(() => package.SaveAs(buffer));

                // Descargar el archivo Excel
                buffer.Position = 0;
                return buffer;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al generar el Excel de KPIs por activo: {ex.Message}");
            }
        }
    }
}