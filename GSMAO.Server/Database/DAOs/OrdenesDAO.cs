using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing.Printing;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace GSMAO.Server.Database.DAOs
{
    public class OrdenesDAO : BaseDAO<Orden, int>
    {
        private readonly IMapper _mapper;
        private readonly ComponenteDAO _componentesDAO;

        public OrdenesDAO(ApplicationDbContext context, ILogger<OrdenesDAO> logger, IMapper mapper, ComponenteDAO componentesDAO) : base(context, logger)
        {
            _mapper = mapper;
            _componentesDAO = componentesDAO;
         }

        /// <summary>
        /// Obtiene una lista de órdenes de trabajo, aplicando filtros según el rol del usuario y otros parámetros como años, tipos de orden, criticidades, y fechas.
        /// </summary>
        /// <param name="filtros">Filtros aplicados para la búsqueda de las órdenes.</param>
        /// <param name="rolUser">Rol del usuario que está realizando la consulta.</param>
        /// <param name="idUser">ID del usuario que está realizando la consulta.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <returns>Lista de objetos <see cref="OrdenTableDTO"/> con los datos de las órdenes.</returns>
        public async Task<List<OrdenTableDTO>> GetOrdenesTableAsync(FiltrosPanelGeneralDTO filtros, int rolUser, string idUser, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.Ordenes
                .AsNoTrackingWithIdentityResolution()
                .Where(o => o.Confirmada == true && (
                    // Filtro según el rol del usuario que está logueado
                    rolUser == 7
                    ? o.UsuariosOrden!.Any(uo => _context.Usuarios
                            .Where(u => new[] { 4, 5, 6, 7 }.Contains(u.Rol.Orden))
                            .Select(u => u.Id)
                            .Contains(uo.Usuario.Id))
                    : rolUser < 4
                        ? o.UsuariosOrden!.Any(uo => uo.Usuario.Rol.Orden >= rolUser!)
                        : o.UsuariosOrden!.Any(uo => uo.Usuario.Rol.Orden >= 4)
                ));

            if (filtros.Years != null && filtros.Years.Count != 0)
            {
                baseQuery = baseQuery.Where(o => filtros.Years.Contains(o.FechaApertura!.Value.Year));
            }

            if (filtros.TiposOrdenes != null && filtros.TiposOrdenes.Count != 0)
            {
                baseQuery = baseQuery.Where(o => filtros.TiposOrdenes.Contains((int)o.IdTipoOrden!));
            }

            if (filtros.Criticidades != null && filtros.Criticidades.Count != 0)
            {
                baseQuery = baseQuery.Where(o => filtros.Criticidades.Contains(o.Activo!.IdCriticidad));
            }

            if (filtros.FechaDesde != null)
            {
                baseQuery = baseQuery.Where(o => o.FechaApertura >= filtros.FechaDesde);
            }

            if (filtros.FechaHasta != null)
            {
                baseQuery = baseQuery.Where(o => o.FechaApertura <= filtros.FechaHasta);
            }

            var query = baseQuery.Select(o => new
            {
                o.Id,
                o.IdSAP,
                o.FechaApertura,
                o.FechaCierre,
                Activo = new { o.Activo!.Id, o.Activo.DescripcionES },
                Estado = new { o.EstadoOrden!.Id, o.EstadoOrden.Name, o.EstadoOrden.Orden },
                Tipo = new { o.TipoOrden!.Id, o.TipoOrden.Name },
                MecanismosDeFallos = o.IncidenciasOrden!
                    .Select(io => new
                    {
                        io.Incidencia.MecanismoDeFallo.Id,
                        io.Incidencia.MecanismoDeFallo.DescripcionES
                    }),
                Incidencias = o.IncidenciasOrden!
                    .Select(io => new
                    {
                        io.Incidencia.Id,
                        io.Incidencia.DescripcionES,
                        io.Incidencia.IdMecanismoFallo,
                        MecanismoDeFalloDescripcion = io.Incidencia.MecanismoDeFallo.DescripcionES
                    }),
                Usuarios = o.UsuariosOrden!
                    .Select(uo => new { uo.Usuario.Id, uo.Usuario.Nombre, uo.Usuario.Apellidos }),
                o.ComentarioOrden,
                contieneOperario = o.UsuariosOrden!.Any(uo => uo.Usuario.Id == idUser), // Verifica si el usuario que consulta está asignado a las órdenes
                orden = o.EstadoOrden.Orden,
                valorCriticidad = o.Activo.ValorCriticidad,
                fechaCreacion = o.FechaApertura
            });

            // Aplicar el ordenamiento unificado
            if (rolUser == 7)
            {
                query = query.OrderByDescending(o => o.contieneOperario)
                .ThenBy(o => o.contieneOperario ? o.orden : o.orden)
                //.ThenBy(o => o.orden)
                .ThenByDescending(o => o.valorCriticidad)
                .ThenByDescending(o => o.fechaCreacion);
            }
            else
            {
                query = query.OrderBy(o => o.orden)
                .ThenByDescending(o => o.valorCriticidad)
                .ThenByDescending(o => o.fechaCreacion);
            }

            var datos = await query.ToListAsync(cancellationToken);

            var ordenes = datos.Select(o => new OrdenTableDTO
            {
                Id = o.Id,
                IdSAP = o.IdSAP,
                FechaApertura = o.FechaApertura!.Value,
                FechaCierre = o.FechaCierre,
                Activo = new ActivoMinDTO { Id = o.Activo.Id, DescripcionES = o.Activo.DescripcionES },
                Estado = new EstadoOrdenDTO { Id = o.Estado.Id, Name = o.Estado.Name, Orden = o.Estado.Orden },
                Tipo = new TipoOrdenDTO { Id = o.Tipo.Id, Name = o.Tipo.Name },
                MecanismosDeFallos = o.MecanismosDeFallos
                    .Select(mf => new MecanismoDeFalloDTO { Id = mf.Id, DescripcionES = mf.DescripcionES })
                    .Distinct()
                    .ToList(),
                Incidencias = o.Incidencias
                    .Select(i => new IncidenciaDTO
                    {
                        Id = i.Id,
                        DescripcionES = i.DescripcionES,
                        MecanismoDeFallo = new MecanismoDeFalloDTO { Id = i.IdMecanismoFallo, DescripcionES = i.MecanismoDeFalloDescripcion }
                    })
                    .ToList(),
                Usuarios = o.Usuarios
                    .Select(u => new UsuarioOrdenDTO { Id = u.Id, Nombre = u.Nombre, Apellidos = u.Apellidos })
                    .ToList(),
                ComentarioOrden = o.ComentarioOrden ?? ""
            }).ToList();

            return ordenes;
        }

        /// <summary>
        /// Obtiene una orden por su ID, incluyendo la información relacionada de las entidades asociadas como activo, estado, tipo de orden, usuarios y incidencias.
        /// </summary>
        /// <param name="id">ID de la orden a recuperar.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <returns>Objeto <see cref="Orden"/> con los datos de la orden solicitada.</returns>
        /// <exception cref="KeyNotFoundException">Lanzado si no se encuentra una orden con el ID especificado.</exception>
        protected override async Task<Orden> GetByIdInternalAsync(int id, CancellationToken cancellationToken = default)
        {
            var orden = await _context.Ordenes
                                    .Where(o => o.Id == id)
                                    .Include(o => o.Activo!)
                                        .ThenInclude(o => o.CentroCoste)
                                    .Include(o => o.EstadoOrden)
                                    .Include(o => o.TipoOrden)
                                    .Include(o => o.UsuariosOrden)
                                        .ThenInclude(o => o.Usuario)
                                    .Include(o => o.IncidenciasOrden)
                                        .ThenInclude(o => o.Incidencia)
                                        .ThenInclude(o => o.MecanismoDeFallo)
                                    .Include(o => o.IncidenciasOrden)
                                        .ThenInclude(o => o.Resolucion)
                                    .Include(o => o.IncidenciasOrden)
                                        .ThenInclude(o => o.Componente)
                                    .Include(o => o.Usuario)
                                    .FirstOrDefaultAsync(cancellationToken);

            if (orden == null)
                throw new KeyNotFoundException($"No hay órdenes con el id seleccionado");

            return orden;
        }

        /// <summary>
        /// Obtiene los años disponibles en las órdenes, filtrados por la fecha de apertura.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <returns>Lista de años disponibles en las órdenes.</returns>
        public async Task<IEnumerable<int>> GetYears(CancellationToken cancellationToken = default)
        {
            var years = await _context.Ordenes
                                    .AsNoTracking() // Mejora el rendimiento al no rastrear cambios
                                    .Where(o => o.FechaApertura != null)
                                    .GroupBy(o => o.FechaApertura!.Value.Year)
                                    .Select(g => g.Key) // Obtenemos solo el año
                                    .OrderBy(year => year) // Ordenamos los años
                                    .ToListAsync();

            return years;
        }

        /// <summary>
        /// Elimina una relación de usuario y orden, registrando la eliminación en el historial de cambios.
        /// </summary>
        /// <param name="idUser">ID del usuario a eliminar.</param>
        /// <param name="idOrden">ID de la orden de la que se eliminará el usuario.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <exception cref="KeyNotFoundException">Lanzado si no se encuentra la relación de usuario y orden a eliminar.</exception>
        public async Task DeleteUsuarioOrdenAsync(string idUser, int idOrden, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Usuarios_Ordenes.Where(uo => uo.IdUsuario == idUser && uo.IdOrden == idOrden).FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException(
                    $"No se puede eliminar la entidad {typeof(Usuario_Orden).Name} con idUser {idUser} e idOrden {idOrden} porque no existe.");

            await HandleWithLoggingAndExceptionAsync(
                async ct => {
                    // Eliminación en la tabla Usuario_Orden
                    _context.Set<Usuario_Orden>().Remove(entity);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Inserción en la tabla HistorialModificaciones
                    var historialModificacion = new HistorialCambiosUsuarioOrden
                    {
                        IdOrden = idOrden,
                        IdUsuarioOrigen = idUser,
                        IdUsuarioDestino = null
                    };

                    await _context.Set<HistorialCambiosUsuarioOrden>().AddAsync(historialModificacion, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
                $"Eliminando la entidad {typeof(Usuario_Orden).Name} y registrando nueva entidad {typeof(HistorialCambiosUsuarioOrden).Name}",
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Elimina múltiples relaciones de usuario y orden, registrando las eliminaciones en el historial de cambios.
        /// </summary>
        /// <param name="usersIds">Lista de IDs de usuarios a eliminar.</param>
        /// <param name="idOrden">ID de la orden de la que se eliminarán los usuarios.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <exception cref="ArgumentException">Lanzado si la lista de usuarios está vacía.</exception>
        /// <exception cref="KeyNotFoundException">Lanzado si no se encuentran relaciones de usuario y orden a eliminar.</exception>
        public async Task DeleteUsuariosOrdenAsync(List<string> usersIds, int idOrden, CancellationToken cancellationToken = default)
        {
            if (usersIds == null || !usersIds.Any())
                throw new ArgumentException("La lista de usuarios no puede estar vacía.", nameof(usersIds));

            var entities = await _context.Usuarios_Ordenes
                .Where(uo => usersIds.Contains(uo.IdUsuario) && uo.IdOrden == idOrden)
                .ToListAsync(cancellationToken);

            if (!entities.Any())
                throw new KeyNotFoundException(
                    $"No se encontraron entidades {typeof(Usuario_Orden).Name} con los usuarios proporcionados e idOrden {idOrden}.");

            await HandleWithLoggingAndExceptionAsync(
                async ct =>
                {
                    // Eliminación de entidades en la tabla Usuario_Orden
                    _context.Set<Usuario_Orden>().RemoveRange(entities);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Inserción en la tabla HistorialModificaciones para cada usuario eliminado
                    var historialModificaciones = entities.Select(entity => new HistorialCambiosUsuarioOrden
                    {
                        IdOrden = idOrden,
                        IdUsuarioOrigen = entity.IdUsuario,
                        IdUsuarioDestino = null
                    }).ToList();

                    await _context.Set<HistorialCambiosUsuarioOrden>().AddRangeAsync(historialModificaciones, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
                $"Eliminando entidades {typeof(Usuario_Orden).Name} y registrando nuevas entidades {typeof(HistorialCambiosUsuarioOrden).Name} para los usuarios proporcionados.",
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Crea una nueva relación de usuario y orden, registrando la creación en el historial de cambios.
        /// </summary>
        /// <param name="user_orden">Entidad <see cref="Usuario_Orden"/> que representa la relación a crear.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <returns>Entidad <see cref="Usuario_Orden"/> creada.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si la entidad proporcionada es nula.</exception>
        public async Task<Usuario_Orden> CreateUsuarioOrdenAsync(Usuario_Orden user_orden, CancellationToken cancellationToken = default)
        {
            if (user_orden == null)
                throw new ArgumentNullException(nameof(user_orden));

            await HandleWithLoggingAndExceptionAsync(
                async ct => {
                    // Inserción en la tabla Usuario_Orden
                    await _context.Set<Usuario_Orden>().AddAsync(user_orden, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Inserción en la tabla HistorialModificaciones
                    var historialModificacion = new HistorialCambiosUsuarioOrden
                    {
                        IdOrden = user_orden.IdOrden,
                        IdUsuarioOrigen = null,
                        IdUsuarioDestino = user_orden.IdUsuario
                    };

                    await _context.Set<HistorialCambiosUsuarioOrden>().AddAsync(historialModificacion, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
                $"Agregando nueva entidad {typeof(Usuario_Orden).Name} y registrando nueva entidad {typeof(HistorialCambiosUsuarioOrden).Name}",
                cancellationToken
            ).ConfigureAwait(false);

            return user_orden;
        }

        /// <summary>
        /// Crea múltiples relaciones de usuario y orden, registrando la creación de cada relación en el historial de cambios.
        /// </summary>
        /// <param name="users">Lista de entidades <see cref="Usuario_Orden"/> que representan las relaciones a crear.</param>
        /// <param name="idOrden">ID de la orden en la que se crearán las relaciones.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <exception cref="ArgumentException">Lanzado si la lista de usuarios está vacía.</exception>
        /// <exception cref="KeyNotFoundException">Lanzado si no se encuentran relaciones de usuario y orden a crear.</exception>
        public async Task CreateUsuarioOrdenAsync(List<Usuario_Orden> users, int idOrden, CancellationToken cancellationToken = default)
        {
            if (users == null || !users.Any())
                throw new ArgumentException("La lista de usuarios no puede estar vacía.", nameof(users));

            var entities = await _context.Usuarios_Ordenes
                .Where(uo => users.Select(u => u.IdUsuario).Contains(uo.IdUsuario) && uo.IdOrden == idOrden)
                .ToListAsync(cancellationToken);

            if (entities.Any())
                throw new KeyNotFoundException(
                    $"Alguno de los usuarios que deseas asignar a la orden ya se encuentran asignados a la orden {idOrden}.");

            await HandleWithLoggingAndExceptionAsync(
                async ct =>
                {
                    // Inserción de entidades en la tabla Usuario_Orden
                    await _context.Set<Usuario_Orden>().AddRangeAsync(users, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Inserción en la tabla HistorialModificaciones para cada usuario eliminado
                    var historialModificaciones = users.Select(entity => new HistorialCambiosUsuarioOrden
                    {
                        IdOrden = idOrden,
                        IdUsuarioOrigen = null,
                        IdUsuarioDestino = entity.IdUsuario
                    }).ToList();

                    await _context.Set<HistorialCambiosUsuarioOrden>().AddRangeAsync(historialModificaciones, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
                $"Agregando entidades {typeof(Usuario_Orden).Name} y registrando nuevas entidades {typeof(HistorialCambiosUsuarioOrden).Name} para los usuarios proporcionados.",
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Cambia la relación de un usuario en una orden, registrando el cambio en el historial de modificaciones.
        /// </summary>
        /// <param name="reasignarUsuarioOrdenDTO">DTO con los detalles de la re-asignación del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
        /// <returns>Entidad <see cref="Usuario_Orden"/> actualizada.</returns>
        /// <exception cref="KeyNotFoundException">Lanzado si no se encuentra la relación de usuario y orden a modificar.</exception>
        public async Task<Usuario_Orden> ChangeUsuarioOrdenAsync(ReasignarUsuarioOrdenDTO reasignarUsuarioOrdenDTO, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Usuarios_Ordenes.Where(uo => uo.IdOrden == reasignarUsuarioOrdenDTO.idOrden && uo.IdUsuario == reasignarUsuarioOrdenDTO.idUsuarioOrigen).FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Entidad no encontrada (IdOrden: {reasignarUsuarioOrdenDTO.idOrden}, IdUsuario: {reasignarUsuarioOrdenDTO.idUsuarioOrigen})");

            var newEntity = _mapper.Map<Usuario_Orden>(reasignarUsuarioOrdenDTO);

            await HandleWithLoggingAndExceptionAsync(
                async ct => {
                    // Cambio de usuario en la tabla Usuario_Orden
                    _context.Usuarios_Ordenes.Remove(entity);
                    await _context.Set<Usuario_Orden>().AddAsync(newEntity).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    // Inserción en la tabla HistorialModificaciones
                    var historialModificacion = new HistorialCambiosUsuarioOrden
                    {
                        IdOrden = reasignarUsuarioOrdenDTO.idOrden,
                        IdUsuarioOrigen = reasignarUsuarioOrdenDTO.idUsuarioOrigen,
                        IdUsuarioDestino = reasignarUsuarioOrdenDTO.idUsuarioDestino
                    };

                    await _context.Set<HistorialCambiosUsuarioOrden>().AddAsync(historialModificacion, cancellationToken).ConfigureAwait(false);
                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
                $"Cambiando una entidad {typeof(Usuario_Orden).Name} y registrando nueva entidad {typeof(HistorialCambiosUsuarioOrden).Name}",
                cancellationToken
            ).ConfigureAwait(false);

            newEntity = await _context.Usuarios_Ordenes.Where(uo => uo.IdOrden == reasignarUsuarioOrdenDTO.idOrden && uo.IdUsuario == reasignarUsuarioOrdenDTO.idUsuarioDestino).Include(uo => uo.Usuario).FirstOrDefaultAsync(cancellationToken);

            return newEntity!;
        }

        /// <summary>
        /// Obtiene un conjunto de KPI (Indicadores Clave de Desempeño) relacionados con las órdenes, filtradas por diversos parámetros como año, mes, centro de coste, criticidad y activo.
        /// </summary>
        /// <param name="year">Año de las órdenes a filtrar. Puede ser nulo para no filtrar por año.</param>
        /// <param name="mes">Mes de las órdenes a filtrar. Puede ser nulo para no filtrar por mes.</param>
        /// <param name="idCentroCoste">ID del centro de coste para filtrar las órdenes. Puede ser nulo para no filtrar por centro de coste.</param>
        /// <param name="idCriticidad">ID de la criticidad para filtrar las órdenes. Puede ser nulo para no filtrar por criticidad.</param>
        /// <param name="idActivo">ID del activo para filtrar las órdenes. Puede ser nulo para no filtrar por activo.</param>
        /// <returns>Una lista de objetos <see cref="KPIsOrdenesAuxDTO"/> que contienen la información clave de las órdenes, incluyendo el ID de la orden, activo, tipo de orden, estado, y fecha de apertura.</returns>
        public async Task<List<KPIsOrdenesAuxDTO>> GetOrdenesKPIs(int year, int mes, int idCentroCoste, int idCriticidad, int idActivo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var criticidad = await _context.Criticidades.Where(c => c.Id == idCriticidad).FirstOrDefaultAsync();

            var query = _context.Ordenes.Where(o => o.Confirmada && o.EstadoOrden!.Name != "Anulada" && o.TipoOrden!.Name != "Predictivo");

            if (year != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => o.FechaApertura!.Value.Year== year);

            if (mes != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => o.FechaApertura!.Value.Month == mes);

            if (idCentroCoste != 0)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idCriticidad != 0)
                query = query.Where(o => criticidad!.Siglas == "SC"
                                        ? o.Activo!.Criticidad == null || o.Activo!.IdCriticidad == idCriticidad
                                        : o.Activo!.IdCriticidad == idCriticidad);

            if (idActivo != 0)
                query = query.Where(o => o.IdActivo == idActivo);

            if (fechaDesde != null)
            {
                query = query.Where(o => o.FechaApertura >= fechaDesde);
            }

            if (fechaHasta != null)
            {
                query = query.Where(o => o.FechaApertura <= fechaHasta);
            }

            var result = await query
                .Select(o => new KPIsOrdenesAuxDTO
                {
                    IdOrden = o.Id,
                    IdActivo = o.IdActivo ?? 0,
                    IdTipoOrden = o.IdTipoOrden ?? 0,
                    IdEstado = o.IdEstadoOrden ?? 0,
                    FechaApertura = (DateTime)o.FechaApertura!
                })
                .ToListAsync();
            
            return result;
        }

        public async Task<List<KPIsOrdenesAuxDTO>> GetOrdenesKPIs(List<int> years, List<int>? meses, int idCentroCoste, int idCriticidad, int idActivo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var criticidad = await _context.Criticidades.Where(c => c.Id == idCriticidad).FirstOrDefaultAsync();

            var query = _context.Ordenes.Where(o => o.Confirmada && o.EstadoOrden!.Name != "Anulada" && o.TipoOrden!.Name != "Predictivo");

            if (years != null && years.Count() != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => years.Contains(o.FechaApertura!.Value.Year));

            if (meses != null && meses.Count() != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => meses.Contains(o.FechaApertura!.Value.Month));

            if (idCentroCoste != 0)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idCriticidad != 0)
                query = query.Where(o => criticidad!.Siglas == "SC"
                                        ? o.Activo!.Criticidad == null || o.Activo!.IdCriticidad == idCriticidad
                                        : o.Activo!.IdCriticidad == idCriticidad);

            if (idActivo != 0)
                query = query.Where(o => o.IdActivo == idActivo);

            if (fechaDesde != null)
            {
                query = query.Where(o => o.FechaApertura >= fechaDesde);
            }

            if (fechaHasta != null)
            {
                query = query.Where(o => o.FechaApertura <= fechaHasta);
            }

            var result = await query
                .Select(o => new KPIsOrdenesAuxDTO
                {
                    IdOrden = o.Id,
                    IdActivo = o.IdActivo ?? 0,
                    IdTipoOrden = o.IdTipoOrden ?? 0,
                    IdEstado = o.IdEstadoOrden ?? 0,
                    FechaApertura = (DateTime)o.FechaApertura!
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<KPIsConfiabilidadAuxDTO>> GetConfiabilidadKPIs(int? year, int? mes, int? idCentroCoste, int? idCriticidad, int? idActivo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var criticidad = await _context.Criticidades.Where(c => c.Id == idCriticidad).FirstOrDefaultAsync();

            var query = _context.Ordenes.Where(o => o.Confirmada && o.EstadoOrden!.Name.Contains("Cerrada") && (o.TipoOrden!.Name == "Correctivo" || o.TipoOrden!.Name == "Falla Humana"));

            if (year != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => o.FechaApertura!.Value.Year== year);

            if (mes != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => o.FechaApertura!.Value.Month == mes);

            if (idCentroCoste != 0)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idCriticidad != 0)
                query = query.Where(o => criticidad!.Siglas == "SC"
                                        ? o.Activo!.Criticidad == null || o.Activo!.IdCriticidad == idCriticidad
                                        : o.Activo!.IdCriticidad == idCriticidad);

            if (idActivo != 0)
                query = query.Where(o => o.IdActivo == idActivo);

            if (fechaDesde != null)
            {
                query = query.Where(o => o.FechaApertura >= fechaDesde);
            }

            if (fechaHasta != null)
            {
                query = query.Where(o => o.FechaApertura <= fechaHasta);
            }

            var result = await query
                .SelectMany(o => o.IncidenciasOrden, (o, incidencia) => new KPIsConfiabilidadAuxDTO
                {
                    IdOrden = o.Id,
                    IdActivo = (int)o.IdActivo!,
                    ActivoCritico = o.Activo!.Criticidad!.Siglas == "SC" ? false : true,
                    FechaApertura = (DateTime)o.FechaApertura!,
                    IdIncidencia = incidencia.Id,
                    FechaDeteccion = incidencia.FechaDeteccion,
                    FechaResolucion = (DateTime)incidencia.FechaResolucion!
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<KPIsConfiabilidadAuxDTO>> GetConfiabilidadKPIs(List<int> years, List<int>? meses, int? idCentroCoste, int? idCriticidad, int? idActivo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var criticidad = await _context.Criticidades.Where(c => c.Id == idCriticidad).FirstOrDefaultAsync();

            var query = _context.Ordenes.Where(o => o.Confirmada && o.EstadoOrden!.Name.Contains("Cerrada") && (o.TipoOrden!.Name == "Correctivo" || o.TipoOrden!.Name == "Falla Humana"));

            if (years != null && years.Count() != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => years.Contains(o.FechaApertura!.Value.Year));

            if (meses != null && meses.Count() != 0 && fechaDesde == null && fechaHasta == null)
                query = query.Where(o => meses.Contains(o.FechaApertura!.Value.Month));

            if (idCentroCoste != 0)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idCriticidad != 0)
                query = query.Where(o => criticidad == null || criticidad!.Siglas == "SC"
                                        ? o.Activo!.Criticidad == null || o.Activo!.IdCriticidad == idCriticidad
                                        : o.Activo!.IdCriticidad == idCriticidad);

            if (idActivo != 0)
                query = query.Where(o => o.IdActivo == idActivo);

            if (fechaDesde != null)
            {
                query = query.Where(o => o.FechaApertura >= fechaDesde);
            }

            if (fechaHasta != null)
            {
                query = query.Where(o => o.FechaApertura <= fechaHasta);
            }

            var result = await query
                .SelectMany(o => o.IncidenciasOrden, (o, incidencia) => new KPIsConfiabilidadAuxDTO
                {
                    IdOrden = o.Id,
                    IdActivo = (int)o.IdActivo!,
                    ActivoCritico = o.Activo!.Criticidad!.Siglas == "SC" ? false : true,
                    FechaApertura = (DateTime)o.FechaApertura!,
                    IdIncidencia = incidencia.Id,
                    FechaDeteccion = incidencia.FechaDeteccion,
                    FechaResolucion = (DateTime)incidencia.FechaResolucion!
                })
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<InformeDTO>?> GetInformeAsync(string? fechaDesde, string? fechaHasta, CancellationToken cancellationToken = default)
        {
            DateTime? parsedFechaDesde = string.IsNullOrEmpty(fechaDesde) ? null : DateTime.Parse(fechaDesde + " 00:00:00");
            DateTime? parsedFechaHasta = string.IsNullOrEmpty(fechaHasta) ? null : DateTime.Parse(fechaHasta + " 23:59:59");

            var query = _context.IncidenciasOrdenes
                                        .Where(io => io.Orden.Confirmada)
                                        .Select(io => new InformeDTO
                                        {
                                            Id = io.Orden.Id,
                                            IdSAP = io.Orden.IdSAP,
                                            TipoOrden = io.Orden.TipoOrden.Name,
                                            EstadoOrden = io.Orden.EstadoOrden.Name,
                                            IdActivo = (int)io.Orden.IdActivo,
                                            ActivoSAP = io.Orden.Activo.ActivoSAP,
                                            Activo = io.Orden.Activo.DescripcionES,
                                            Criticidad = io.Orden.Activo.Criticidad.Siglas,
                                            CentroCosteSAP = io.Orden.Activo.CentroCoste.CentroCosteSAP,
                                            CentroCoste = io.Orden.Activo.CentroCoste.DescripcionES,
                                            FechaApertura = (DateTime)io.Orden.FechaApertura,
                                            FechaCierre = io.Orden.FechaCierre,
                                            Usuarios = string.Join(", ", io.Orden.UsuariosOrden.Select(uo => uo.Usuario.Nombre + " " + uo.Usuario.Apellidos)),
                                            KKSComponente = io.Componente.Denominacion,
                                            Componente = io.Componente.DescripcionES,
                                            MecanismoDeFallo = io.Incidencia.MecanismoDeFallo.DescripcionES,
                                            Incidencia = io.Incidencia.DescripcionES,
                                            Resolucion = io.Resolucion.DescripcionES,
                                            FechaDeteccion = io.FechaDeteccion,
                                            FechaResolucion = io.FechaResolucion,
                                            TiempoParadaIncidencia = CalcularTiempoParada(io.TiempoParada, io.FechaDeteccion, io.FechaResolucion)
                                        });

            if (parsedFechaDesde.HasValue)
            {
                query = query.Where(i => i.FechaApertura >= parsedFechaDesde.Value);
            }

            if (parsedFechaHasta.HasValue)
            {
                query = query.Where(i => i.FechaApertura <= parsedFechaHasta.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<InformeDTO>?> GetInformeAsync(int idActivo, string? fechaDesde, string? fechaHasta, CancellationToken cancellationToken = default)
        {
            DateTime? parsedFechaDesde = string.IsNullOrEmpty(fechaDesde) ? null : DateTime.Parse(fechaDesde + " 00:00:00");
            DateTime? parsedFechaHasta = string.IsNullOrEmpty(fechaHasta) ? null : DateTime.Parse(fechaHasta + " 23:59:59");

            var query = _context.IncidenciasOrdenes
                                        .Where(io => io.Orden.Confirmada)
                                        .Select(io => new InformeDTO
                                        {
                                            Id = io.Orden.Id,
                                            IdSAP = io.Orden.IdSAP,
                                            TipoOrden = io.Orden.TipoOrden!.Name,
                                            EstadoOrden = io.Orden.EstadoOrden!.Name,
                                            IdActivo = (int)io.Orden.IdActivo!,
                                            ActivoSAP = io.Orden.Activo!.ActivoSAP,
                                            Activo = io.Orden.Activo.DescripcionES,
                                            Criticidad = io.Orden.Activo.Criticidad.Siglas,
                                            CentroCosteSAP = io.Orden.Activo.CentroCoste.CentroCosteSAP,
                                            CentroCoste = io.Orden.Activo.CentroCoste.DescripcionES,
                                            FechaApertura = (DateTime)io.Orden.FechaApertura!,
                                            FechaCierre = io.Orden.FechaCierre,
                                            Usuarios = string.Join(", ", io.Orden.UsuariosOrden.Select(uo => uo.Usuario.Nombre + " " + uo.Usuario.Apellidos)),
                                            KKSComponente = io.Componente.Denominacion,
                                            Componente = io.Componente.DescripcionES,
                                            MecanismoDeFallo = io.Incidencia.MecanismoDeFallo.DescripcionES,
                                            Incidencia = io.Incidencia.DescripcionES,
                                            Resolucion = io.Resolucion!.DescripcionES,
                                            FechaDeteccion = io.FechaDeteccion,
                                            FechaResolucion = io.FechaResolucion,
                                            TiempoParadaIncidencia = CalcularTiempoParada(io.TiempoParada, io.FechaDeteccion, io.FechaResolucion)
                                        })
                                        .Where(i => i.IdActivo == idActivo);

            if (parsedFechaDesde.HasValue)
            {
                query = query.Where(i => i.FechaApertura >= parsedFechaDesde.Value);
            }

            if (parsedFechaHasta.HasValue)
            {
                query = query.Where(i => i.FechaApertura <= parsedFechaHasta.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<InformeExcelDTO>?> GetInformeExcelAsync(string? fechaDesde, string? fechaHasta, CancellationToken cancellationToken = default)
        {
            string consulta = $@"
                    WITH RECURSIVE ComponentesPadre AS (
                    SELECT 
                        id,
                        idComponentePadre,
                        CONCAT(denominacion, ' - ', descripcionES) AS ruta,
                        1 as nivel
                    FROM componentes
                    WHERE idComponentePadre IS NULL

                    UNION ALL

                    SELECT 
                        c.id,
                        c.idComponentePadre,
                        CONCAT(cp.ruta, ' > ', CONCAT(c.denominacion, ' - ', c.descripcionES)) AS ruta,
                        cp.nivel + 1 as nivel
                    FROM componentes c
                    INNER JOIN ComponentesPadre cp ON c.idComponentePadre=cp.id
                    )
                    SELECT  
                        c.siglas as 'Criticidad',  
                        a.activoSAP as 'ActivoSAP',  
                        a.descripcionES as 'Activo',  
                        a.id as 'IdActivo',  
                        o.idSAP as 'IdSAP', 
                        o.id as 'Id', 
                        o.comentarioOrden as 'ComentarioOrden',  
                        tp.name AS 'TipoOrden', 
                        o.fechaApertura as 'FechaApertura', 
                        o.fechaCierre as 'FechaCierre',  
                        et.name as 'EstadoOrden', 
                        comp.descripcionES as 'Componente', 
                        comp.denominacion as 'KKSComponente',
                        cp.ruta as 'JerarquiaComponente',
                        cp.nivel as 'NivelesComponente',
                        m.descripcionES as 'MecanismoDeFallo',  
                        i.descripcionES as 'Incidencia', 
                        r.descripcionES as 'Resolucion', 
                        o.comentarioResolucion as 'ComentarioResolucion', 
                        cc.DescripcionES as 'CentroCoste', 
                        cc.CentroCosteSAP as 'CentroCosteSAP',  
                        o.materiales as 'Materiales',  
                        io.fechaDeteccion as 'FechaDeteccion',  
                        io.fechaResolucion as 'FechaResolucion',
                        (SELECT GROUP_CONCAT(DISTINCT CONCAT(u.nombre, ' ', u.apellidos) SEPARATOR ', ') 
                         FROM usuarios_ordenes uo, users u 
                         WHERE u.id=uo.idUsuario AND uo.idOrden=o.id) as 'Usuarios',
                        io.cambioPieza,
                        io.afectaProduccion, 
                        io.paroMaquina,
                        IF(io.tiempoParada is null and io.fechaResolucion is not null,
                            ROUND(CAST(TIMESTAMPDIFF(MINUTE, io.fechaDeteccion, io.fechaResolucion) AS DOUBLE)/60, 2),
                            NULL) as 'TiempoParadaIncidencia',
                        (SELECT SUM(
                            IF(io_inner.tiempoParada IS NULL AND io_inner.fechaResolucion IS NOT NULL,
                                ROUND(CAST(TIMESTAMPDIFF(MINUTE, io_inner.fechaDeteccion, io_inner.fechaResolucion) AS DOUBLE)/60, 2),
                                NULL)
                            )
                         FROM incidenciasordenes io_inner 
                         WHERE io_inner.idOrden = o.id
                        ) AS 'TiempoParadaOrden'
                    FROM 
                        ordenes o
                        JOIN activos a ON o.idActivo = a.id
                        JOIN centrosdecostes cc ON a.idCentroCoste = cc.id
                        JOIN incidenciasordenes io ON o.id = io.idOrden
                        JOIN componentes comp ON io.idComponente = comp.id
                        JOIN incidencias i ON io.idIncidencia = i.id
                        JOIN mecanismosdefallo m ON i.IdMecanismoFallo = m.id
                        JOIN tiposorden tp ON o.idTipoOrden = tp.id
                        JOIN estadosorden et ON o.idEstadoOrden = et.id
                        JOIN ComponentesPadre cp ON comp.id = cp.id
                        LEFT JOIN resoluciones r ON io.idResolucion = r.id 
                        JOIN usuarios_ordenes uo ON o.id = uo.idOrden
                        JOIN users u ON uo.idUsuario = u.id 
                        JOIN criticidades c ON a.IdCriticidad = c.Id
                    WHERE
                        o.confirmada = true
                        {(fechaDesde != null ? $"AND o.fechaApertura >= '{fechaDesde:yyyy-MM-dd} 00:00:00'" : "")}
                        {(fechaHasta != null ? $"AND o.fechaApertura <= '{fechaHasta:yyyy-MM-dd} 23:59:59'" : "")}
                    GROUP BY
                        o.id,
                        c.siglas,
                        a.activoSAP,
                        a.descripcionES,
                        a.id,
                        o.idSAP,
                        o.id,
                        o.comentarioOrden,
                        tp.name,
                        o.fechaApertura,
                        o.fechaCierre,
                        et.name,
                        comp.descripcionES,
                        comp.denominacion,
                        cp.ruta,
                        cp.nivel,
                        m.descripcionES,
                        i.descripcionES,
                        r.descripcionES,
                        o.comentarioResolucion,
                        cc.descripcionES,
                        cc.CentroCosteSAP,
                        o.materiales,
                        io.fechaDeteccion,
                        io.fechaResolucion,
                        io.cambioPieza,
                        io.afectaProduccion, 
                        io.paroMaquina,
                        io.tiempoParada
                    ORDER BY 
                        o.fechaApertura DESC;";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = consulta;
                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<Dictionary<string, object>>();

                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }

                        results.Add(row);
                    }

                    return results.Select(d => new InformeExcelDTO
                    {
                        Id = d.ContainsKey("Id") ? Convert.ToInt32(d["Id"]) : 0,
                        IdSAP = d.ContainsKey("IdSAP") ? d["IdSAP"]?.ToString() : null,
                        TipoOrden = d["TipoOrden"]?.ToString() ?? "",
                        EstadoOrden = d["EstadoOrden"]?.ToString() ?? "",
                        IdActivo = d.ContainsKey("IdActivo") ? Convert.ToInt32(d["IdActivo"]) : 0,
                        ActivoSAP = d["ActivoSAP"]?.ToString() ?? "",
                        Activo = d["Activo"]?.ToString() ?? "",
                        Criticidad = d["Criticidad"]?.ToString() ?? "",
                        CentroCosteSAP = d["CentroCosteSAP"]?.ToString() ?? "",
                        CentroCoste = d["CentroCoste"]?.ToString() ?? "",
                        FechaApertura = Convert.ToDateTime(d["FechaApertura"]),
                        FechaCierre = d["FechaCierre"] != DBNull.Value ? Convert.ToDateTime(d["FechaCierre"]) : null,
                        TiempoParadaOrden = d["TiempoParadaOrden"] != DBNull.Value ? Convert.ToDouble(d["TiempoParadaOrden"]) : null,
                        Usuarios = d["Usuarios"]?.ToString() ?? "",
                        KKSComponente = d["KKSComponente"]?.ToString() ?? "",
                        Componente = d["Componente"]?.ToString() ?? "",
                        MecanismoDeFallo = d["MecanismoDeFallo"]?.ToString() ?? "",
                        Incidencia = d["Incidencia"]?.ToString() ?? "",
                        Resolucion = d["Resolucion"] != DBNull.Value ? d["Resolucion"]?.ToString() : null,
                        FechaDeteccion = Convert.ToDateTime(d["FechaDeteccion"]),
                        FechaResolucion = d["FechaResolucion"] != DBNull.Value ? Convert.ToDateTime(d["FechaResolucion"]) : null,
                        TiempoParadaIncidencia = d["TiempoParadaIncidencia"] != DBNull.Value ? (double?)Convert.ToDouble(d["TiempoParadaIncidencia"]) : null,
                        JerarquiaComponente = d["JerarquiaComponente"]?.ToString() ?? "",
                        NivelesComponente = d.ContainsKey("NivelesComponente") ? Convert.ToInt32(d["NivelesComponente"]) : 0,
                        ComentarioOrden = d["ComentarioOrden"] != DBNull.Value ? d["ComentarioOrden"]?.ToString() : null,
                        ComentarioResolucion = d["ComentarioResolucion"] != DBNull.Value ? d["ComentarioResolucion"]?.ToString() : null,
                        Materiales = d["Materiales"] != DBNull.Value ? d["Materiales"]?.ToString() : null,
                        CambioPieza = d.ContainsKey("CambioPieza") ? Convert.ToBoolean(d["CambioPieza"]) : false,
                        AfectaProduccion = d.ContainsKey("AfectaProduccion") ? Convert.ToBoolean(d["AfectaProduccion"]) : false,
                        ParoMaquina = d.ContainsKey("ParoMaquina") ? Convert.ToBoolean(d["ParoMaquina"]) : false
                    }).ToList();
                }
            }
        }

        public async Task<List<InformeExcelDTO>?> GetInformeExcelAsync(int idActivo, string? fechaDesde, string? fechaHasta, CancellationToken cancellationToken = default)
        {
            string consulta = $@"
                    WITH RECURSIVE ComponentesPadre AS (
                    SELECT 
                        id,
                        idComponentePadre,
                        CONCAT(denominacion, ' - ', descripcionES) AS ruta,
                        1 as nivel
                    FROM componentes
                    WHERE idComponentePadre IS NULL

                    UNION ALL

                    SELECT 
                        c.id,
                        c.idComponentePadre,
                        CONCAT(cp.ruta, ' > ', CONCAT(c.denominacion, ' - ', c.descripcionES)) AS ruta,
                        cp.nivel + 1 as nivel
                    FROM componentes c
                    INNER JOIN ComponentesPadre cp ON c.idComponentePadre=cp.id
                    )
                    SELECT  
                        c.siglas as 'Criticidad',  
                        a.activoSAP as 'ActivoSAP',  
                        a.descripcionES as 'Activo',  
                        a.id as 'IdActivo',  
                        o.idSAP as 'IdSAP', 
                        o.id as 'Id', 
                        o.comentarioOrden as 'ComentarioOrden',  
                        tp.name AS 'TipoOrden', 
                        o.fechaApertura as 'FechaApertura', 
                        o.fechaCierre as 'FechaCierre',  
                        et.name as 'EstadoOrden', 
                        comp.descripcionES as 'Componente', 
                        comp.denominacion as 'KKSComponente',
                        cp.ruta as 'JerarquiaComponente',
                        cp.nivel as 'NivelesComponente',
                        m.descripcionES as 'MecanismoDeFallo',  
                        i.descripcionES as 'Incidencia', 
                        r.descripcionES as 'Resolucion', 
                        o.comentarioResolucion as 'ComentarioResolucion', 
                        cc.DescripcionES as 'CentroCoste', 
                        cc.CentroCosteSAP as 'CentroCosteSAP',  
                        o.materiales as 'Materiales',  
                        io.fechaDeteccion as 'FechaDeteccion',  
                        io.fechaResolucion as 'FechaResolucion',
                        (SELECT GROUP_CONCAT(DISTINCT CONCAT(u.nombre, ' ', u.apellidos) SEPARATOR ', ') 
                         FROM usuarios_ordenes uo, users u 
                         WHERE u.id=uo.idUsuario AND uo.idOrden=o.id) as 'Usuarios',
                        io.cambioPieza,
                        io.afectaProduccion, 
                        io.paroMaquina,
                        IF(io.tiempoParada is null and io.fechaResolucion is not null,
                            ROUND(CAST(TIMESTAMPDIFF(MINUTE, io.fechaDeteccion, io.fechaResolucion) AS DOUBLE)/60, 2),
                            NULL) as 'TiempoParadaIncidencia',
                        (SELECT SUM(
                            IF(io_inner.tiempoParada IS NULL AND io_inner.fechaResolucion IS NOT NULL,
                                ROUND(CAST(TIMESTAMPDIFF(MINUTE, io_inner.fechaDeteccion, io_inner.fechaResolucion) AS DOUBLE)/60, 2),
                                NULL)
                            )
                         FROM incidenciasordenes io_inner 
                         WHERE io_inner.idOrden = o.id
                        ) AS 'TiempoParadaOrden'
                    FROM 
                        ordenes o
                        JOIN activos a ON o.idActivo = a.id
                        JOIN centrosdecostes cc ON a.idCentroCoste = cc.id
                        JOIN incidenciasordenes io ON o.id = io.idOrden
                        JOIN componentes comp ON io.idComponente = comp.id
                        JOIN incidencias i ON io.idIncidencia = i.id
                        JOIN mecanismosdefallo m ON i.IdMecanismoFallo = m.id
                        JOIN tiposorden tp ON o.idTipoOrden = tp.id
                        JOIN estadosorden et ON o.idEstadoOrden = et.id
                        JOIN ComponentesPadre cp ON comp.id = cp.id
                        LEFT JOIN resoluciones r ON io.idResolucion = r.id 
                        JOIN usuarios_ordenes uo ON o.id = uo.idOrden
                        JOIN users u ON uo.idUsuario = u.id 
                        JOIN criticidades c ON a.IdCriticidad = c.Id
                    WHERE
                        o.confirmada = true
                        AND o.idActivo = {idActivo}
                        {(fechaDesde != null ? $"AND o.fechaApertura >= '{fechaDesde:yyyy-MM-dd} 00:00:00'" : "")}
                        {(fechaHasta != null ? $"AND o.fechaApertura <= '{fechaHasta:yyyy-MM-dd} 23:59:59'" : "")}
                    GROUP BY
                        o.id,
                        c.siglas,
                        a.activoSAP,
                        a.descripcionES,
                        a.id,
                        o.idSAP,
                        o.id,
                        o.comentarioOrden,
                        tp.name,
                        o.fechaApertura,
                        o.fechaCierre,
                        et.name,
                        comp.descripcionES,
                        comp.denominacion,
                        cp.ruta,
                        cp.nivel,
                        m.descripcionES,
                        i.descripcionES,
                        r.descripcionES,
                        o.comentarioResolucion,
                        cc.descripcionES,
                        cc.CentroCosteSAP,
                        o.materiales,
                        io.fechaDeteccion,
                        io.fechaResolucion,
                        io.cambioPieza,
                        io.afectaProduccion, 
                        io.paroMaquina,
                        io.tiempoParada
                    ORDER BY 
                        o.fechaApertura DESC;";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = consulta;
                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<Dictionary<string, object>>();

                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);

                        }

                        results.Add(row);
                    }

                    return results.Select(d => new InformeExcelDTO
                    {
                        Id = d.ContainsKey("Id") ? Convert.ToInt32(d["Id"]) : 0,
                        IdSAP = d.ContainsKey("IdSAP") ? d["IdSAP"]?.ToString() : null,
                        TipoOrden = d["TipoOrden"]?.ToString() ?? "",
                        EstadoOrden = d["EstadoOrden"]?.ToString() ?? "",
                        IdActivo = d.ContainsKey("IdActivo") ? Convert.ToInt32(d["IdActivo"]) : 0,
                        ActivoSAP = d["ActivoSAP"]?.ToString() ?? "",
                        Activo = d["Activo"]?.ToString() ?? "",
                        Criticidad = d["Criticidad"]?.ToString() ?? "",
                        CentroCosteSAP = d["CentroCosteSAP"]?.ToString() ?? "",
                        CentroCoste = d["CentroCoste"]?.ToString() ?? "",
                        FechaApertura = Convert.ToDateTime(d["FechaApertura"]),
                        FechaCierre = d["FechaCierre"] != DBNull.Value ? Convert.ToDateTime(d["FechaCierre"]) : null,
                        TiempoParadaOrden = d["TiempoParadaOrden"] != DBNull.Value ? Convert.ToDouble(d["TiempoParadaOrden"]) : null,
                        Usuarios = d["Usuarios"]?.ToString() ?? "",
                        KKSComponente = d["KKSComponente"]?.ToString() ?? "",
                        Componente = d["Componente"]?.ToString() ?? "",
                        MecanismoDeFallo = d["MecanismoDeFallo"]?.ToString() ?? "",
                        Incidencia = d["Incidencia"]?.ToString() ?? "",
                        Resolucion = d["Resolucion"] != DBNull.Value ? d["Resolucion"]?.ToString() : null,
                        FechaDeteccion = Convert.ToDateTime(d["FechaDeteccion"]),
                        FechaResolucion = d["FechaResolucion"] != DBNull.Value ? Convert.ToDateTime(d["FechaResolucion"]) : null,
                        TiempoParadaIncidencia = d["TiempoParadaIncidencia"] != DBNull.Value ? (double?)Convert.ToDouble(d["TiempoParadaIncidencia"]) : null,
                        JerarquiaComponente = d["JerarquiaComponente"]?.ToString() ?? "",
                        NivelesComponente = d.ContainsKey("NivelesComponente") ? Convert.ToInt32(d["NivelesComponente"]) : 0,
                        ComentarioOrden = d["ComentarioOrden"] != DBNull.Value ? d["ComentarioOrden"]?.ToString() : null,
                        ComentarioResolucion = d["ComentarioResolucion"] != DBNull.Value ? d["ComentarioResolucion"]?.ToString() : null,
                        Materiales = d["Materiales"] != DBNull.Value ? d["Materiales"]?.ToString() : null,
                        CambioPieza = d.ContainsKey("CambioPieza") ? Convert.ToBoolean(d["CambioPieza"]) : false,
                        AfectaProduccion = d.ContainsKey("AfectaProduccion") ? Convert.ToBoolean(d["AfectaProduccion"]) : false,
                        ParoMaquina = d.ContainsKey("ParoMaquina") ? Convert.ToBoolean(d["ParoMaquina"]) : false
                    }).ToList();
                }
            }
        }

        private static double? CalcularTiempoParada(double? TiempoParadaBBDD, DateTime fechaDesde, DateTime? fechaHasta)
        {
            double? tiempo = null;
            if (TiempoParadaBBDD == null && fechaHasta != null) {
                tiempo = Math.Round((fechaHasta.Value - fechaDesde).TotalMinutes / 60, 2);
            }
            return tiempo;
        }
    }
}
