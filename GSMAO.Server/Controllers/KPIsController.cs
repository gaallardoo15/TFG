using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<KPIsController> _logger;
        private readonly CentroCosteDAO _centrosCostesDAO;
        private readonly CriticidadDAO _criticidadDAO;
        private readonly ActivoDAO _activoDAO;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly GeneradorExcel _generadorExcel;

        public KPIsController(
            IMapper mapper,
            ILogger<KPIsController> logger,
            CentroCosteDAO centrosCostesDAO,
            CriticidadDAO criticidadDAO,
            ActivoDAO activoDAO,
            OrdenesDAO ordenesDAO,
            GeneradorExcel generadorExcel
        )
        {
            _mapper = mapper;
            _logger = logger;
            _centrosCostesDAO = centrosCostesDAO;
            _criticidadDAO = criticidadDAO;
            _activoDAO = activoDAO;
            _ordenesDAO = ordenesDAO;
            _generadorExcel = generadorExcel;
        }

        /// <summary>
        /// Obtiene un diccionario de meses del año.
        /// </summary>
        /// <returns>
        /// Un <see cref="ActionResult{Dictionary{int, string}}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Diccionario con los meses del año.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("meses")]
        public ActionResult<Dictionary<int, string>> RetrieveMeses()
        {
            Dictionary<int, string> meses = new Dictionary<int, string>()
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" },
            };

            return Ok(meses);
        }

        /// <summary>
        /// Obtiene los centros de coste con filtros opcionales.
        /// </summary>
        /// <param name="year">Año para filtrar los centros de coste.</param>
        /// <param name="mes">Mes para filtrar los centros de coste.</param>
        /// <param name="idCriticidad">Identificador de la criticidad para filtrar los centros de coste.</param>
        /// <param name="idActivo">Identificador del activo para filtrar los centros de coste.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de centros de coste.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("centros-de-costes")]
        public async Task<ActionResult<IEnumerable<CentroCosteTableDTO>>> RetrieveCentrosCostes(
            int? year = null,
            int? mes = null,
            int? idCriticidad = null,
            int? idActivo = null
        )
        {
            var centrosCostes = await _centrosCostesDAO.GetCentrosCostesFiltrosAsync(year, mes, idCriticidad, idActivo);
            return Ok(_mapper.Map<List<CentroCosteTableDTO>>(centrosCostes));
        }

        /// <summary>
        /// Obtiene las criticidades con filtros opcionales.
        /// </summary>
        /// <param name="year">Año para filtrar las criticidades.</param>
        /// <param name="mes">Mes para filtrar las criticidades.</param>
        /// <param name="idCentroCoste">Identificador del centro de coste para filtrar las criticidades.</param>
        /// <param name="idActivo">Identificador del activo para filtrar las criticidades.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de criticidades.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("criticidades")]
        public async Task<ActionResult<IEnumerable<CriticidadDTO>>> RetrieveCriticidades(
            int? year = null, 
            int? mes = null, 
            int? idCentroCoste = null, 
            int? idActivo = null
        )
        {
            var criticidades = await _criticidadDAO.GetCriticidadesFiltrosAsync(year, mes, idCentroCoste, idActivo);
            return Ok(_mapper.Map<List<CriticidadDTO>>(criticidades));
        }

        /// <summary>
        /// Obtiene los activos por centro de coste con filtros opcionales.
        /// </summary>
        /// <param name="year">Año para filtrar los activos.</param>
        /// <param name="mes">Mes para filtrar los activos.</param>
        /// <param name="idCentroCoste">Identificador del centro de coste para filtrar los activos.</param>
        /// <param name="idCriticidad">Identificador de la criticidad para filtrar los activos.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de activos por centro de coste.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ActivoTableDTO>>> RetrieveActivosPorCentroCoste(
            int? year = null, 
            int? mes = null, 
            int? idCentroCoste = null, 
            int? idCriticidad = null
        )
        {
            var activos = await _activoDAO.GetActivosFiltrosAsync(year, mes, idCentroCoste, idCriticidad);
            return Ok(_mapper.Map<List<ActivoTableDTO>>(activos));
        }

        /// <summary>
        /// Obtiene los KPIs de las órdenes con filtros opcionales.
        /// </summary>
        /// <param name="year">Año para filtrar las órdenes.</param>
        /// <param name="mes">Mes para filtrar las órdenes.</param>
        /// <param name="idCentroCoste">Identificador del centro de coste para filtrar las órdenes.</param>
        /// <param name="idCriticidad">Identificador de la criticidad para filtrar las órdenes.</param>
        /// <param name="idActivo">Identificador del activo para filtrar las órdenes.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: KPIs de las órdenes.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("ordenes")]
        public async Task<ActionResult<KPIsOrdenesDTO>> GetKPIsOrdenes(
            [FromQuery] int year,
            [FromQuery] int mes,
            [FromQuery] int idCentroCoste,
            [FromQuery] int idCriticidad,
            [FromQuery] int idActivo,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            var ordenes = await _ordenesDAO.GetOrdenesKPIs(year, mes, idCentroCoste, idCriticidad, idActivo, fechaDesde, fechaHasta);

            if (!ordenes.Any())
            {
                return new KPIsOrdenesDTO()
                {
                    Correctivas = new List<OrdenesPeriodoDTO>(),
                    FallaHumana = new List<OrdenesPeriodoDTO>(),
                    MantenimientoGeneral = new List<MantenimientoGeneralDTO>(),
                    Mejoras = new List<OrdenesPeriodoDTO>(),
                    Preventivas = new List<OrdenesPeriodoDTO>(),
                    DesglosePorActivos = new List<IndicadoresOTPorActivoDTO>()
                };
            }

            var vista = year != 0 && mes != 0 ? "SEMANAL" :
                        year != 0 && mes == 0 ? "MENSUAL" :
                        "ANUAL";

            var todosLosMeses = true;
            var vistaEspecifica = false;
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();
            if (fechaDesde != null || fechaHasta != null)
            {
                DateTime fechaInicio = fechaDesde ?? ordenes.Min(o => o.FechaApertura);
                DateTime fechaFin = fechaHasta ?? DateTime.Today;

                int diferenciaMeses = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month;
                int diferenciaAnios = fechaFin.Year - fechaInicio.Year;

                if (diferenciaMeses <= 3)
                {
                    vista = "SEMANAL";
                    vistaEspecifica = years.Count() > 1 ? true : false;
                }
                if (diferenciaMeses > 3 && diferenciaMeses <= 12)
                {
                    vista = "MENSUAL";
                    vistaEspecifica = years.Count() > 1 ? true : false;
                    todosLosMeses = years.Count() > 1 ? true : false;
                }
                if (diferenciaMeses > 12) vista = "ANUAL";
            }

            Calendar calendario = CultureInfo.CurrentCulture.Calendar;
            try {
                foreach (var orden in ordenes)
                {
                    orden.Periodo = vista switch
                    {
                        "SEMANAL" => calendario.GetWeekOfYear(orden.FechaApertura, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                        "MENSUAL" => calendario.GetMonth(orden.FechaApertura),
                        "ANUAL" => calendario.GetYear(orden.FechaApertura),
                        _ => throw new ArgumentNullException("Error al obtener el periodo de una orden por no tener fecha de apertura predefinida.")
                    };
                }
            }
            catch (ArgumentNullException ex) {
                _logger.LogError($"{ex.Message}");
                return StatusCode(500, $"{ex.Message}");
            }

            var contadorPeriodos = 0;
            var periodosTotalesDelAnoAnteriorSegunVista = 0;

            var ordenesOrderByFechaApertura = ordenes.OrderBy(o => o.FechaApertura).ToList();
            var minPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).First();
            var maxPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).Last();

            if (vista == "SEMANAL" && vistaEspecifica)
            {
                periodosTotalesDelAnoAnteriorSegunVista = calendario.GetWeekOfYear(new DateTime(years[0], 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            }
            else if (vista == "MENSUAL" && vistaEspecifica)
            {
                periodosTotalesDelAnoAnteriorSegunVista = calendario.GetMonth(new DateTime(years[0], 12, 31));
            }

            if (vistaEspecifica)
            {
                contadorPeriodos = periodosTotalesDelAnoAnteriorSegunVista - minPeriodos + maxPeriodos + 1;
            }
            else
            {
                contadorPeriodos = maxPeriodos - minPeriodos + 1;
            }

            KPIsOrdenesDTO kpis = new KPIsOrdenesDTO()
            {
                TotalOrdenes = ordenes.Count(),
                Correctivas = new List<OrdenesPeriodoDTO>(),
                Preventivas = new List<OrdenesPeriodoDTO>(),
                Mejoras = new List<OrdenesPeriodoDTO>(),
                FallaHumana = new List<OrdenesPeriodoDTO>(),
                MantenimientoGeneral = new List<MantenimientoGeneralDTO>(),
                PorcentajeCompletadas = 0,
                PorcentajeMaterial = 0,
                PorcentajePendientes = 0,
                DesglosePorActivos = new List<IndicadoresOTPorActivoDTO>()
            };

            // Calcular los porcentajes de COMPLETADAS, PENDIENTES Y MATERIALES
            CalcularPorcentajes(kpis, ordenes);

            // Rellenar las listas de tipos CORRECTIVAS, PREVENTIVAS, MEJORAS Y FALLA HUMANA
            RellenarTipos(kpis, ordenes, vista, minPeriodos, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, todosLosMeses);

            // Rellenar la lista de mantenimiento general
            RellenarMantenimientoGeneral(kpis, ordenes, vista, minPeriodos, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, todosLosMeses);

            // Rellenar la lista de indicadores por activo
            RellenarIndicadoresPorActivo(kpis, ordenes);

            return Ok(kpis);
        }

        [HttpGet("ordenes/descargar-tabla")]
        public async Task<IActionResult> DescargarKPIsOrdenesTabla(
            [FromQuery] int year,
            [FromQuery] int mes,
            [FromQuery] int idCentroCoste,
            [FromQuery] int idCriticidad,
            [FromQuery] int idActivo,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            try
            {
                var ordenes = await _ordenesDAO.GetOrdenesKPIs(year, mes, idCentroCoste, idCriticidad, idActivo, fechaDesde, fechaHasta);

                var tabla = RellenarIndicadoresPorActivo(ordenes);

                if (tabla == null || tabla.Count() == 0)
                    throw new Exception("La tabla no contiene datos, por lo que no está permitida la descarga.");

                MemoryStream buffer = await _generadorExcel.GenerarTablaKPIsPorActivoAsync(tabla);

                // Compruebo si tengo un Activo especificado o no para indicar el nombre del fichero
                string nombreFichero = "TablaKPIsOrdenesPorActivo.xlsx";

                return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreFichero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                if (ex.Message.Contains("La tabla no contiene datos"))
                    return NotFound(ex.Message);
                else
                    return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("confiabilidad")]
        public async Task<ActionResult<KPIsConfiabilidadDTO>> GetKPIsConfiabilidad(
            [FromQuery] int year,
            [FromQuery] int mes, 
            [FromQuery] int idCentroCoste, 
            [FromQuery] int idCriticidad, 
            [FromQuery] int idActivo,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            var ordenes = await _ordenesDAO.GetConfiabilidadKPIs(year, mes, idCentroCoste, idCriticidad, idActivo, fechaDesde, fechaHasta);

            if (!ordenes.Any())
                return new KPIsConfiabilidadDTO() { GraficasPeriodos = new List<ConfiabilidadPeriodoDTO>(), DesglosePorActivos = new List<IndicadoresConfiabilidadPorActivoDTO>() };

            var vista = year != 0 && mes != 0 ? "SEMANAL" :
                        year != 0 && mes == 0 ? "MENSUAL" :
                        "ANUAL";
            
            var todosLosMeses = true;
            var vistaEspecifica = false;
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();
            if (fechaDesde != null || fechaHasta != null)
            {
                DateTime fechaInicio = fechaDesde ?? ordenes.Min(o => o.FechaApertura);
                DateTime fechaFin = fechaHasta ?? DateTime.Today;

                int diferenciaMeses = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month;
                int diferenciaAnios = fechaFin.Year - fechaInicio.Year;

                if (diferenciaMeses <= 3)
                {                    
                    vista = "SEMANAL";
                    vistaEspecifica = years.Count() > 1 ? true : false;
                }

                if (diferenciaMeses > 3 && diferenciaMeses <= 12)
                {
                    vista = "MENSUAL";
                    vistaEspecifica = years.Count() > 1 ? true : false;
                    todosLosMeses = false;                    
                }

                if (diferenciaMeses > 12) vista = "ANUAL";
            }

            Calendar calendario = CultureInfo.CurrentCulture.Calendar;
            try {
                foreach (var orden in ordenes)
                {
                    orden.Periodo = vista switch
                    {
                        "SEMANAL" => calendario.GetWeekOfYear(orden.FechaApertura, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                        "MENSUAL" => calendario.GetMonth(orden.FechaApertura),
                        "ANUAL" => calendario.GetYear(orden.FechaApertura),
                        _ => throw new ArgumentNullException("Error al obtener el periodo de una orden por no tener fecha de apertura predefinida.")
                    };
                }
            }
            catch (ArgumentNullException ex) {
                _logger.LogError($"{ex.Message}");
                return StatusCode(500, $"{ex.Message}");
            }

            var contadorPeriodos = 0;
            var periodosTotalesDelAnoAnteriorSegunVista = 0;

            var ordenesOrderByFechaApertura = ordenes.OrderBy(o => o.FechaApertura).ToList();
            var minPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).First();
            var maxPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).Last();

            if (vista == "SEMANAL" && vistaEspecifica)
            {
                periodosTotalesDelAnoAnteriorSegunVista = calendario.GetWeekOfYear(new DateTime(years[0], 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            }
            else if (vista == "MENSUAL" && vistaEspecifica)
            {
                periodosTotalesDelAnoAnteriorSegunVista = calendario.GetMonth(new DateTime(years[0], 12, 31));
            }

            if (vistaEspecifica)
            {
                contadorPeriodos = periodosTotalesDelAnoAnteriorSegunVista - minPeriodos + maxPeriodos + 1;
            }
            else
            {
                contadorPeriodos = maxPeriodos - minPeriodos + 1;
            }

            KPIsConfiabilidadDTO kpis = new KPIsConfiabilidadDTO()
            {
                TotalOrdenesCerradas = ordenes.Select(o => o.IdOrden).Distinct().Count(),
                GraficasPeriodos = new List<ConfiabilidadPeriodoDTO>(),
                DesglosePorActivos = new List<IndicadoresConfiabilidadPorActivoDTO>()
            };

            var tiempoActivoCritico = vista == "SEMANAL" ? 168 : vista == "MENSUAL" ? 720 : 8760;
            var tiempoActivoNoCritico = vista == "SEMANAL" ? 80 : vista == "MENSUAL" ? 320 : 3840;

            // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad por periodos (SEMANAL, MENSUAL, ANUAL)
            CalcularFormulasPeriodos(kpis, ordenes, vista, minPeriodos, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, tiempoActivoCritico, tiempoActivoNoCritico, todosLosMeses);

            CalcularFormulasActivos(out List<ConfiabilidadPeriodoDTO> indicadoresPorPeriodoAndPorActivo, ordenes, vista, minPeriodos, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, tiempoActivoCritico, tiempoActivoNoCritico, todosLosMeses);

            // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad TOTALES
            var tiempoParada = ordenes.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
            var tiempoTotalReparaciones = tiempoParada;
            var NumIncidencias = ordenes.Select(p => p.IdIncidencia).Count();

            if (kpis.TotalOrdenesCerradas != 0)
            {
                kpis.TotalMTBF = Math.Round(kpis.GraficasPeriodos.Sum(g => g.MTBF) / kpis.GraficasPeriodos.Where(g => g.OrdenesCerradas != 0).Count());
                kpis.TotalMTTR = CalcularMTTR(tiempoTotalReparaciones, NumIncidencias);
                kpis.TotalDisponibilidad = CalcularDisponibilidad(kpis.TotalMTBF, kpis.TotalMTTR);
                kpis.TotalConfiabilidad = Math.Round(kpis.GraficasPeriodos.Sum(g => g.Confiabilidad) / kpis.GraficasPeriodos.Count());
            }

            // Agrupamos por IdActivo y obtenemos todos los periodos
            var groupedByActivo = indicadoresPorPeriodoAndPorActivo.GroupBy(g => g.IdActivo);

            foreach (var item in groupedByActivo)
            {
                var ordenesActivo = ordenes.Where(o => o.IdActivo == item.Key).ToList();

                // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad TOTALES POR ACTIVO
                var tiempoParadaActivo = ordenesActivo.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
                var NumOrdenesActivo = ordenesActivo.Select(p => p.IdOrden).Distinct().Count();
                var tiempoTotalReparacionesActivo = tiempoParadaActivo;
                var NumIncidenciasActivo = ordenesActivo.Select(p => p.IdIncidencia).Count();

                if (NumOrdenesActivo != 0)
                {
                    var calculoMTBF = Math.Round(item.Sum(i => i.MTBF) / item.Where(i => i.OrdenesCerradas != 0).Count());
                    var calculoMTTR = CalcularMTTR(tiempoTotalReparacionesActivo, NumIncidenciasActivo);

                    kpis.DesglosePorActivos.Add(new IndicadoresConfiabilidadPorActivoDTO
                    {
                        IdActivo = item.Key,
                        Indicadores = new IndicadoresConfiabilidadDTO
                        {
                            TotalOrdenesCerradas = NumOrdenesActivo,
                            TotalMTBF = calculoMTBF,
                            TotalMTTR = calculoMTTR,
                            TotalDisponibilidad = CalcularDisponibilidad(calculoMTBF, calculoMTTR),
                            TotalConfiabilidad = Math.Round(item.Sum(i => i.Confiabilidad) / item.Count())
                        }
                    });
                }
            }

            kpis.DesglosePorActivos = kpis.DesglosePorActivos.OrderByDescending(d => d.Indicadores.TotalOrdenesCerradas).ToList();

            return Ok(kpis);
        }

        [HttpGet("confiabilidad/descargar-tabla")]
        public async Task<IActionResult> DescargarKPIsConfiablidadTabla(
            [FromQuery] int year,
            [FromQuery] int mes,
            [FromQuery] int idCentroCoste,
            [FromQuery] int idCriticidad,
            [FromQuery] int idActivo,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            try
            {
                var ordenes = await _ordenesDAO.GetConfiabilidadKPIs(year, mes, idCentroCoste, idCriticidad, idActivo, fechaDesde, fechaHasta);

                var vista = year != 0 && mes != 0 ? "SEMANAL" :
                        year != 0 && mes == 0 ? "MENSUAL" :
                        "ANUAL";
            
                var todosLosMeses = true;
                var vistaEspecifica = false;
                var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();
                if (fechaDesde != null || fechaHasta != null)
                {
                    DateTime fechaInicio = fechaDesde ?? ordenes.Min(o => o.FechaApertura);
                    DateTime fechaFin = fechaHasta ?? DateTime.Today;

                    int diferenciaMeses = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month;
                    int diferenciaAnios = fechaFin.Year - fechaInicio.Year;

                    if (diferenciaMeses <= 3)
                    {                    
                        vista = "SEMANAL";
                        vistaEspecifica = years.Count() > 1 ? true : false;
                    }

                    if (diferenciaMeses > 3 && diferenciaMeses <= 12)
                    {
                        vista = "MENSUAL";
                        vistaEspecifica = years.Count() > 1 ? true : false;
                        todosLosMeses = false;                    
                    }

                    if (diferenciaMeses > 12) vista = "ANUAL";
                }

                Calendar calendario = CultureInfo.CurrentCulture.Calendar;
                try {
                    foreach (var orden in ordenes)
                    {
                        orden.Periodo = vista switch
                        {
                            "SEMANAL" => calendario.GetWeekOfYear(orden.FechaApertura, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                            "MENSUAL" => calendario.GetMonth(orden.FechaApertura),
                            "ANUAL" => calendario.GetYear(orden.FechaApertura),
                            _ => throw new ArgumentNullException("Error al obtener el periodo de una orden por no tener fecha de apertura predefinida.")
                        };
                    }
                }
                catch (ArgumentNullException ex) {
                    _logger.LogError($"{ex.Message}");
                    return StatusCode(500, $"{ex.Message}");
                }

                var contadorPeriodos = 0;
                var periodosTotalesDelAnoAnteriorSegunVista = 0;

                var ordenesOrderByFechaApertura = ordenes.OrderBy(o => o.FechaApertura).ToList();
                var minPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).First();
                var maxPeriodos = ordenesOrderByFechaApertura.Select(oo => oo.Periodo).Last();

                if (vista == "SEMANAL" && vistaEspecifica)
                {
                    periodosTotalesDelAnoAnteriorSegunVista = calendario.GetWeekOfYear(new DateTime(years[0], 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                }
                else if (vista == "MENSUAL" && vistaEspecifica)
                {
                    periodosTotalesDelAnoAnteriorSegunVista = calendario.GetMonth(new DateTime(years[0], 12, 31));
                }

                if (vistaEspecifica)
                {
                    contadorPeriodos = periodosTotalesDelAnoAnteriorSegunVista - minPeriodos + maxPeriodos + 1;
                }
                else
                {
                    contadorPeriodos = maxPeriodos - minPeriodos + 1;
                }

                var tiempoActivoCritico = vista == "SEMANAL" ? 168 : vista == "MENSUAL" ? 720 : 8760;
                var tiempoActivoNoCritico = vista == "SEMANAL" ? 80 : vista == "MENSUAL" ? 320 : 3840;

                CalcularFormulasActivos(out List<ConfiabilidadPeriodoDTO> indicadoresPorPeriodoAndPorActivo, ordenes, vista, minPeriodos, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, tiempoActivoCritico, tiempoActivoNoCritico, todosLosMeses);

                // Agrupamos por IdActivo y obtenemos todos los periodos
                var groupedByActivo = indicadoresPorPeriodoAndPorActivo.GroupBy(g => g.IdActivo);
                List<IndicadoresConfiabilidadPorActivoDTO> tabla = new List<IndicadoresConfiabilidadPorActivoDTO>();

                foreach (var item in groupedByActivo)
                {
                    var ordenesActivo = ordenes.Where(o => o.IdActivo == item.Key).ToList();

                    // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad TOTALES POR ACTIVO
                    var tiempoParadaActivo = ordenesActivo.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
                    var NumOrdenesActivo = ordenesActivo.Select(p => p.IdOrden).Distinct().Count();
                    var tiempoTotalReparacionesActivo = tiempoParadaActivo;
                    var NumIncidenciasActivo = ordenesActivo.Select(p => p.IdIncidencia).Count();

                    if (NumOrdenesActivo != 0)
                    {
                        var calculoMTBF = Math.Round(item.Sum(i => i.MTBF) / item.Where(i => i.OrdenesCerradas != 0).Count());
                        var calculoMTTR = CalcularMTTR(tiempoTotalReparacionesActivo, NumIncidenciasActivo);

                        tabla.Add(new IndicadoresConfiabilidadPorActivoDTO
                        {
                            IdActivo = item.Key,
                            Indicadores = new IndicadoresConfiabilidadDTO
                            {
                                TotalOrdenesCerradas = NumOrdenesActivo,
                                TotalMTBF = calculoMTBF,
                                TotalMTTR = calculoMTTR,
                                TotalDisponibilidad = CalcularDisponibilidad(calculoMTBF, calculoMTTR),
                                TotalConfiabilidad = Math.Round(item.Sum(i => i.Confiabilidad) / item.Count())
                            }
                        });
                    }
                }

                tabla = tabla.OrderByDescending(d => d.Indicadores.TotalOrdenesCerradas).ToList();

                if (tabla == null || tabla.Count == 0)
                    throw new Exception("La tabla no contiene datos, por lo que no está permitida la descarga.");

                MemoryStream buffer = await _generadorExcel.GenerarTablaKPIsPorActivoAsync(tabla);

                // Compruebo si tengo un Activo especificado o no para indicar el nombre del fichero
                string nombreFichero = "TablaKPIsOrdenesPorActivo.xlsx";

                return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreFichero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                if (ex.Message.Contains("La tabla no contiene datos"))
                    return NotFound(ex.Message);
                else
                    return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("estudio-comparativo-ordenes")]
        public async Task<ActionResult<IEnumerable<KPIsOrdenesDTO>>> GetEstudioComparativoKPIsOrdenes(
            [FromQuery] string yearsString, 
            [FromQuery] string? mesesString, 
            [FromQuery] int idCentroCoste, 
            [FromQuery] int idCriticidad, 
            [FromQuery] int idActivo
        )
        {
            try
            {
                List<KPIsOrdenesDTO> kpis = new List<KPIsOrdenesDTO>();

                // Obtener los listados de años y meses
                var years = JsonSerializer.Deserialize<List<int>>(yearsString);
                var meses = !string.IsNullOrWhiteSpace(mesesString) ? JsonSerializer.Deserialize<List<int>>(mesesString) : null;

                if (years == null)
                    throw new ArgumentNullException("Debes indicar al menos 2 años para realizar el estudio comparativo.");

                // Comprobar la vista que se va a representar
                var vista = meses != null && meses.Count() > 0 ? "SEMANAL" : "MENSUAL";

                var ordenesEstudio = await _ordenesDAO.GetOrdenesKPIs(years, meses, idCentroCoste, idCriticidad, idActivo, null, null);

                if (ordenesEstudio.Count() == 0)
                    return new List<KPIsOrdenesDTO>();

                Calendar calendario = CultureInfo.CurrentCulture.Calendar;
                foreach (var orden in ordenesEstudio)
                {
                    if (vista == "SEMANAL")
                        orden.Periodo = calendario.GetWeekOfYear(orden.FechaApertura, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    else
                        orden.Periodo = calendario.GetMonth(orden.FechaApertura);
                }

                var maxPeriodos = ordenesEstudio.Max(o => o.Periodo);
                var minPeriodo = ordenesEstudio.Min(o => o.Periodo);

                foreach (var year in years)
                {
                    var ordenesYear = ordenesEstudio.Where(o => o.FechaApertura.Year == year).ToList();

                    KPIsOrdenesDTO kpisOrden = new KPIsOrdenesDTO()
                    {
                        Year = year,
                        TotalOrdenes = ordenesYear.Select(o => o.IdOrden).Distinct().Count(),
                        Correctivas = new List<OrdenesPeriodoDTO>(),
                        Preventivas = new List<OrdenesPeriodoDTO>(),
                        Mejoras = new List<OrdenesPeriodoDTO>(),
                        FallaHumana = new List<OrdenesPeriodoDTO>(),
                        MantenimientoGeneral = new List<MantenimientoGeneralDTO>(),
                        PorcentajeCompletadas = 0,
                        PorcentajeMaterial = 0,
                        PorcentajePendientes = 0,
                        DesglosePorActivos = new List<IndicadoresOTPorActivoDTO>()
                    };

                    var periodosTotalesDelAnoAnteriorSegunVista = 0;
                    var todosLosMeses = true;
                    var contadorPeriodos = maxPeriodos - minPeriodo + 1;

                    if (ordenesYear.Count() != 0)
                    {
                        // Calcular los porcentajes de COMPLETADAS, PENDIENTES Y MATERIALES
                        CalcularPorcentajes(kpisOrden, ordenesYear);

                        // Rellenar las listas de tipos CORRECTIVAS, PREVENTIVAS, MEJORAS Y FALLA HUMANA
                        RellenarTipos(kpisOrden, ordenesYear, vista, minPeriodo, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, todosLosMeses);

                        // Rellenar la lista de mantenimiento general
                        RellenarMantenimientoGeneral(kpisOrden, ordenesYear, vista, minPeriodo, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, todosLosMeses);
                    }

                    kpis.Add(kpisOrden);
                }

                return kpis;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener el estudio comparativo de los indicativos de ordenes: {ex.Message}");

                return StatusCode(500, $"Error al obtener el estudio comparativo de los indicativos de ordenes.");
            }
        }

        [HttpGet("estudio-comparativo-confiabilidad")]
        public async Task<ActionResult<IEnumerable<KPIsConfiabilidadDTO>>> GetEstudioComparativoKPIsConfiabilidad(
            [FromQuery] string yearsString, 
            [FromQuery] string? mesesString, 
            [FromQuery] int? idCentroCoste, 
            [FromQuery] int? idCriticidad, 
            [FromQuery] int? idActivo,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            try
            {
                List<KPIsConfiabilidadDTO> kpis = new List<KPIsConfiabilidadDTO>();

                // Obtener los listados de años y meses
                var years = JsonSerializer.Deserialize<List<int>>(yearsString);
                var meses = !string.IsNullOrWhiteSpace(mesesString) ? JsonSerializer.Deserialize<List<int>>(mesesString) : null;

                if (years == null)
                    throw new ArgumentNullException("Debes indicar al menos 2 años para realizar el estudio comparativo.");

                // Comprobar la vista que se va a representar
                var vista = meses != null && meses.Count() > 0 ? "SEMANAL" : "MENSUAL";

                var ordenesEstudio = await _ordenesDAO.GetConfiabilidadKPIs(years, meses, idCentroCoste, idCriticidad, idActivo, null, null);

                if (ordenesEstudio.Count() == 0)
                    return new List<KPIsConfiabilidadDTO>();

                Calendar calendario = CultureInfo.CurrentCulture.Calendar;
                foreach (var orden in ordenesEstudio)
                {
                    if (vista == "SEMANAL")
                        orden.Periodo = calendario.GetWeekOfYear(orden.FechaApertura, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    else
                        orden.Periodo = calendario.GetMonth(orden.FechaApertura);
                }

                var maxPeriodos = ordenesEstudio.Max(o => o.Periodo);
                var minPeriodo = ordenesEstudio.Min(o => o.Periodo);
                var periodosTotalesDelAnoAnteriorSegunVista = 0;
                var todosLosMeses = true;
                var contadorPeriodos = maxPeriodos - minPeriodo + 1;

                foreach (var year in years)
                {
                    var ordenesYear = ordenesEstudio.Where(o => o.FechaApertura.Year == year).ToList();

                    KPIsConfiabilidadDTO kpisOrden = new KPIsConfiabilidadDTO()
                    {
                        Year = year,
                        TotalOrdenesCerradas = ordenesYear.Select(o => o.IdOrden).Distinct().Count(),
                        GraficasPeriodos = new List<ConfiabilidadPeriodoDTO>(),
                        DesglosePorActivos = new List<IndicadoresConfiabilidadPorActivoDTO>()
                    };

                    var tiempoActivoCritico = vista == "SEMANAL" ? 168 : vista == "MENSUAL" ? 720 : 8760;
                    var tiempoActivoNoCritico = vista == "SEMANAL" ? 80 : vista == "MENSUAL" ? 320 : 3840;

                    // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad por periodos (SEMANAL, MENSUAL, ANUAL)
                    CalcularFormulasPeriodos(kpisOrden, ordenesYear, vista, minPeriodo, contadorPeriodos, periodosTotalesDelAnoAnteriorSegunVista, tiempoActivoCritico, tiempoActivoNoCritico, todosLosMeses);
                    
                    // Calculamos las fórmulas MTBF, MTTR, Disponibilidad y Confiabilidad TOTALES
                    var tiempoFuncionamiento = ordenesYear.Count(p => p.ActivoCritico) * tiempoActivoCritico + ordenesYear.Count(p => !p.ActivoCritico) * tiempoActivoNoCritico;
                    var tiempoParada = ordenesYear.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
                    var NumOrdenes = ordenesYear.Select(p => p.IdOrden).Distinct().Count();
                    var tiempoTotalReparaciones = tiempoParada;
                    var NumIncidencias = ordenesYear.Select(p => p.IdIncidencia).Count();
                    var tiempoPeriodo = tiempoActivoNoCritico;

                    if (kpisOrden.TotalOrdenesCerradas != 0)
                    {
                        kpisOrden.TotalMTBF = Math.Round(kpisOrden.GraficasPeriodos.Sum(g => g.MTBF) / kpisOrden.GraficasPeriodos.Where(g => g.OrdenesCerradas != 0).Count());
                        kpisOrden.TotalMTTR = CalcularMTTR(tiempoTotalReparaciones, NumIncidencias);
                        kpisOrden.TotalDisponibilidad = CalcularDisponibilidad(kpisOrden.TotalMTBF, kpisOrden.TotalMTTR);
                        kpisOrden.TotalConfiabilidad = Math.Round(kpisOrden.GraficasPeriodos.Sum(g => g.Confiabilidad) / kpisOrden.GraficasPeriodos.Count());
                    }

                    kpis.Add(kpisOrden);
                }

                return kpis;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el estudio comparativo de los indicativos de confiabilidad: {ex}");

                return StatusCode(500, $"Error al obtener el estudio comparativo de los indicativos de confiabilidad: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula los porcentajes de órdenes completadas, pendientes y con material.
        /// </summary>
        /// <param name="kpis">El objeto KPIsOrdenesDTO donde se almacenan los resultados.</param>
        /// <param name="ordenes">La lista de órdenes para calcular los porcentajes.</param>
        private void CalcularPorcentajes(
            KPIsOrdenesDTO kpis, 
            List<KPIsOrdenesAuxDTO> ordenes
        )
        {
            // Solo hace referencia al estado CERRADA
            kpis.PorcentajeCompletadas = Math.Round(ordenes.Where(o => o.IdEstado == 2).Count() * 100.0 / kpis.TotalOrdenes, 2);

            // Solo hace referencia a los estados ABIERTA y EN CURSO
            kpis.PorcentajePendientes = Math.Round(ordenes.Where(o => o.IdEstado == 1 || o.IdEstado == 6).Count() * 100.0 / kpis.TotalOrdenes, 2);

            // Solo hace referencia a los estados ABIERTA/CERRADA: PENDIENTE MATERIAL y ABIERTA/CERRADA: MATERIAL GESTIONADO
            kpis.PorcentajeMaterial = Math.Round(ordenes.Where(o => o.IdEstado == 4 || o.IdEstado == 5 || o.IdEstado == 7 || o.IdEstado == 8).Count() * 100.0 / kpis.TotalOrdenes, 2);
        }

        /// <summary>
        /// Calcula los porcentajes de órdenes completadas, pendientes y con material.
        /// </summary>
        /// <param name="kpis">El objeto KPIsOrdenesDTO donde se almacenan los resultados.</param>
        /// <param name="ordenes">La lista de órdenes para calcular los porcentajes.</param>
        private IndicadoresDTO CalcularPorcentajes(
            List<KPIsOrdenesAuxDTO> ordenes, 
            int TotalOrdenes
        )
        {
            var indicadores = new IndicadoresDTO() { TotalOrdenes = TotalOrdenes };

            // Solo hace referencia al estado CERRADA
            indicadores.PorcentajeCompletadas = Math.Round(ordenes.Where(o => o.IdEstado == 2).Count() * 100.0 / TotalOrdenes, 2);

            // Solo hace referencia a los estados ABIERTA y EN CURSO
            indicadores.PorcentajePendientes = Math.Round(ordenes.Where(o => o.IdEstado == 1 || o.IdEstado == 6).Count() * 100.0 / TotalOrdenes, 2);

            // Solo hace referencia a los estados ABIERTA/CERRADA: PENDIENTE MATERIAL y ABIERTA/CERRADA: MATERIAL GESTIONADO
            indicadores.PorcentajeMaterial = Math.Round(ordenes.Where(o => o.IdEstado == 4 || o.IdEstado == 5 || o.IdEstado == 7 || o.IdEstado == 8).Count() * 100.0 / TotalOrdenes, 2);

            return indicadores;
        }

        /// <summary>
        /// Rellena los tipos de órdenes (correctivas, preventivas, mejoras, etc.) por período (semanal o anual).
        /// </summary>
        /// <param name="kpis">El objeto KPIsOrdenesDTO donde se almacenan los resultados.</param>
        /// <param name="ordenes">La lista de órdenes para agrupar y calcular los porcentajes.</param>
        /// <param name="vista">El tipo de vista (semanal o anual) que se utilizará para nombrar los períodos.</param>
        private void RellenarTipos(
            KPIsOrdenesDTO kpis, 
            List<KPIsOrdenesAuxDTO> ordenes, 
            string vista, 
            int minPeriodoGlobal, 
            int contadorPeriodos,
            int periodosTotalesDelFirstYearSegunVista,
            bool todosLosMeses
        )
        {
            var ordenesPeriodosTipos = new List<OrdenesPeriodoAuxDTO>();

            // Quiero agrupar por tipos de orden, además de por periodos (semanas)
            var groupedByTipoPeriodo = ordenes
                .GroupBy(o => new { o.IdTipoOrden, o.Periodo, Anio = o.FechaApertura.Year }) // Agrupamos por IdTipoOrden y Año
                .Select(grupo => new
                {
                    TipoOrden = grupo.Key.IdTipoOrden,
                    grupo.Key.Periodo,
                    grupo.Key.Anio,
                    Ordenes = grupo.OrderBy(o => o.FechaApertura).ToList() // Ordenamos las órdenes
                })
                .OrderBy(g => g.Anio)  // Ordenamos los resultados finales por año
                .ThenBy(g => g.Periodo)
                .ToList();
            
            // Obtener los años únicos
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();

            // Crea una lista de periodos completa desde 1 hasta maxPeriodoGlobal
            IEnumerable<(int, int)> todosLosPeriodos;
            if (periodosTotalesDelFirstYearSegunVista != 0)
            {
                var periodosFirstYear = Enumerable.Range(minPeriodoGlobal, periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1).ToList().Select(p => (years[0], p));
                var periodosLastYear = Enumerable.Range(1, contadorPeriodos - (periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1)).ToList().Select(p => (years[1], p));

                todosLosPeriodos = periodosFirstYear.Concat(periodosLastYear).ToList();
            }
            else if (vista == "MENSUAL" && todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (years[0], p));
            }
            else if (vista == "MENSUAL" && !todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (p, p));
            }
            else if (vista == "ANUAL")
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (p, p));
            }
            else
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (years[0], p));
            }

            // Obtener los tipos de orden únicos
            var tiposOrdenUnicos = ordenes.Select(o => o.IdTipoOrden).Distinct().ToList();

            foreach (var tipo in tiposOrdenUnicos)
            {
                foreach (var periodo in todosLosPeriodos)
                {
                    var coincidencia = groupedByTipoPeriodo.FirstOrDefault(t => t.TipoOrden == tipo && t.Periodo == periodo.Item2 && t.Anio == periodo.Item1);
                    var coletilla = years.Count() > 1 ? $" del {periodo.Item1}" : "";

                    var ordenesPeriodo = new OrdenesPeriodoAuxDTO()
                    {
                        IdTipoOrden = tipo,
                        Periodo = periodo.Item2,
                        NombrePeriodo = vista == "SEMANAL"
                                            ? $"Semana {periodo.Item2} {coletilla}"
                                            : vista == "MENSUAL"
                                                ? ConvertirMes(periodo.Item2) + coletilla
                                                : periodo.Item2.ToString(),
                        NumOrdenes = coincidencia?.Ordenes.Count() ?? 0,
                        PorcentajeOrdenes = coincidencia != null ? (coincidencia.Ordenes.Count() * 100.0f / ordenes.Count) : 0,
                        OrdenesPorActivo =  coincidencia != null
                                                ? coincidencia.Ordenes
                                                    .GroupBy(o => o.IdActivo)
                                                    .Select(g => new OrdenesActivoDTO
                                                    {
                                                        IdActivo = g.Key,
                                                        NumOrdenesActivo = g.Count()
                                                    })
                                                    .OrderByDescending(o => o.NumOrdenesActivo)
                                                    .Take(3)
                                                    .ToList()
                                                : new List<OrdenesActivoDTO>()
                    };

                    ordenesPeriodosTipos.Add(ordenesPeriodo);
                }
            }

            kpis.Correctivas = _mapper.Map<List<OrdenesPeriodoDTO>>(ordenesPeriodosTipos.Where(o => o.IdTipoOrden == 1).ToList());
            kpis.Preventivas = _mapper.Map<List<OrdenesPeriodoDTO>>(ordenesPeriodosTipos.Where(o => o.IdTipoOrden == 2).ToList());
            kpis.Mejoras = _mapper.Map<List<OrdenesPeriodoDTO>>(ordenesPeriodosTipos.Where(o => o.IdTipoOrden == 3).ToList());
            kpis.FallaHumana = _mapper.Map<List<OrdenesPeriodoDTO>>(ordenesPeriodosTipos.Where(o => o.IdTipoOrden == 4).ToList());
        }

        /// <summary>
        /// Rellena los datos de mantenimiento general por período (semanal o anual).
        /// </summary>
        /// <param name="kpis">El objeto KPIsOrdenesDTO donde se almacenan los resultados.</param>
        /// <param name="ordenes">La lista de órdenes para agrupar por período y calcular los porcentajes de cada tipo de orden.</param>
        /// <param name="vista">El tipo de vista (semanal o anual) que se utilizará para nombrar los períodos.</param>
        private void RellenarMantenimientoGeneral(
            KPIsOrdenesDTO kpis, 
            List<KPIsOrdenesAuxDTO> ordenes, 
            string vista, 
            int minPeriodoGlobal, 
            int contadorPeriodos,
            int periodosTotalesDelFirstYearSegunVista,
            bool todosLosMeses
        )
        {
            var groupedByPeriodo = ordenes.GroupBy(o => new { o.Periodo, Anio = o.FechaApertura.Year }).ToList();
            
            // Obtener los años únicos
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();

            // Crea una lista de periodos completa desde 1 hasta maxPeriodoGlobal
            IEnumerable<(int, int)> todosLosPeriodos;
            if (periodosTotalesDelFirstYearSegunVista != 0)
            {
                var periodosFirstYear = Enumerable.Range(minPeriodoGlobal, periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1).ToList().Select(p => (years[0], p));
                var periodosLastYear = Enumerable.Range(1, contadorPeriodos - (periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1)).ToList().Select(p => (years[1], p));

                todosLosPeriodos = periodosFirstYear.Concat(periodosLastYear).ToList();
            }
            else if (vista == "MENSUAL" && todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (years[0], p));
            }
            else if (vista == "MENSUAL" && !todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (p, p));
            }
            else if (vista == "ANUAL")
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (p, p));
            }
            else
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (years[0], p));
            }

            foreach (var periodo in todosLosPeriodos)
            {
                // Buscar las órdenes para el periodo actual, si no existen, utilizar una lista vacía
                var ordenesPeriodo = groupedByPeriodo.FirstOrDefault(g => g.Key.Periodo == periodo.Item2 && g.Key.Anio == periodo.Item1)?.ToList() ?? new List<KPIsOrdenesAuxDTO>();
                var coletilla = years.Count() > 1 ? $" del {periodo.Item1}" : "";

                var mantenimientoPeriodo = new MantenimientoGeneralDTO()
                {
                    Periodo = periodo.Item2,
                    NombrePeriodo = vista == "SEMANAL"
                                        ? $"Semana {periodo.Item2} {coletilla}"
                                        : vista == "MENSUAL"
                                            ? ConvertirMes(periodo.Item2) + coletilla
                                                : periodo.Item2.ToString(),
                };

                if (ordenesPeriodo.Count() != 0)
                {
                    mantenimientoPeriodo.PorcentajeCorrectivas = ordenesPeriodo.Count(o => o.IdTipoOrden == 1) * 100.0f / ordenesPeriodo.Count();
                    mantenimientoPeriodo.PorcentajePreventivas = ordenesPeriodo.Count(o => o.IdTipoOrden == 2) * 100.0f / ordenesPeriodo.Count();
                    mantenimientoPeriodo.PorcentajeMejoras = ordenesPeriodo.Count(o => o.IdTipoOrden == 3) * 100.0f / ordenesPeriodo.Count();
                    mantenimientoPeriodo.PorcentajeFallaHumana = ordenesPeriodo.Count(o => o.IdTipoOrden == 4) * 100.0f / ordenesPeriodo.Count();
                }

                kpis.MantenimientoGeneral.Add(mantenimientoPeriodo);
            }

            kpis.MantenimientoGeneral = kpis.MantenimientoGeneral.ToList();
        }

        private void RellenarIndicadoresPorActivo(
            KPIsOrdenesDTO kpis, 
            List<KPIsOrdenesAuxDTO> ordenes
        )
        {
            var ordenesAgrupadasPorActivo = ordenes.GroupBy(o => o.IdActivo);

            foreach (var item in ordenesAgrupadasPorActivo)
            {
                kpis.DesglosePorActivos.Add(new IndicadoresOTPorActivoDTO
                {
                    IdActivo = item.Key,
                    Indicadores = CalcularPorcentajes(item.ToList(), item.Count())
                });
            }

            kpis.DesglosePorActivos = kpis.DesglosePorActivos.OrderByDescending(d => d.Indicadores.TotalOrdenes).ToList();
        }

        private List<IndicadoresOTPorActivoDTO> RellenarIndicadoresPorActivo(List<KPIsOrdenesAuxDTO> ordenes)
        {
            List<IndicadoresOTPorActivoDTO> desgloseActivos = new List<IndicadoresOTPorActivoDTO>();
            var ordenesAgrupadasPorActivo = ordenes.GroupBy(o => o.IdActivo);

            foreach (var item in ordenesAgrupadasPorActivo)
            {
                desgloseActivos.Add(new IndicadoresOTPorActivoDTO
                {
                    IdActivo = item.Key,
                    Indicadores = CalcularPorcentajes(item.ToList(), item.Count())
                });
            }

            desgloseActivos = desgloseActivos.OrderByDescending(d => d.Indicadores.TotalOrdenes).ToList();

            return desgloseActivos;
        }

        private void CalcularFormulasPeriodos(
            KPIsConfiabilidadDTO kpis, 
            List<KPIsConfiabilidadAuxDTO> ordenes, 
            string vista, 
            int minPeriodoGlobal, 
            int contadorPeriodos,
            int periodosTotalesDelFirstYearSegunVista, 
            int tiempoActivoCritico, 
            int tiempoActivoNoCritico,
            bool todosLosMeses
        )
        {
            var groupedByPeriodo = ordenes.GroupBy(o => new { o.Periodo, Anio = o.FechaApertura.Year }).ToList();
            
            // Obtener los años únicos
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();

            // Crea una lista de periodos completa desde 1 hasta maxPeriodoGlobal
            IEnumerable<(int, int)> todosLosPeriodos;
            if (periodosTotalesDelFirstYearSegunVista != 0)
            {
                var periodosFirstYear = Enumerable.Range(minPeriodoGlobal, periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1).ToList().Select(p => (years[0], p));
                var periodosLastYear = Enumerable.Range(1, contadorPeriodos - (periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1)).ToList().Select(p => (years[1], p));

                todosLosPeriodos = periodosFirstYear.Concat(periodosLastYear).ToList();
            }
            else if (vista == "MENSUAL" && todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (years[0], p));
            }
            else if (vista == "MENSUAL" && !todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (p, p));
            }
            else if (vista == "ANUAL")
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (p, p));
            }
            else
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (years[0], p));
            }

            foreach (var periodo in todosLosPeriodos)
            {
                // Buscar las órdenes para el periodo actual, si no existen, utilizar una lista vacía
                var ordenesPeriodo = groupedByPeriodo.FirstOrDefault(g => g.Key.Periodo == periodo.Item2 && g.Key.Anio == periodo.Item1)?.ToList() ?? new List<KPIsConfiabilidadAuxDTO>();
                var coletilla = years.Count() > 1 ? $" del {periodo.Item1}" : "";

                var ordenesConfiabilidadPeriodo = new ConfiabilidadPeriodoDTO()
                {
                    Periodo = periodo.Item2,
                    NombrePeriodo = vista == "SEMANAL"
                                        ? $"Semana {periodo.Item2} {coletilla}"
                                        : vista == "MENSUAL"
                                            ? ConvertirMes(periodo.Item2) + coletilla
                                                : periodo.Item2.ToString(),
                    OrdenesCerradas = ordenesPeriodo.Select(o => o.IdOrden).Distinct().Count(),
                    Confiabilidad = 100
                };

                if (ordenesPeriodo.Count() != 0)
                {
                    var CountActivosCriticos = ordenesPeriodo.Where(o => o.ActivoCritico).Select(o => o.IdActivo).Distinct().Count();
                    var CountActivosNoCriticos = ordenesPeriodo.Where(o => !o.ActivoCritico).Select(o => o.IdActivo).Distinct().Count();
                    var tiempoFuncionamiento = CountActivosCriticos * tiempoActivoCritico + CountActivosNoCriticos * tiempoActivoNoCritico;
                    var tiempoParada = ordenesPeriodo.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
                    var NumOrdenes = ordenesPeriodo.Select(p => p.IdOrden).Distinct().Count();
                    var tiempoTotalReparaciones = tiempoParada;
                    var NumIncidencias = ordenesPeriodo.Select(p => p.IdIncidencia).Count();
                    var tiempoPeriodo = tiempoActivoNoCritico;

                    var MTBF = CalcularMTBF(tiempoFuncionamiento, tiempoParada, NumOrdenes);
                    var MTTR = CalcularMTTR(tiempoTotalReparaciones, NumIncidencias);

                    ordenesConfiabilidadPeriodo.MTBF = MTBF;
                    ordenesConfiabilidadPeriodo.MTTR = MTTR;
                    ordenesConfiabilidadPeriodo.Disponibilidad = CalcularDisponibilidad(MTBF, MTTR);
                    ordenesConfiabilidadPeriodo.Confiabilidad = CalcularConfiabilidad(MTBF, tiempoPeriodo);
                }

                kpis.GraficasPeriodos.Add(ordenesConfiabilidadPeriodo);
            }
        }

        private void CalcularFormulasActivos(
            out List<ConfiabilidadPeriodoDTO> indicadoresConfiabilidadActivo, 
            List<KPIsConfiabilidadAuxDTO> ordenes, 
            string vista,  
            int minPeriodoGlobal, 
            int contadorPeriodos,
            int periodosTotalesDelFirstYearSegunVista,  
            int tiempoActivoCritico, 
            int tiempoActivoNoCritico,
            bool todosLosMeses
        )
        {
            indicadoresConfiabilidadActivo = new List<ConfiabilidadPeriodoDTO>();

            // Quiero agrupar por tipos de orden, además de por periodos (semanas)
            var groupedByActivoPeriodo = ordenes
                .GroupBy(o => new { o.IdActivo, o.Periodo, Anio = o.FechaApertura.Year }) // Agrupamos por IdActivo, Periodo y Año
                .Select(grupo => new
                {
                    Activo = grupo.Key.IdActivo,
                    ActivoCritico = grupo.Select(g => g.ActivoCritico),
                    grupo.Key.Periodo,
                    grupo.Key.Anio,
                    Ordenes = grupo.OrderBy(o => o.FechaApertura).ToList() // Ordenamos las órdenes
                })
                .OrderBy(g => g.Anio)  // Ordenamos los resultados finales por año
                .ThenBy(g => g.Periodo)
                .ToList();
            
            // Obtener los años únicos
            var years = ordenes.Select(o => o.FechaApertura.Year).Distinct().OrderBy(y => y).ToArray();

            // Crea una lista de periodos completa desde 1 hasta maxPeriodoGlobal
            IEnumerable<(int, int)> todosLosPeriodos;
            if (periodosTotalesDelFirstYearSegunVista != 0)
            {
                var periodosFirstYear = Enumerable.Range(minPeriodoGlobal, periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1).ToList().Select(p => (years[0], p));
                var periodosLastYear = Enumerable.Range(1, contadorPeriodos - (periodosTotalesDelFirstYearSegunVista - minPeriodoGlobal + 1)).ToList().Select(p => (years[1], p));

                todosLosPeriodos = periodosFirstYear.Concat(periodosLastYear).ToList();
            }
            else if (vista == "MENSUAL" && todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (years[0], p));
            }
            else if (vista == "MENSUAL" && !todosLosMeses)
            {                
                todosLosPeriodos = Enumerable.Range(1, 12).ToList().Select(p => (p, p));
            }
            else if (vista == "ANUAL")
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (p, p));
            }
            else
            {
                todosLosPeriodos = Enumerable.Range(minPeriodoGlobal, contadorPeriodos).ToList().Select(p => (years[0], p));
            }

            // Obtener los tipos de orden únicos
            var activosUnicos = ordenes.Select(o => o.IdActivo).Distinct().ToList();

            foreach (var activo in activosUnicos)
            {
                foreach (var periodo in todosLosPeriodos)
                {
                    var coincidencia = groupedByActivoPeriodo.FirstOrDefault(t => t.Activo == activo && t.Periodo == periodo.Item2 && t.Anio == periodo.Item1);
                    var coletilla = years.Count() > 1 ? $" del {periodo.Item1}" : "";

                    var indicadoresOrdenesPeriodoDelActivo = new ConfiabilidadPeriodoDTO()
                    {
                        Periodo = periodo.Item2,
                        NombrePeriodo = vista == "SEMANAL"
                                        ? $"Semana {periodo.Item2} {coletilla}"
                                        : vista == "MENSUAL"
                                            ? ConvertirMes(periodo.Item2) + coletilla
                                                : periodo.Item2.ToString(),
                        OrdenesCerradas = coincidencia != null
                                                ? coincidencia.Ordenes
                                                    .GroupBy(o => o.IdOrden)
                                                    .Distinct()
                                                    .Count()
                                                : 0,
                        Confiabilidad = 100,
                        IdActivo = activo
                    };

                    // Para cada activo en cada periodo calculo los indicadores correspondientes
                    if (coincidencia != null)
                    {
                        bool esActivoCritico = coincidencia.ActivoCritico.First();
                        var tiempoFuncionamiento = esActivoCritico ? tiempoActivoCritico : tiempoActivoNoCritico;
                        var tiempoParada = coincidencia.Ordenes.Where(p => p.FechaResolucion != null).Sum(p => Math.Round(((DateTime)p.FechaResolucion!).Subtract(p.FechaDeteccion).TotalHours));
                        var numOrdenes = coincidencia.Ordenes.Select(p => p.IdOrden).Distinct().Count();
                        var numIncidencias = coincidencia.Ordenes.Select(i => i.IdIncidencia).Count();
                        var tiempoTotalReparaciones = tiempoParada;

                        var MTBF = CalcularMTBF(tiempoFuncionamiento, tiempoParada, numOrdenes);
                        var MTTR = CalcularMTTR(tiempoTotalReparaciones, numIncidencias);

                        indicadoresOrdenesPeriodoDelActivo.MTBF = MTBF;
                        indicadoresOrdenesPeriodoDelActivo.MTTR = MTTR;
                        indicadoresOrdenesPeriodoDelActivo.Disponibilidad = CalcularDisponibilidad(MTBF, MTTR);
                        indicadoresOrdenesPeriodoDelActivo.Confiabilidad = CalcularConfiabilidad(MTBF, tiempoActivoNoCritico);
                    }

                    indicadoresConfiabilidadActivo.Add(indicadoresOrdenesPeriodoDelActivo);
                }
            }
        }

        private double CalcularMTBF(
            double tiempoFuncionamiento, 
            double tiempoParada, 
            int NumOrdenes
        )
        {
            return Math.Round((tiempoFuncionamiento - tiempoParada) / NumOrdenes);
        }

        private double CalcularMTTR(
            double tiempoTotalReparaciones, 
            int NumIncidencias
        )
        {
            return Math.Round(tiempoTotalReparaciones / NumIncidencias);
        }

        private double CalcularDisponibilidad(
            double MTBF, 
            double MTTR
        )
        {
            var disponibilidad = MTBF / (MTBF + MTTR) * 100;

            if (MTBF == 0 && MTTR == 0)
                disponibilidad = 100; 
            else if (disponibilidad < 0)
                disponibilidad = 0;

            return disponibilidad;
        }

        private double CalcularConfiabilidad(
            double MTBF, 
            double tiempoPeriodo
        )
        {
            double tasaRefresco = 1 / MTBF;
            double exponente = -tasaRefresco * tiempoPeriodo;
            double e = Math.E;

            return Math.Pow(e, exponente) * 100;
        }

        private string ConvertirMes(
            int periodo
        )
        {
            Dictionary<int, string> meses = new Dictionary<int, string>()
            {
                { 1, "Ene" },
                { 2, "Feb" },
                { 3, "Mar" },
                { 4, "Abr" },
                { 5, "May" },
                { 6, "Jun" },
                { 7, "Jul" },
                { 8, "Ago" },
                { 9, "Sep" },
                { 10, "Oct" },
                { 11, "Nov" },
                { 12, "Dic" },
            };

            string mes = "";
            meses.TryGetValue(periodo, out mes!);

            return mes;
        }
    }
}
