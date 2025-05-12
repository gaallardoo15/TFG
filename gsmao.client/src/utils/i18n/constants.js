const t = (s) => s;

t("The file field is required.");

// Errores Inicio Sesión
t(
    "Verifica el email para poder iniciar sesión. \nSi tienes dudas de cómo hacerlo consulta al Administrador del sistema.",
);

// Estados de usuarios
t("Borrado");
t("Inactivo");
t("Activo");

// Roles de usuarios
t("Proveedor");
t("Superadministrador");
t("Administrador");
t("Álvaro Moreno");

// Estados de pedidos y packings
t("Creado");
t("Tramitando");
t("Enviando");
t("Enviado");
t("Recepcionado");

// Tipos de packing
t("retail");
t("online");

// Tallas
t("Niños");
t("Camisas");
t("Pantalones");
t("Única");

// Mensajes BadRequest() LeerPacking y LeerPedido
t("Nombre de usuario o contraseña incorrectos.");
t("El archivo Excel debe contener 1 hoja.");
t("No se puede cargar el pedido. El fichero no está estructurado correctamente, descargue la plantilla.");
t("No se ha indicado el número de Pedido.");
t("No se ha indicado el Taller.");
t("El Proveedor indicado no coincide con el indicado en el fichero.");
t("No se encuentra el proveedor registrado.");
t("No se encuentra en el fichero la columna de EAN, PRECIO o CANTIDAD");
t("Se han detectado valores vacíos en el fichero.");
t("Se debe indicar el número de Packing.");
//t("El código EAN " + worksheet.Cells[notacionCelda].Value.ToString() + ", no está registrado.");

// APIs externas
// Emails
t("Ocurrió un problema al enviar el correo electrónico de verificación de cuenta.");
// Productos
t("Problema de conexión con el API externa de productos. Inténtelo más tarde o consulte al administrador.");

// Password
t("Passwords must be at least 8 characters.");
t("Passwords must have at least one lowercase ('a'-'z').");

// Errores de packings
t(
    "Error al guardar la información básica de un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al guardar la infomación detallada de un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al guardar los tipos de tallas que contiene un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al actualizar el estado de un pedido cuando se crea un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al actualizar la información básica de un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al actualizar el estado de un pedido cuando se actualiza un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al eliminar un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al actualizar el estado de un pedido cuando se elimina un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t("Error al enviar el packing al endpoint de recepción de mercancía de Álvaro Moreno.");
t(
    "No se ha podido recepcionar el packing. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t(
    "Error al actualizar el estado de un pedido cuando se recepciona un packing debido a un problema de base de datos. Intenta nuevamente. Si continúa, contacta con el Administrador del sistema.",
);
t("El archivo Excel debe contener 1 hoja.");
t("Fichero de plantilla no encontrado.");
t(
    "Las cabeceras del fichero no coinciden con la plantilla descargada. Descague nuevamente la plantilla y no añada, modifique ni elimine columnas.",
);
t(
    "Las cabeceras del fichero no coinciden con la plantilla descargada. Descague nuevamente la plantilla y no añada, modifique ni elimine columnas.",
);
t("Falta la Referencia de la caja.");
t("Falta el Color de la caja.");
t("El número de bultos indicado no coincide con los bultos leídos en el fichero.");
t("El producto no está registrado en el maestro de artículos.");

// Tipos de packings
t("retail");
t("Retail");
t("online");
t("Online");

// Notificaciones
t("Pedido cargado");
t("Pedido actualizado");
t("Packing cargado");
t("Packing actualizado");
t("Contenedor asignado");
