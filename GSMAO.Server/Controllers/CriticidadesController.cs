using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriticidadesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CriticidadesController> _logger;
        private readonly CriticidadDAO _criticidadDAO;

        public CriticidadesController(IMapper mapper, ILogger<CriticidadesController> logger, CriticidadDAO criticidadDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _criticidadDAO = criticidadDAO;
        }

        /// <summary>
        /// Obtiene la información de las Criticidades creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de criticidades se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{CriticidadDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CriticidadDTO>>> RetrieveCriticidades()
        {
            var criticidades = await _criticidadDAO.GetAllAsync();
            return Ok(_mapper.Map<List<CriticidadDTO>>(criticidades));
        }
    }
}
