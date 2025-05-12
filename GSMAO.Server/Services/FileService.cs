using GSMAO.Server.DTOs;
using System.IO;
using System.Text;

namespace GSMAO.Server.Services
{
    /// <summary>
    /// Interfaz que define los servicios de gestión de archivos.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Genera la ruta completa de un archivo basado en el controlador, el ID y la ruta relativa.
        /// </summary>
        /// <param name="controller">El nombre del controlador.</param>
        /// <param name="id">El ID asociado al archivo.</param>
        /// <param name="path">La ruta relativa del archivo.</param>
        /// <returns>La ruta completa donde se encuentra o se almacenará el archivo.</returns>
        string GenerateFilePath(string controller, int id, string path);

        /// <summary>
        /// Crea una carpeta en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta donde se creará la carpeta.</param>
        /// <returns>Un Task que indica la finalización del proceso de creación de la carpeta.</returns>
        Task CreateFolderAsync(string path);

        /// <summary>
        /// Renombra una carpeta de manera asíncrona.
        /// </summary>
        /// <param name="pathActual">La ruta actual de la carpeta.</param>
        /// <param name="pathNew">La nueva ruta de la carpeta.</param>
        /// <returns>Un Task que indica la finalización del proceso de renombrado.</returns>
        Task RenameFolderAsync(string pathActual, string pathNew);

        /// <summary>
        /// Elimina una carpeta de manera asíncrona en la ruta especificada.
        /// </summary>
        /// <param name="path">La ruta de la carpeta que se eliminará.</param>
        /// <returns>Un Task que indica la finalización del proceso de eliminación.</returns>
        Task DeleteFolderAsync(string path);

        /// <summary>
        /// Guarda un archivo en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="file">El archivo a guardar.</param>
        /// <param name="path">La ruta donde se guardará el archivo.</param>
        /// <returns>Un Task que indica la finalización del proceso de guardado.</returns>
        Task SaveFileAsync(IFormFile file, string path);

        /// <summary>
        /// Obtiene los bytes de un archivo de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta del archivo del cual se obtendrán los bytes.</param>
        /// <returns>Un Task que devuelve los bytes del archivo.</returns>
        Task<byte[]> GetFileBytesAsync(string path);

        /// <summary>
        /// Elimina un archivo en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta del archivo que se eliminará.</param>
        /// <returns>Un Task que indica la finalización del proceso de eliminación.</returns>
        Task DeleteFileAsync(string path);

        bool ContieneCaracteresNoValidos(string text);

        string RemoveDiacritics(string text);

