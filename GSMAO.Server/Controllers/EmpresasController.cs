using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.Database;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR")]
    [Produces("application/json")]
    public class EmpresasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EmpresasController> _logger;
        private readonly EmpresaDAO _empresaDAO;

        public EmpresasController(IMapper mapper, ILogger<EmpresasController> logger, EmpresaDAO empresaDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _empresaDAO = empresaDAO;
        }

        /// <summary>
        /// Obtiene la información de las Empresas creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de empresas se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{EmpresaDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpresaDTO>>> RetrieveEmpresas()
        {
            // Obtener el rol del token
            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();

            // Comprobamos si el usuario tiene rol Superadministrador
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            IEnumerable<Empresa> empresas;
            if (esSuperadmin)
            {
                empresas = await _empresaDAO.GetAllAsync();
            }
            else
            {
                // Obtener el id del usuario logueado, para obtener su empresa actual
                var idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var empresa = await _empresaDAO.GetEmpresaAsync(idUser!);

                // Agregar elemento a una lista auxiliar
                var lista = new List<Empresa> { empresa };

                // Asignar la lista auxiliar al IEnumerable
                empresas = lista;
            }

            return Ok(_mapper.Map<List<EmpresaDTO>>(empresas));
        }

        /// <summary>
        /// Crea una empresa en el sistema.
        /// </summary>
        /// <param name="createEmpresaDTO">Objeto que contiene la descripción de la empresa.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La empresa ha sido creada correctamente y se devuelve el objeto creado <see cref="EmpresaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la empresa.
        ///     </description></item>
        /// </list></returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR")]
        [HttpPost]
        public async Task<ActionResult<EmpresaDTO>> CreateEmpresa([FromBody] CreateEmpresaDTO createEmpresaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newEmpresa = _mapper.Map<Empresa>(createEmpresaDTO);

                var createdEmpresa = await _empresaDAO.CreateAsync(newEmpresa);
                
                _logger.LogInformation($"Nueva empresa creada (Id: {createdEmpresa!.Id})");
                
                return Ok(_mapper.Map<EmpresaDTO>(createdEmpresa));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la empresa {createEmpresaDTO.Descripcion}: {ex.Message}");
                
                return StatusCode(500, $"Error al crear la empresa.");
            }

        }

        /// <summary>
        /// Actualiza una empresa existente del sistema.
        /// </summary>
        /// <param name="id">Identificador de la empresa a actualizar.</param>
        /// <param name="empresaDTO">Objeto que contiene la nueva descripción de la empresa.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La empresa ha sido creada correctamente y se devuelve el objeto creado <see cref="EmpresaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la empresa.
        ///     </description></item>
        /// </list></returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EmpresaDTO>> UpdateEmpresa(int id, [FromBody] EmpresaDTO empresaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != empresaDTO.Id)
            {
                return BadRequest("El identificador de empresa no coincide con los datos del JSON enviado.");
            }

            try
            {
                var empresa = await _empresaDAO.GetByIdAsync(empresaDTO.Id);

                _mapper.Map(empresaDTO, empresa);

                await _empresaDAO.UpdateAsync(empresa);
                empresa = await _empresaDAO.GetByIdAsync(id);

                _logger.LogInformation($"Empresa actualizada (id: {empresa.Id})");
                return Ok(_mapper.Map<EmpresaDTO>(empresa));
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error al editar la empresa: {ex.Message}");
                return StatusCode(500, $"Error al editar la empresa.");
            }
        }
    }
}
