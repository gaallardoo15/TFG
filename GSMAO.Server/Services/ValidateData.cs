using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GSMAO.Server.Services
{
    /// <summary>
    /// Servicio para la validación compuesta de datos.
    /// </summary>
    public class ValidateData
    {
        private readonly UsersDAO _userDAO;
        private readonly RolesDAO _roleDAO;
        private readonly EstadosUsuariosDAO _stateDAO;
        private readonly EmpresaDAO _empresaDAO;
        private readonly PlantaDAO _plantaDAO;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly EstadosOrdenesDAO _estadoOrdenDAO;
        private readonly IncidenciaDAO _incidenciaDAO;
        private readonly TiposOrdenesDAO _tipoOrdenDAO;

        public ValidateData(
            UsersDAO userDAO, 
            RolesDAO roleDAO, 
            EstadosUsuariosDAO stateDAO, 
            EmpresaDAO empresaDAO, 
            PlantaDAO plantaDAO,
            OrdenesDAO ordenesDAO,
            EstadosOrdenesDAO estadoOrdenDAO,
            IncidenciaDAO incidenciaDAO,
            TiposOrdenesDAO tipoOrdenDAO
        )
        {
            _userDAO = userDAO;
            _roleDAO = roleDAO;
            _stateDAO = stateDAO;
            _empresaDAO = empresaDAO;
            _plantaDAO = plantaDAO;
            _ordenesDAO = ordenesDAO;
            _estadoOrdenDAO = estadoOrdenDAO;
            _incidenciaDAO = incidenciaDAO;
            _tipoOrdenDAO = tipoOrdenDAO;
        }

        /// <summary>
        /// Método que valida la información de un usuario.
        /// </summary>
        /// <param name="rolUsuarioLogueado">Nombre del rol del usuario que hizo la solicitud.</param>
        /// <param name="createUserDTO">La información del usuario a crear. <see cref="CreateUserDTO"/></param>
        /// <exception cref="ValidationException">Cuando no cumple alguna de las validaciones.</exception>
        public async Task IsValidUser(string rolUsuarioLogueado, TempDTO createUserDTO)
        {
            List<string> errorMessages = new List<string>();

            // Comprobar si usuario logueado es Superadmin solo podrá crear Superadmin y Administradores
            var RolSuperadmin = await _roleDAO.GetByNameAsync("SUPER_ADMINISTRADOR", "Name");
            var RolAdmin = await _roleDAO.GetByNameAsync("ADMINISTRADOR", "Name");

            // Rol Administrador no puede crear Superadministradores
            if (rolUsuarioLogueado == RolAdmin.Name && createUserDTO.IdRol == RolSuperadmin.Id)
            {
                throw new ValidationException("No puedes crear usuarios Superadministradores y Administradores.");
            }

            // Comprobar si rol Administrador, debe estar indicado el id de la empresa
            if (createUserDTO.IdRol == RolAdmin.Id && createUserDTO.IdEmpresa == null)
            {
                throw new ValidationException("Para crear un usuario Administrador debes indicar la empresa a la que pertenece.");
            }
            else if (createUserDTO.IdRol == RolAdmin.Id && createUserDTO.IdEmpresa != null)
            {
                // Comprobar que la empresa indicada exista
                await _empresaDAO.GetByIdAsync((int)createUserDTO.IdEmpresa);
            }

            // Comprobar si rol Administrador, no debe estar indicado el id de la planta
            if (createUserDTO.IdRol == RolAdmin.Id && createUserDTO.IdPlanta != null)
            {
                throw new ValidationException("Para crear un usuario Administrador no debes indicar la planta a la que pertenece.");
            }

            // Comprobar si rol no Superadministrador ni Administrador, debe indicar la Planta
            if (createUserDTO.IdRol != RolSuperadmin.Id && createUserDTO.IdRol != RolAdmin.Id && createUserDTO.IdPlanta == null)
            {
                throw new ValidationException("Debes indicar la planta a la que pertenece el usuario.");
            }
            else if (createUserDTO.IdRol != RolSuperadmin.Id && createUserDTO.IdRol != RolAdmin.Id && createUserDTO.IdPlanta != null)
            {
                // Comprobar que la planta seleccionada pertenezca a la empresa indicada
                if (createUserDTO.IdEmpresa != null && createUserDTO.IdPlanta == null)
                {
                    throw new ValidationException("Debes indicar la planta a la que pertenece el usuario.");
                }
                else
                {
                    var planta = await _plantaDAO.GetByIdAsync((int)createUserDTO.IdPlanta!);
                    if (planta.IdEmpresa != createUserDTO.IdEmpresa)
                    {
                        throw new ValidationException("Debes indicar una planta perteneciente a la empresa indicada.");
                    }
                }
            }            
        }

        /// <summary>
        /// Método que valida la información de una orden.
        /// </summary>
        /// <param name="updateOrdenDTO">La información de la orden a actualizar. <see cref="UpdateOrdenDTO"/></param>
        /// <returns></returns>
        /// <exception cref="ValidationException">Cuando no cumple alguna de las validaciones.</exception>
        public async Task<Orden> IsValidOrden(UpdateOrdenDTO updateOrdenDTO)
        {
            // Validar que la orden existe.
            var orden = await _ordenesDAO.GetByIdAsync(updateOrdenDTO.Id);

            if (orden == null)
                throw new ValidationException("La orden no existe.");

            // Validar que el estado que queremos asignar exista.
            var estadoOrden = await _estadoOrdenDAO.GetByIdAsync(updateOrdenDTO.IdEstadoOrden);

            // Validar que el tipo de orden exista
            await _tipoOrdenDAO.GetByIdAsync(updateOrdenDTO.IdTipoOrden);

            // Obtener las incidencias de la orden para verificar fechas.
            var incidenciasOrden = await _incidenciaDAO.GetOrdenIncidencias(updateOrdenDTO.Id);

            // 1. Se debe tener una fecha de cierre.
            if (updateOrdenDTO.FechaApertura == null || updateOrdenDTO.FechaApertura == "")
                throw new ValidationException("Debe especificar la fecha de apertura de la orden.");

            // 2. La fecha de apertura debe ser anterior a cualquier fecha de detección de las incidencias de la orden.
            if (DateTime.Parse(updateOrdenDTO.FechaApertura) > orden.IncidenciasOrden.Min(io => io.FechaDeteccion))
                throw new ValidationException("La fecha de apertura no puede ser posterior a ninguna de las fechas de detección de las incidencias de la orden.");

            // 3. La fecha de apertura debe ser anterior a cualquier fecha de resolución de las incidencias de la orden.
            if (DateTime.Parse(updateOrdenDTO.FechaApertura) > orden.IncidenciasOrden.Min(io => io.FechaResolucion))
                throw new ValidationException("La fecha de apertura no puede ser posterior a ninguna de las fechas de resolución de las incidencias de la orden.");

            // 4. Si se cierra la orden:
            if (estadoOrden.Name.Contains("Cerrada"))
            {
                // 4.1. Todas las incidencias deben estar resueltas.
                if (incidenciasOrden.Any(i => i.IdResolucion == null))
                    throw new ValidationException("No se puede cerrar la orden porque hay incidencias sin resolver.");

                // 4.2. Se debe tener una fecha de cierre.
                if (updateOrdenDTO.FechaCierre == null || updateOrdenDTO.FechaCierre == "")
			        throw new ValidationException("Debe especificar la fecha de cierre de la orden para poder cerrar la orden.");

                // 4.3. La fecha de cierre tiene que ser posterior a la fecha de apertura.
                if (DateTime.Parse(updateOrdenDTO.FechaCierre) < DateTime.Parse(updateOrdenDTO.FechaApertura))
            		throw new ValidationException("La fecha de cierre no puede ser anterior a la fecha de apertura de la orden.");

                // 4.4. La fecha de cierre debe ser anterior a la fecha actual.
                if (DateTime.Parse(updateOrdenDTO.FechaCierre) > DateTime.Now)
			        throw new ValidationException("La fecha de cierre no puede ser posterior a la fecha actual.");

                // 4.5. La fecha de cierre debe ser posterior a la fecha de detección de la incidencia MAX.
                if (DateTime.Parse(updateOrdenDTO.FechaCierre) < incidenciasOrden.Max(io => io.FechaDeteccion))
			        throw new ValidationException("La fecha de cierre no puede ser anterior a las fechas de detección de las incidencias de la orden.");

                // 4.6. La fecha de cierre debe ser posterior a la fecha de resolución de la incidencia MAX.
                if (DateTime.Parse(updateOrdenDTO.FechaCierre) < incidenciasOrden.Max(io => io.FechaResolucion))
			        throw new ValidationException("La fecha de cierre no puede ser anterior a las fechas de resolución de las incidencias de la orden.");
            }
            else
            {
                // Si la orden no está en estado cerrada, no se puede especificar la fecha de cierre.
                if (updateOrdenDTO.FechaCierre != null && updateOrdenDTO.FechaCierre != "")
			        throw new ValidationException("Cambiar el estado a cerrada para poder especificar la fecha de cierre.");
            }

            // 5. Si la orden se está modificando y no creando:
            if (orden.Confirmada)
            {
                // 5.1. Cualquier estado de Materiales debe tener su campo de Materiales completo:
                if (estadoOrden.Name.Contains("Material"))
                {
                    // El campo de materiales debe estar completo.
                    if (updateOrdenDTO.Materiales == null || updateOrdenDTO.Materiales == "")
                        throw new ValidationException("Debe especificar los materiales de la orden para poder cambiar el estado.");
                }

                // 5.2. En los estados "Cerrada" y "Anulada" no se puede modificar el campo de Materiales.
                if ((estadoOrden.Name.Equals("Cerrada") || estadoOrden.Name.Equals("Anulada")) && orden.Materiales != updateOrdenDTO.Materiales)
                    throw new ValidationException($"No se puede modificar el campo de Materiales cuando está en estado {estadoOrden.Name}.");

                // 5.3. La orden solamente puede cambiar de estado "Anulada" a "Abierta".
                if (orden.EstadoOrden!.Name == "Anulada" && estadoOrden.Name != "Abierta")
			        throw new ValidationException("Solamente puede cambiar el estado de la orden a 'Abierta' porque está actualmente 'Anulada'.");
            }

            return orden;
        }
    }
}

