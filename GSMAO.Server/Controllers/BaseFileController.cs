using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public abstract class BaseFileController : ControllerBase
    {
        protected readonly IFileService _fileService;
        protected readonly ILogger<BaseFileController> _logger;

        /// <summary>
        /// Nombre de carpeta raíz para este controlador (por ejemplo, "Activos" u "Ordenes").
        /// </summary>
        protected abstract string RootFolderName { get; }

        /// <summary>
        /// Método abstracto para validar en BD si la entidad (Activo u Orden) con el ID dado existe.
        /// </summary>
        protected abstract Task ValidateEntityExistsAsync(int id);

        public BaseFileController(IFileService fileService, ILogger<BaseFileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la documentación adjunta de la entidad (carpetas, ficheros, etc.).
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La documentación se ha obtenido correctamente y se devuelve el objeto <see cref="IEnumerable{DocumentacionDTO}"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la carpeta de la entidad actual o no contiene documentación.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar obtener la documentación.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet("{id}/documentacion")]
        public virtual async Task<ActionResult<IEnumerable<DocumentacionDTO>>> RetrieveDocumentacion(int id, [FromQuery] bool materiales)
        {
            try
            {
                await ValidateEntityExistsAsync(id);

                var folderPath = _fileService.GenerateFilePath(RootFolderName, id, "");
                if (Directory.Exists(folderPath))
                {
                    var documentacion = Directory.GetFileSystemEntries(folderPath, "*", SearchOption.AllDirectories);

                    List<DocumentacionDTO> documentacionDTO = new();
                    foreach (var item in documentacion)
                    {
                        var itemInfo = new FileInfo(item);

                        // Determinar si es carpeta o fichero
                        bool isDirectory = (itemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
                        string type = isDirectory ? "folder" : "file";

                        // El DTO que usas para listar documentación
                        var itemDTO = new DocumentacionDTO
                        {
                            // Key = sub-ruta relativa (reemplaza "\" por "/")
                            Key = itemInfo.FullName.Split($@"\{id}\")[1].Replace("\\", "/") + (isDirectory ? "/" : ""),
                            Type = type,
                            Extension = itemInfo.Extension,
                            Modified = itemInfo.LastWriteTime.ToString(),
                            Size = !isDirectory ? Utils.FormatFileSize(itemInfo.Length) : null
                        };
                        
                        if (!materiales && !itemDTO.Key.Contains("Materiales/"))
                        {
                            documentacionDTO.Add(itemDTO);
                        }
                        else if(materiales && itemDTO.Key.Contains("Materiales/"))
                        {
                            itemDTO.Key = itemDTO.Key.Replace("Materiales/", "");
                            if (itemDTO.Key != "") documentacionDTO.Add(itemDTO);
                        }
                    }

                    return Ok(documentacionDTO);
                }
                else
                {
                    _logger.LogInformation($"No existe carpeta para la entidad con ID={id} en {folderPath}.");
                    return NotFound("El explorador de archivos de esta entidad está vacío.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la documentación con ID={id}: {ex.Message}");
                return StatusCode(500, $"Error al obtener la documentación de la entidad {id}.");
            }
        }

        /// <summary>
        /// Crea una carpeta dentro de la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="createCarpetaDTO">Objeto <see cref="CreateCarpetaDTO"/> que contiene la información de la carpeta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La carpeta se ha creado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se tienen los permisos suficientes para crear la carpeta.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la ruta de la entidad actual.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la carpeta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost("{id}/crear-carpeta")]
        public virtual async Task<ActionResult> CreateCarpetaDocumentacion(int id, [FromBody] CreateCarpetaDTO createCarpetaDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = createCarpetaDTO.Materiales ? "Materiales" : "";

                if (createCarpetaDTO.Ruta.Equals("Materiales/")) throw new InvalidOperationException("No se puede crear ninguna carpeta con el nombre \"Materiales\"");

                createCarpetaDTO.Ruta = createCarpetaDTO.Ruta!.Replace("/", "\\");

                if (_fileService.ContieneCaracteresNoValidos(createCarpetaDTO.Ruta.TrimEnd('\\').Split("\\").Last())) {
                    throw new InvalidOperationException("Error. Palabras o Carácteres no permitidos (<, >, :,\", /, \\, |, ?, * ,CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5,COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4,LPT5, LPT6, LPT7, LPT8, LPT9 )");
                }

                // Concatenar el nombre del fichero a la ruta
                if (!createCarpetaDTO.Ruta.IsNullOrEmpty())
                {
                    createCarpetaDTO.Ruta = Path.Combine(rutaBase, createCarpetaDTO.Ruta!);
                }
                else
                {
                    createCarpetaDTO.Ruta = rutaBase;
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, createCarpetaDTO.Ruta);
                await _fileService.CreateFolderAsync(path);

                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear una carpeta para ID={id}: {ex.Message}");
                return StatusCode(500, $"Error al crear la carpeta para la entidad {id}.");
            }
        }

        /// <summary>
        /// Renombra una carpeta dentro de la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="renameCarpetaDTO">Objeto <see cref="RenameCarpetaDTO"/> que contiene la información de la carpeta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La carpeta se ha renombrado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se tienen los permisos suficientes para renombrar la carpeta.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la ruta de la entidad actual.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar renombrar la carpeta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}/rename-carpeta")]
        public virtual async Task<ActionResult> RenameCarpetaDocumentacion(int id, [FromBody] RenameCarpetaDTO renameCarpetaDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = renameCarpetaDTO.Materiales ? "Materiales" : "";

                renameCarpetaDTO.Ruta = renameCarpetaDTO.Ruta!.Replace("/", "\\");
                renameCarpetaDTO.RutaNueva = renameCarpetaDTO.RutaNueva!.Replace("/", "\\");

                if (_fileService.ContieneCaracteresNoValidos(renameCarpetaDTO.RutaNueva.TrimEnd('\\').Split("\\").Last()))
                {
                    throw new InvalidOperationException("Error. Palabras o Carácteres no permitidos (<, >, :,\", /, \\, |, ?, * ,CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5,COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4,LPT5, LPT6, LPT7, LPT8, LPT9 )");
                }

                // Concatenar el nombre del fichero a la ruta
                if (!renameCarpetaDTO.Ruta.IsNullOrEmpty())
                {
                    renameCarpetaDTO.Ruta = Path.Combine(rutaBase, renameCarpetaDTO.Ruta!);
                }
                else
                {
                    renameCarpetaDTO.Ruta = rutaBase;
                }

                // Concatenar el nombre del fichero a la ruta
                if (!renameCarpetaDTO.RutaNueva.IsNullOrEmpty())
                {
                    renameCarpetaDTO.RutaNueva = Path.Combine(rutaBase, renameCarpetaDTO.RutaNueva!);
                }
                else
                {
                    renameCarpetaDTO.RutaNueva = rutaBase;
                }

                var pathActual = _fileService.GenerateFilePath(RootFolderName, id, renameCarpetaDTO.Ruta);
                var pathNew = _fileService.GenerateFilePath(RootFolderName, id, renameCarpetaDTO.RutaNueva);

                await _fileService.RenameFolderAsync(pathActual, pathNew);
                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException || ex is DirectoryNotFoundException)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al renombrar la carpeta para ID={id}: {ex.Message}");
                return StatusCode(500, $"Error al renombrar la carpeta para la entidad {id}.");
            }
        }

        /// <summary>
        /// Elimina una carpeta y todo el contenido dentro de la misma de la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="deleteCarpetaDTO">Objeto <see cref="DeleteCarpetaDTO"/> que contiene la información de la carpeta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La carpeta se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se tienen los permisos suficientes para eliminar la carpeta.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la ruta de la entidad actual.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar la carpeta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}/eliminar-carpeta")]
        public virtual async Task<ActionResult> DeleteCarpetaDocumentacion(int id, [FromBody] DeleteCarpetaDTO deleteCarpetaDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = deleteCarpetaDTO.Materiales ? "Materiales" : "";

                // Concatenar el nombre del fichero a la ruta
                if (!deleteCarpetaDTO.Ruta.IsNullOrEmpty())
                {
                    deleteCarpetaDTO.Ruta = Path.Combine(rutaBase, deleteCarpetaDTO.Ruta!);
                }
                else
                {
                    deleteCarpetaDTO.Ruta = rutaBase;
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, deleteCarpetaDTO.Ruta);
                await _fileService.DeleteFolderAsync(path);

                // Comprobar si existe más documentación en la carpeta de la orden para eliminar la carpeta si esta está vacía.
                var folderPath = _fileService.GenerateFilePath(RootFolderName, id, rutaBase);
                var documentacion = Directory.GetFileSystemEntries(folderPath, "*", SearchOption.AllDirectories);

                if (documentacion.Length == 0) await _fileService.DeleteFolderAsync(_fileService.GenerateFilePath(RootFolderName, id, rutaBase));

                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException || ex is DirectoryNotFoundException)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar carpeta para ID={id} \n ERROR: {ex.Message}");
                return StatusCode(500, $"Error al eliminar la carpeta situada en {deleteCarpetaDTO.Ruta}.");
            }
        }

        /// <summary>
        /// Subir un fichero dentro de la estrucutra de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="file">Archivo que se desea subir.</param>
        /// <param name="createFicheroDTO">Objeto <see cref="CreateFicheroDTO"/> que contiene la información adicional del fichero.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El fichero se ha subido correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se tienen los permisos suficientes para subir el fichero.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la entidad asociada al identificador proporcionado o la ruta de destino.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar subir el fichero.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost("{id}/subir-archivo")]
        public virtual async Task<ActionResult> CreateFicheroDocumentacion(int id, IFormFile file, [FromForm] CreateFicheroDTO createFicheroDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = createFicheroDTO.Materiales ? "Materiales" : "";

                if (_fileService.ContieneCaracteresNoValidos(file.FileName))
                {
                    throw new InvalidOperationException("Error. Palabras o Carácteres no permitidos (<, >, :,\", /, \\, |, ?, * ,CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5,COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4,LPT5, LPT6, LPT7, LPT8, LPT9 )");
                }

                // Concatenar el nombre del fichero a la ruta
                if (!createFicheroDTO.Ruta.IsNullOrEmpty())
                {
                    createFicheroDTO.Ruta = createFicheroDTO.Ruta!.Replace("/", "\\");
                    createFicheroDTO.Ruta = Path.Combine(rutaBase, createFicheroDTO.Ruta!, file.FileName);
                }
                else
                {
                    createFicheroDTO.Ruta = Path.Combine(rutaBase, file.FileName);
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, createFicheroDTO.Ruta);
                await _fileService.SaveFileAsync(file, path);

                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException || ex is DirectoryNotFoundException)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al subir el fichero para ID={id}: {ex.Message}");
                return StatusCode(500, $"Error al subir el fichero en {createFicheroDTO.Ruta}.");
            }
        }

        /// <summary>
        /// Descarga un fichero desde la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="ruta">Ruta del fichero que se desea descargar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El fichero se ha descargado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra el fichero en la ruta proporcionada.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar descargar el fichero.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet("{id}/descargar-archivo/{ruta}")]
        public virtual async Task<ActionResult> DownloadFicheroDocumentacion(int id, string ruta, [FromQuery] bool materiales)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = materiales ? "Materiales" : "";

                // Concatenar el nombre del fichero a la ruta
                if (!ruta.IsNullOrEmpty())
                {
                    ruta = Path.Combine(rutaBase, ruta);
                }
                else
                {
                    ruta = rutaBase;
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, HttpUtility.UrlDecode(ruta)).Replace("/", "\\");
                var fileBytes = await _fileService.GetFileBytesAsync(path);
                var contentType = "application/octet-stream";

                return File(fileBytes, contentType, Path.GetFileName(path));
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al descargar el fichero para ID={id}, ruta={ruta}: {ex.Message}");
                return StatusCode(500, $"Error al descargar el fichero desde {ruta}.");
            }
        }

        /// <summary>
        /// Visualiza un fichero desde la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="ruta">Ruta del fichero que se desea visualizar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El fichero se ha visualizado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra el fichero en la ruta proporcionada.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar visualizar el fichero.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet("{id}/ver-archivo/{ruta}")]
        public virtual async Task<ActionResult> ViewFicheroDocumentacion(int id, string ruta, [FromQuery] bool materiales)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = materiales ? "Materiales" : "";

                // Concatenar el nombre del fichero a la ruta
                if (!ruta.IsNullOrEmpty())
                {
                    ruta = Path.Combine(rutaBase, ruta);
                }
                else
                {
                    ruta = rutaBase;
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, HttpUtility.UrlDecode(ruta)).Replace("/", "\\");
                var fileBytes = await _fileService.GetFileBytesAsync(path);

                // MIME type correcto según extensión
                var contentType = Utils.GetMimeType(Path.GetExtension(path));
                if (contentType == "application/octet-stream")
                    throw new InvalidOperationException("No está disponible la vista previa del documento.");

                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    Inline = true,
                    FileName = _fileService.SanitizeFileName(_fileService.RemoveDiacritics(Path.GetFileName(path)))
                };

                Response.Headers.Append("Content-Disposition", contentDisposition.ToString());
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al visualizar el fichero para ID={id}, ruta={ruta}: {ex.Message}");
                return StatusCode(500, $"Error al visualizar el fichero desde {ruta}.");
            }
        }

        /// <summary>
        /// Elimina un fichero dentro de la estructura de la documentación de la entidad.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <param name="deleteFicheroDTO">Objeto <see cref="DeleteFicheroDTO"/> que contiene la información del fichero a eliminar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El fichero se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se tienen los permisos suficientes para eliminar el fichero.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra el fichero en la ruta proporcionada.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar el fichero.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}/eliminar-archivo")]
        public virtual async Task<ActionResult> DeleteFicheroDocumentacion(int id, [FromBody] DeleteFicheroDTO deleteFicheroDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await ValidateEntityExistsAsync(id);

                var rutaBase = deleteFicheroDTO.Materiales ? "Materiales" : "";

                // Concatenar el nombre del fichero a la ruta
                if (!deleteFicheroDTO.Ruta.IsNullOrEmpty())
                {
                    deleteFicheroDTO.Ruta = Path.Combine(rutaBase, deleteFicheroDTO.Ruta);
                }
                else
                {
                    deleteFicheroDTO.Ruta = rutaBase;
                }

                var path = _fileService.GenerateFilePath(RootFolderName, id, deleteFicheroDTO.Ruta);
                await _fileService.DeleteFileAsync(path);

                // Comprobar si existe más documentación en la carpeta de la orden para eliminar la carpeta si esta está vacía.
                var folderPath = _fileService.GenerateFilePath(RootFolderName, id, rutaBase);
                var documentacion = Directory.GetFileSystemEntries(folderPath, "*", SearchOption.AllDirectories);

                if (documentacion.Length == 0) await _fileService.DeleteFolderAsync(_fileService.GenerateFilePath(RootFolderName, id, rutaBase));

                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException || ex is FileNotFoundException)
            {
                _logger.LogInformation($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"{ex.Message}");
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el fichero para ID={id}, ruta={deleteFicheroDTO.Ruta}: {ex.Message}");
                return StatusCode(500, $"Error al eliminar el fichero situado en {deleteFicheroDTO.Ruta}.");
            }
        }
    }
}