        string SanitizeFileName(string text);
    }

    /// <summary>
    /// Implementación de los servicios de gestión de archivos.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly string _baseFolderPath;
        private readonly ILogger<IFileService> _logger;

        public FileService(IConfiguration configuration, ILogger<IFileService> logger)
        {
            _baseFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "documentacion");
            _logger = logger;
        }

        /// <summary>
        /// Genera la ruta completa de un archivo basado en el controlador, el ID y la ruta relativa.
        /// </summary>
        /// <param name="controller">El nombre del controlador.</param>
        /// <param name="id">El ID asociado al archivo.</param>
        /// <param name="path">La ruta relativa del archivo.</param>
        /// <returns>La ruta completa del archivo.</returns>
        public string GenerateFilePath(string controller, int id, string path)
        {
            return Path.Combine(_baseFolderPath, controller, id.ToString(), path);
        }

        /// <summary>
        /// Crea una carpeta en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta donde se creará la carpeta.</param>
        /// <returns>Un Task que indica la finalización del proceso de creación de la carpeta.</returns>
        public Task CreateFolderAsync(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"No se tienen los permisos suficientes para crear la carpeta en la ruta {path}: {ex.Message}");

                throw new UnauthorizedAccessException($"No se tienen los permisos suficientes para crear la carpeta en la ruta {path}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ruta o nombre de la carpeta incorrectos (Ruta: {path}): {ex.Message}");

                throw new InvalidOperationException($"Ruta o nombre de la carpeta incorrectos (Ruta: {path})");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Renombra una carpeta de manera asíncrona.
        /// </summary>
        /// <param name="pathActual">La ruta actual de la carpeta.</param>
        /// <param name="pathNew">La nueva ruta de la carpeta.</param>
        /// <returns>Un Task que indica la finalización del proceso de renombrado.</returns>
        public Task RenameFolderAsync(string pathActual, string pathNew)
        {
            try
            {
                Directory.Move(pathActual, pathNew);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"No se tienen los permisos suficientes para renombrar la carpeta en la ruta {pathActual}: {ex.Message}");

                throw new UnauthorizedAccessException($"No se tienen los permisos suficientes para renombrar la carpeta en la ruta {pathActual}.");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError($"La carpeta que desea renombrar no se encuentra (Ruta: {pathActual}): {ex.Message}");

                throw new UnauthorizedAccessException($"La carpeta que desea renombrar no se encuentra (Ruta: {pathActual}).");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ruta o nombre de la carpeta incorrectos (Ruta actual: {pathActual}) (Ruta nueva: {pathNew}): {ex.Message}");

                throw new InvalidOperationException($"Ruta o nombre de la carpeta incorrectos (Ruta actual: {pathActual}) (Ruta nueva: {pathNew})");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina una carpeta de manera asíncrona en la ruta especificada.
        /// </summary>
        /// <param name="path">La ruta de la carpeta que se eliminará.</param>
        /// <returns>Un Task que indica la finalización del proceso de eliminación.</returns>
        public Task DeleteFolderAsync(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"No se tienen los permisos suficientes para eliminar la carpeta en la ruta {path}: {ex.Message}");

                throw new UnauthorizedAccessException($"No se tienen los permisos suficientes para eliminar la carpeta en la ruta {path}.");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError($"La carpeta que desea eliminar no se encuentra (Ruta: {path}): {ex.Message}");

                throw new DirectoryNotFoundException($"La carpeta que desea eliminar no se encuentra (Ruta: {path}).");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la carpeta (Ruta: {path}): {ex.Message}");

                throw new InvalidOperationException($"Error al eliminar la carpeta (Ruta: {path})");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Guarda un archivo en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="file">El archivo a guardar.</param>
        /// <param name="path">La ruta donde se guardará el archivo.</param>
        /// <returns>Un Task que indica la finalización del proceso de guardado del archivo.</returns>
        public async Task SaveFileAsync(IFormFile file, string path)
        {
            try
            {
                // Comprobamos si el fichero ya está subido
                if (File.Exists(path))
                    _logger.LogInformation($"El fichero {path} se va a sobreescribir.");

                var folderPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath!);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                //_logger.LogError($"No se tienen los permisos suficientes para subir el fichero en {path}: {ex.Message}");

                throw new UnauthorizedAccessException($"No se tienen los permisos suficientes para subir el fichero en {path}.");
            }
            catch (DirectoryNotFoundException ex)
            {
                //_logger.LogError($"La carpeta donde desea subir el fichero no se encuentra (Ruta: {path}): {ex.Message}");

                throw new DirectoryNotFoundException($"La carpeta donde desea subir el fichero no se encuentra (Ruta: {path}).");
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error al subir el fichero (Ruta: {path}): {ex.Message}");

                throw new InvalidOperationException($"Error al subir el fichero (Ruta: {path})");
            }
        }

        /// <summary>
        /// Obtiene los bytes de un archivo de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta del archivo.</param>
        /// <returns>Un Task que devuelve los bytes del archivo.</returns>
        public async Task<byte[]> GetFileBytesAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"El archivo en la ruta {path} no fue encontrado.");

            // Leer y devolver el contenido del archivo
            return await File.ReadAllBytesAsync(path);
        }

        /// <summary>
        /// Elimina un archivo en la ruta especificada de manera asíncrona.
        /// </summary>
        /// <param name="path">La ruta del archivo que se eliminará.</param>
        /// <returns>Un Task que indica la finalización del proceso de eliminación del archivo.</returns>
        public Task DeleteFileAsync(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"No se tienen los permisos suficientes para eliminar el fichero en la ruta {path}: {ex.Message}");

                throw new UnauthorizedAccessException($"No se tienen los permisos suficientes para eliminar el fichero en la ruta {path}.");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError($"El fichero que desea eliminar no se encuentra (Ruta: {path}): {ex.Message}");

                throw new DirectoryNotFoundException($"El fichero que desea eliminar no se encuentra (Ruta: {path}).");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el fichero (Ruta: {path}): {ex.Message}");

                throw new InvalidOperationException($"Error al eliminar el fichero (Ruta: {path})");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Comprueba si contiene caracteres no válidos o palabras reservadas de Windows
        /// </summary>
        /// <param name="text">El nombre de la carpeta o del fichero.</param>
        /// <returns>Un Booleano si el nombre es correcto o no.</returns>
        public bool ContieneCaracteresNoValidos(string text)
        {
            // Caracteres no válidos para nombres de archivos y carpetas en Windows
            char[] invalidChars = new char[]
            {
                '\\', '/', ':', '*', '?', '"', '<', '>', '|'
            };

            // También se podrían incluir algunos nombres reservados en Windows (opcional)
            string[] reservedNames = new string[] {
                "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5",
                "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            // Verifica si el nombre contiene algún carácter inválido
            bool contieneCaracterInvalido = text.Any(c => invalidChars.Contains(c));

            // Verifica si el nombre coincide con alguno de los nombres reservados (sin extensión)
            bool esNombreReservado = reservedNames.Contains(text.ToUpper());

            return contieneCaracterInvalido || esNombreReservado;
        }

        /// <summary>
        /// Elimina las tildes y los caracteres especiales en nombres de carpetas o ficheros.
        /// </summary>
        /// <param name="text">El nombre de la carpeta o del fichero.</param>
        /// <returns>Un String con los caracteres especiales y las tildes eliminadas.</returns>
        public string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Elimina los caracteres inválidos en nombres de carpetas y ficheros para Windows.
        /// </summary>
        /// <param name="text">El nombre de la carpeta o del fichero.</param>
        /// <returns>Un string con los caracteres inválidos eliminados.</returns>
        public string SanitizeFileName(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var split = text.Split("\\");
            string name = split[split.Length - 1].Split("/")[0];

            foreach (char c in invalidChars)
            {
                name = name.Replace(c.ToString(), "_"); // Sustituimos caracteres inválidos por "_"
            }

            // Sustituir los cambios en la última parte del texto
            text = text.Substring(0, text.LastIndexOf(name)) + name;
            
            return text;
        }
    }
}
