using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<InformesController> _logger;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly GeneradorExcel _generadorInformeExcel;

        public InformesController(IMapper mapper, ILogger<InformesController> logger, OrdenesDAO ordenesDAO, GeneradorExcel generadorInformeExcel)
        {
            _mapper = mapper;
            _logger = logger;
            _ordenesDAO = ordenesDAO;
            _generadorInformeExcel = generadorInformeExcel;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InformeDTO>?>> RetrieveInforme(string? fechaDesde, string? fechaHasta, int? idActivo)
        {
            IEnumerable<InformeDTO>? informe = new List<InformeDTO>();

            try
            {
                // Compruebo si tengo un Activo especificado o no
                if (idActivo != null && idActivo > 0)
                {
                    // Obtener todas las órdenes del activa especificado según los filtros de fechas
                    informe = await _ordenesDAO.GetInformeAsync((int)idActivo!, fechaDesde, fechaHasta);
                }
                else
                {
                    // Obtener todas las órdenes según los filtros de fechas
                    informe = await _ordenesDAO.GetInformeAsync(fechaDesde, fechaHasta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el informe: {ex.Message}");

                return StatusCode(500, $"Error al obtener el informe: {ex.Message}");
            }

            return Ok(informe);
        }

        [HttpGet("descargar")]
        public async Task<IActionResult> DescargarInforme(string? fechaDesde, string? fechaHasta, int? idActivo, bool antiguo)
        {
            List<InformeExcelDTO>? informe = new List<InformeExcelDTO>();

            try
            {
                // Compruebo si tengo un Activo especificado o no
                if (idActivo != null && idActivo > 0)
                {
                    // Obtener todas las órdenes del activa especificado según los filtros de fechas
                    informe = await _ordenesDAO.GetInformeExcelAsync((int)idActivo!, fechaDesde, fechaHasta);
                }
                else
                {
                    // Obtener todas las órdenes según los filtros de fechas
                    informe = await _ordenesDAO.GetInformeExcelAsync(fechaDesde, fechaHasta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el informe de Excel: {ex.Message}");

                return StatusCode(500, $"Error al obtener el informe de Excel: {ex.Message}");
            }

            try
            {
                if (informe == null || informe.Count == 0)
                    throw new Exception("El informe no contiene datos, por lo que no está permitida la descarga.");

                MemoryStream buffer = await _generadorInformeExcel.GenerarInformeExcelAsync( informe, antiguo);

                // Compruebo si tengo un Activo especificado o no para indicar el nombre del fichero
                string nombreFichero = idActivo != null && idActivo > 0
                                        ? $"Ordenes_Activo_{idActivo}.xlsx"
                                        : "Ordenes_Activo.xlsx";

                return File(buffer.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreFichero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                if (ex.Message.Contains("El informe no contiene datos"))
                    return NotFound(ex.Message);
                else
                    return StatusCode(500, ex.Message);
            }
        }
    }
}
