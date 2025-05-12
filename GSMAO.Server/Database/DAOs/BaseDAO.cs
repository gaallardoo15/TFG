using GSMAO.Server.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GSMAO.Server.Database.DAOs;

/// <summary>
/// Interfaz base que define las operaciones CRUD generales.
/// </summary>
/// <typeparam name="T">El tipo de entidad que maneja el DAO.</typeparam>
/// <typeparam name="TKey">El tipo de dato del identificador de la entidad.</typeparam>
public interface IBaseDAO<T, TKey> where T : class
{
    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    /// <param name="id">El identificador de la entidad.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>La entidad encontrada o una excepción si no se encuentra.</returns>
    Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una entidad por su nombre o descripción.
    /// </summary>
    /// <param name="name">El valor del nombre o la descripción.</param>
    /// <param name="attribute">Bandera que identifica si esa entidad el campo se llama "Name" o "Descripcion".</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>La entidad encontrada o una excepción si no se encuentra.</returns>
    Task<T> GetByNameAsync(string name, string attribute, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>Una colección de todas las entidades.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    /// <param name="entity">La entidad a agregar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una lista de nuevas entidades.
    /// </summary>
    /// <param name="entities">Las entidades a agregar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    Task BulkCreateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    /// <param name="entity">La entidad con los nuevos datos.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una entidad por su identificador.
    /// </summary>
    /// <param name="id">El identificador de la entidad a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Clase base abstracta que implementa las operaciones CRUD comunes.
/// Proporciona funcionalidad de logging configurable y manejo genérico de excepciones.
/// Las subclases pueden sobrescribir los métodos protegidos para personalizar la lógica.
/// </summary>
/// <typeparam name="T">El tipo de entidad que maneja el DAO.</typeparam>
/// <typeparam name="TKey">El tipo de dato del identificador de la entidad.</typeparam>
public abstract class BaseDAO<T, TKey> : IBaseDAO<T, TKey> where T : class
{
    /// <summary>
    /// El contexto de la base de datos utilizado por el DAO.
    /// </summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// Logger configurado para el DAO.
    /// </summary>
    protected readonly ILogger<BaseDAO<T, TKey>> _logger;

    /// <summary>
    /// Nivel de verbosidad del logging.
    /// </summary>
    private readonly bool _isVerboseLogging;

    /// <summary>
    /// Constructor de la clase BaseDAO.
    /// </summary>
    /// <param name="context">El contexto de base de datos.</param>
    /// <param name="logger">El logger para registrar eventos.</param>
    /// <param name="isVerboseLogging">Indica si los logs deben ser detallados o no.</param>
    protected BaseDAO(ApplicationDbContext context, ILogger<BaseDAO<T, TKey>> logger, bool isVerboseLogging = true)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _isVerboseLogging = isVerboseLogging;
    }

    /// <inheritdoc />
    public async Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                var entity = await GetByIdInternalAsync(id, ct).ConfigureAwait(false);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"No se encontró la entidad {typeof(T).Name} con ID {id}.");
                }

                return entity;
            },
            $"Obteniendo entidad {typeof(T).Name} con ID {id}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T> GetByNameAsync(string valor, string attribute, CancellationToken cancellationToken = default)
    {
        return await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                var entity = await GetByNameInternalAsync(valor, attribute, ct).ConfigureAwait(false);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"No se encontró la entidad {typeof(T).Name} con {attribute} {valor}");
                }

                return entity;
            },
            $"Obteniendo entidad {typeof(T).Name} con {attribute} {valor}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await HandleWithLoggingAndExceptionAsync(
            async ct => await GetAllInternalAsync(ct).ConfigureAwait(false),
            $"Obteniendo todas las entidades de {typeof(T).Name}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return await HandleWithLoggingAndExceptionAsync(
            async ct => await CreateInternalAsync(entity, ct).ConfigureAwait(false),
            $"Agregando nueva entidad {typeof(T).Name}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task BulkCreateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                await _context.Set<T>().AddRangeAsync(entities, ct);
                await _context.SaveChangesAsync(ct);
            },
            $"Agregando listado de entidad {typeof(T).Name}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await HandleWithLoggingAndExceptionAsync(
            async ct => await UpdateInternalAsync(entity, ct).ConfigureAwait(false),
            $"Actualizando entidad {typeof(T).Name}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        await HandleWithLoggingAndExceptionAsync(
            async ct => await DeleteInternalAsync(id, ct).ConfigureAwait(false),
            $"Eliminando entidad {typeof(T).Name} con ID {id}",
            cancellationToken
        ).ConfigureAwait(false);
    }

    // MÉTODOS SOBREESCRIBIBLES: Estos métodos pueden ser sobrescritos para personalizar la lógica del DAO.

    /// <summary>
    /// Método protegido que obtiene una entidad por su ID.
    /// Las subclases pueden sobrescribir este método para modificar la lógica de obtención.
    /// </summary>
    /// <param name="id">El identificador de la entidad.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    protected virtual async Task<T> GetByIdInternalAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Método protegido que obtiene una entidad que tenga el valor 'value' en su atributo 'attribute'
    /// Las subclases pueden sobreescribir este método para modificar la lógica de obtención.
    /// </summary>
    /// <param name="value">El valor por el que busco en la entidad.</param>
    /// <param name="attribute">El nombre del atributo que busco en la entidad.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    /// <exception cref="ArgumentNullException">Cuando no se encuentra el atributo 'attribute' en la entidad.</exception>
    protected virtual async Task<T> GetByNameInternalAsync(string value, string attribute, CancellationToken cancellationToken = default)
    {
        // Obtener el tipo de la entidad T
        var entityType = typeof(T);

        // Buscar la propiedad que coincide con el nombre del atributo (Name o Descripcion)
        var property = entityType.GetProperty(attribute, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (property == null) throw new ArgumentNullException($"La entidad {entityType.Name} no tiene una propiedad llamada {attribute}");

        // Crear la consulta usando LINQ dinámico
        var query = _context.Set<T>().AsQueryable();

        // Filtrar la entidad por el valor del atributo (Name o Descripcion)
        query = query.Where(e => EF.Property<string>(e, attribute) == value);

        // Retornar el primer resultado encontrado
        return await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Método protegido que obtiene todas las entidades.
    /// Las subclases pueden sobrescribir este método para modificar la lógica de obtención.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>Una lista de todas las entidades.</returns>
    protected virtual async Task<IEnumerable<T>> GetAllInternalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Método protegido que agrega una entidad.
    /// Las subclases pueden sobrescribir este método para modificar la lógica de agregado.
    /// </summary>
    /// <param name="entity">La entidad a agregar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    protected virtual async Task<T> CreateInternalAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Método protegido que actualiza una entidad.
    /// Las subclases pueden sobrescribir este método para modificar la lógica de actualización.
    /// </summary>
    /// <param name="entity">La entidad a actualizar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    protected virtual async Task UpdateInternalAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Método protegido que elimina una entidad por su ID.
    /// Las subclases pueden sobrescribir este método para modificar la lógica de eliminación.
    /// </summary>
    /// <param name="id">El identificador de la entidad a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    protected virtual async Task DeleteInternalAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdInternalAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new KeyNotFoundException(
                $"No se puede eliminar la entidad {typeof(T).Name} con ID {id} porque no existe.");
        }
    }

    // MÉTODOS DE APOYO: Estos no son sobreescribibles.

    /// <summary>
    /// Método que maneja la ejecución de lógica con logging y manejo de excepciones.
    /// </summary>
    /// <typeparam name="TResult">El tipo de resultado esperado.</typeparam>
    /// <param name="func">Función que ejecuta la lógica principal.</param>
    /// <param name="actionDescription">Descripción de la acción para el logging.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>El resultado de la función ejecutada.</returns>
    private async Task<TResult> HandleWithLoggingAndExceptionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> func,
        string actionDescription,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_isVerboseLogging)
            {
                _logger.LogInformation($"Iniciando: {actionDescription}");
            }

            TResult result = await func(cancellationToken).ConfigureAwait(false);

            if (_isVerboseLogging)
            {
                _logger.LogInformation($"Completado: {actionDescription}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error durante {actionDescription}");
            HandleException(ex);
            throw;
        }
    }

    /// <summary>
    /// Método que maneja la ejecución de lógica con logging y manejo de excepciones para acciones que no retornan resultado.
    /// </summary>
    /// <param name="func">Función que ejecuta la lógica principal.</param>
    /// <param name="actionDescription">Descripción de la acción para el logging.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    protected async Task HandleWithLoggingAndExceptionAsync(
        Func<CancellationToken, Task> func,
        string actionDescription,
        CancellationToken cancellationToken = default)
    {
        await HandleWithLoggingAndExceptionAsync<object>(
            async ct =>
            {
                await func(ct).ConfigureAwait(false);
                return null!;
            },
            actionDescription,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Método protegido que maneja las excepciones.
    /// </summary>
    /// <param name="ex">La excepción capturada.</param>
    protected virtual void HandleException(Exception ex)
    {
        // Ir a la excepción raíz
        ex = Utils.GetInnermostException(ex);

        if (ex is DbUpdateConcurrencyException concurrencyEx)
        {
            throw new InvalidOperationException("Error de concurrencia al actualizar la base de datos.", concurrencyEx);
        }
        else if (ex is MySqlException mysqlEx && mysqlEx.Number == 1062) // Código de error de entrada duplicada.
        {
            if (mysqlEx.Message.Contains("Duplicate entry") && mysqlEx.Message.Split("'").Length >= 3)
            {
                throw new InvalidOperationException(
                    $"El valor '{mysqlEx.Message.Split("'")[1]}' no puede repetirse en la base de datos.");
            }
            else
            {
                throw new InvalidOperationException("Error: Entrada duplicada detectada.", mysqlEx);
            }
        }
        else if (ex is DbUpdateException dbUpdateEx)
        {
            throw new InvalidOperationException("Error al actualizar la base de datos.", dbUpdateEx);
        }
        else if (ex is MySqlException mysql && mysql.Number == 1451) // Código de error de foreign key constraint.
        {
            if (mysql.Message.Contains("foreign key constraint fails"))
            {
                throw new InvalidOperationException($"Existe información relacionada al elemento que se intenta modificar o eliminar.");
            }
            else
            {
                throw new InvalidOperationException("Error: Foreign Key detectada con asociaciones.", mysql);
            }
        }
        else
        {
            throw ex;
        }
        // Agregar más manejo de excepciones específicas según sea necesario.
    }
}
