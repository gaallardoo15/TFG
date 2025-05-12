import {useEffect, useMemo, useState} from "react";
import {useTranslation} from "react-i18next";

import {AddFileModal} from "./AddFileModal";
import {AddFolderModal} from "./AddFolderModal";
import {DeleteModal} from "./DeleteModal";
import {FileExplorerEmpty} from "./FileExplorerEmpty";

import "@/styles/foldersExplorer.css";
import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {PointsDropdown} from "@/components/common/Dropdown/PointsDropdown";
import {Icono} from "@/components/common/Icono";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {useFileDownloader} from "@/hooks/useFileDownloader";
import {useFileViewer} from "@/hooks/useFileViewer";
import {useModalState} from "@/hooks/useModalState";
import {folderService} from "@/services/FolderService";

// Función para eliminar caracteres específicos
function strip(str, charsToStrip) {
    const regex = new RegExp(`^[${charsToStrip}]+|[${charsToStrip}]+$`, "g");
    return str.replace(regex, "");
}

// Componente principal
export const FoldersExplorer = ({
    id,
    gestor,
    actualizarAdjuntosOrden,
    isVisible = true,
    materiales = false,
    activeTab = true,
    ...rest
}) => {
    const {t} = useTranslation();

    const [expandedFolders, setExpandedFolders] = useState({});
    const [searchTerm, setSearchTerm] = useState("");
    const [files, setFiles] = useState([]);
    const {modalState, openModal, closeModal} = useModalState();
    const [errors, setErrors] = useState({});
    const [modalErrors, setModalErrors] = useState({});
    const {isLoadingDownloader, fileDownload} = useFileDownloader();
    const [isLoading, setIsLoading] = useState(false);
    const {viewFile} = useFileViewer();

    useEffect(() => {
        if (
            id
            // && activeTab
        ) {
            fetchFiles(id);
        }
        setErrors({}); //Creo que es este
    }, [id, activeTab]);

    useEffect(() => {
        actualizarAdjuntosOrden && actualizarAdjuntosOrden(files);
    }, [files]);

    const fetchFiles = (id) => {
        const params = new URLSearchParams({
            materiales: materiales, // Serializa arrays como JSON
        });

        folderService.documentacion(id, gestor, params).then((response) => {
            if (!response.is_error) {
                let orderedFiles = orderFiles(response.content);
                setFiles(orderedFiles);
            } else {
                console.log(response.error_content);
                if (response.statusCode == 404) {
                    setErrors({});
                    setFiles([]);
                } else {
                    setErrors(response.error_content);
                }
            }
        });
    };
    // Alternar expansión de carpetas
    const toggleFolder = (folderKey) => {
        setExpandedFolders((prev) => ({
            ...prev,
            [folderKey]: !prev[folderKey],
        }));
    };

    //==================================================== HANDLES =====================================================

    const handleAction = (actionType, file = {}) => {
        if (Object.keys(file).length === 0) {
            file = "";
        }
        setModalErrors({});
        openModal(actionType, file);
    };

    // Manejar eliminación de archivos o carpetas
    const handleDeleteFolder = (formData, setIsLoaingDeleteFolder) => {
        formData.materiales = materiales;
        folderService.deleteFolder(id, gestor, formData).then((response) => {
            if (!response.is_error) {
                // setFiles((prev) =>
                //     prev.filter((file) => file.key !== formData.ruta && !file.key.startsWith(formData.ruta)),
                // );
                fetchFiles(id);
                closeModal();
            } else {
                console.log(response.error_content);
                setModalErrors(response.error_content);
            }
            setIsLoaingDeleteFolder(false);
        });
    };

    const handleDeleteFile = (formData, setIsLoadingDelete) => {
        formData.materiales = materiales;
        folderService.deleteFile(id, gestor, formData).then((response) => {
            if (!response.is_error) {
                // setFiles((prev) =>
                //     prev.filter((file) => file.key !== formData.ruta && !file.key.startsWith(formData.ruta)),
                // );
                fetchFiles(id);
                closeModal();
            } else {
                console.log(response.error_content);
                setModalErrors(response.error_content);
            }
            setIsLoadingDelete(false);
        });
    };

    // Manejar renombrado de archivos o carpetas
    const handleRename = (oldFolder, newFormData, setIsLoadingRename) => {
        const oldKey = oldFolder.key || "";

        const parentPath = oldKey.endsWith("/")
            ? oldKey.slice(0, -1).split("/").slice(0, -1).join("/")
            : oldKey.split("/").slice(0, -1).join("/");
        const newKey =
            (parentPath ? parentPath + "/" : "") + newFormData.folderName + (oldKey.endsWith("/") ? "/" : "");
        //(parentPath ? parentPath + "/" : "") + newFormData.folderName;

        newFormData.ruta = oldKey;
        newFormData.rutaNueva = newKey;
        newFormData.materiales = materiales;

        if (files.some((existingFile) => existingFile.key === newKey)) {
            setModalErrors(t("El nombre de carpeta ya existe. "));
            setIsLoading(false);
            return;
        }

        folderService.renameFolder(id, gestor, newFormData).then((response) => {
            if (!response.is_error) {
                // setFiles((prev) =>
                //     prev.map((file) => {
                //         if (file.key === oldKey || file.key.startsWith(oldKey)) {
                //             const updatedKey = file.key.replace(oldKey, newKey);
                //             return {
                //                 ...file,
                //                 key: updatedKey,
                //                 modified: obtenerFechaActual(),
                //             };
                //         }
                //         return file;
                //     }),
                // );
                fetchFiles(id);

                closeModal();
            } else {
                console.log(response.error_content);
                setModalErrors(response.error_content || {Error: "Ha ocurrido un error"});
            }
            setIsLoadingRename(false);
        });
    };

    // Manejar creación de nuevas carpetas
    const handleCreateFolder = (parentKey = "", formData, setIsLoadingCreateFolder) => {
        setIsLoadingCreateFolder(true);
        const rutaCarpeta = parentKey + formData.folderName.trimEnd() + "/";
        formData.ruta = rutaCarpeta;
        formData.materiales = materiales;

        folderService.crearCarpeta(id, gestor, formData).then((response) => {
            if (!response.is_error) {
                setFiles((prev) => {
                    const newFiles = [
                        ...prev,
                        {key: formData.ruta, type: "folder", extension: "", modified: obtenerFechaActual(), size: null},
                    ];
                    return orderFiles(newFiles); // Llama a `orderFiles` después de añadir el nuevo archivo
                });
                // Expandir la carpeta padre NO FUNCIONA
                setExpandedFolders((prev) => ({
                    ...prev,
                    [parentKey]: true,
                }));
                closeModal();
            } else {
                console.log(response.error_content);
                setModalErrors(response.error_content || {Error: "Ha ocurrido un error"});
            }
            setIsLoadingCreateFolder(false);
        });
    };

    const handleCreateFile = (parentKey = "", formData, setIsLoadingCreateFile) => {
        setIsLoadingCreateFile(true);
        // formData.Ruta = parentKey;
        //parentKey = materiales && "Materiales/" + parentKey;
        formData.append("Ruta", parentKey);
        formData.append("materiales", materiales);

        const fileName = formData.get("file").name;
        const newFileKey = parentKey + fileName;
        const extension = obtenerExtension(fileName);
        const fileSize = formData.get("file").size;

        folderService.uploadFile(id, gestor, formData).then((response) => {
            if (!response.is_error) {
                setFiles((prev) => {
                    const newFiles = [
                        ...prev,
                        {
                            key: newFileKey,
                            type: "file",
                            extension: extension,
                            modified: obtenerFechaActual(),
                            size: formatFileSize(fileSize),
                        },
                    ];
                    return orderFiles(newFiles); // Llama a `orderFiles` después de añadir el nuevo archivo
                });
                // Expandir la carpeta padre
                setExpandedFolders((prev) => ({
                    ...prev,
                    [parentKey]: true,
                }));

                closeModal();
            } else {
                console.log("Error fetch: ", response.error_content);
                setModalErrors(response.error_content || {Error: "Ha ocurrido un error"});
            }
            setIsLoadingCreateFile(false);
        });
    };

    // Manejar descarga de archivos (implementación simulada)
    const handleDownload = async (file) => {
        const params = new URLSearchParams({
            materiales: materiales, // Serializa arrays como JSON
        });
        const url = `/api/${gestor}/${id}/descargar-archivo/${encodeURIComponent(file.key)}?${params.toString()}`;
        const errorContent = await fileDownload(url, file.key);
        setErrors(errorContent);
    };

    const handleOpenFile = async (file) => {
        const params = new URLSearchParams({
            materiales: materiales, // Serializa arrays como JSON
        });
        const url = `/api/${gestor}/${id}/ver-archivo/${encodeURIComponent(file.key)}?${params.toString()}`;
        const errorContent = await viewFile(url, file.key);
        setErrors(errorContent);
    };
    //==================================================== FIN HANDLES =====================================================

    // Filtrar archivos según el término de búsqueda
    const filteredFiles = useMemo(() => {
        return files.filter((file) => file.key.toLowerCase().includes(searchTerm.toLowerCase()));
    }, [files, searchTerm]);

    // Construir estructura de árbol de archivos
    const fileTree = useMemo(() => buildFileTree(filteredFiles), [filteredFiles]);

    // Expandir carpetas por defecto al buscar
    useEffect(() => {
        if (searchTerm.trim() !== "") {
            const expanded = {};
            filteredFiles.forEach((file) => {
                const parts = strip(file.key, "/").split("/");
                let pathSoFar = "";
                parts.forEach((part, index) => {
                    const isFolder = index < parts.length - 1 || file.type === "folder";
                    if (isFolder) {
                        pathSoFar += part + "/";
                        expanded[pathSoFar] = true;
                    }
                });
            });
            setExpandedFolders(expanded);
        }
    }, [searchTerm, filteredFiles]);

    function obtenerFechaActual() {
        const ahora = new Date();

        const dia = String(ahora.getDate()).padStart(2, "0");
        const mes = String(ahora.getMonth() + 1).padStart(2, "0"); // getMonth() devuelve de 0 a 11
        const año = ahora.getFullYear();

        const horas = String(ahora.getHours()).padStart(2, "0");
        const minutos = String(ahora.getMinutes()).padStart(2, "0");
        const segundos = String(ahora.getSeconds()).padStart(2, "0");

        return `${dia}/${mes}/${año} ${horas}:${minutos}:${segundos}`;
    }

    function orderFiles(auxFiles) {
        let orderedFiles = auxFiles.sort((a, b) => {
            // Priorizar carpetas sobre archivos
            if (a.type === "folder" && b.type !== "folder") {
                return -1;
            }
            if (a.type !== "folder" && b.type === "folder") {
                return 1;
            }

            // Si ambos son del mismo tipo, ordenar alfabéticamente por `key`
            return a.key.localeCompare(b.key);
        });

        return orderedFiles;
    }

    function formatFileSize(bytes) {
        const sizes = ["B", "KB", "MB", "GB", "TB"];
        let len = bytes;
        let order = 0;

        while (len >= 1024 && order < sizes.length - 1) {
            order++;
            len /= 1024;
        }

        return `${len.toFixed(2)} ${sizes[order]}`;
    }

    function obtenerExtension(filename) {
        const arrayFilename = filename.split(".");
        const extension = "." + arrayFilename[arrayFilename.length - 1];
        return extension;
    }
    return (
        <div className="d-flex flex-column justify-content-between" id="folderExplorer">
            <div className="d-flex flex-column gap-3" {...rest}>
                <Toolbar
                    searchTerm={searchTerm}
                    setSearchTerm={setSearchTerm}
                    handleCreateFolder={handleCreateFolder}
                    handleAction={handleAction}
                    isVisible={isVisible}
                />
                {fileTree?.length == 0 ? (
                    <FileExplorerEmpty />
                ) : (
                    <div className="scrollable-list">
                        <table className="tableFolderExplorer">
                            <thead className="text-center">
                                <tr>
                                    <th className="text-start">{t("Nombre")}</th>
                                    <th>{t("Tamaño")}</th>
                                    <th>{t("Última Modificación")}</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <FileList
                                    files={fileTree}
                                    expandedFolders={expandedFolders}
                                    toggleFolder={toggleFolder}
                                    handleDownload={handleDownload}
                                    handleOpenFile={handleOpenFile}
                                    handleAction={handleAction}
                                    isVisible={isVisible}
                                />
                            </tbody>
                        </table>
                    </div>
                )}

                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "addFile":
                                return (
                                    <AddFileModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                        handleAction={handleCreateFile}
                                        existingFiles={files}
                                        errors={modalErrors}
                                    />
                                );
                            case "folderForm":
                                return (
                                    <AddFolderModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                        handleAction={handleCreateFolder}
                                        isRename={false}
                                        errors={modalErrors}
                                        existingFiles={files}
                                    />
                                );
                            case "renameFolder":
                                return (
                                    <AddFolderModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                        handleAction={handleRename}
                                        isRename={true}
                                        errors={modalErrors}
                                    />
                                );
                            case "deleteFolder":
                                return (
                                    <DeleteModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                        handleAction={handleDeleteFolder}
                                        isFolder={true}
                                        modelName={t("Carpeta")}
                                        errors={modalErrors}
                                    />
                                );
                            case "deleteFile":
                                return (
                                    <DeleteModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                        handleAction={handleDeleteFile}
                                        isFolder={false}
                                        modelName={t("Archivo")}
                                        errors={modalErrors}
                                    />
                                );

                            default:
                                return null;
                        }
                    })()}
            </div>
            <MostrarErrores errors={errors} />
        </div>
    );
};

//======================= COMPONENTES EXTERNOS =====================

//--------------------- COMPONENTE TOOLBAR ------------------------
const Toolbar = ({searchTerm, setSearchTerm, handleAction, isVisible}) => {
    const {t} = useTranslation();
    return (
        <div className="toolBarCustomFolderExplorer">
            <div id="botonesToolbarFolderExplorer" className="d-flex gap-2">
                <BtnCrearElemento
                    onClick={() => handleAction("folderForm")}
                    customText={t("Nueva Carpeta")}
                    customIcon="folder-plus"
                    disabled={!isVisible}
                />
                <BtnCrearElemento
                    onClick={() => handleAction("addFile")}
                    customText={t("Importar Archivo")}
                    customIcon="file-arrow-up"
                    disabled={!isVisible}
                />
            </div>

            <input
                type="text"
                placeholder="Filtrar archivos"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="buscadorFolderExplorer"
            />
        </div>
    );
};

//-------------- COMPONENTE PARA PERSONALIZAR EL ICONO DEL ARCHIVO SEGÚN LA EXTENSIÓN ----------------

const IconoTipoFile = ({extension}) => {
    if (extension.includes("doc")) {
        return <Icono name="fa-solid fa-file-word" style={{color: "#2B579A"}} />;
    } else if (extension.includes("csv") || extension.includes("xls")) {
        return <Icono name="fa-solid fa-file-csv" style={{color: "#216b3ccc"}} />;
    } else if (extension.includes("pdf")) {
        return <Icono name="fa-solid fa-file-pdf" style={{color: "#b30000"}} />;
    } else if (
        extension.includes("jpg") ||
        extension.includes("jpeg") ||
        extension.includes("png") ||
        extension.includes("ico")
    ) {
        return <Icono name="fa-solid fa-file-image" style={{color: "#5f7d95"}} />;
    } else {
        return <Icono name="fa-solid fa-file" style={{color: "#2B579A"}} />;
    }
};

//----------------------- CONSTRUIR ÁRBOL DE ARCHIVOS ---------------------------
const buildFileTree = (fileList) => {
    const root = [];
    const nodes = {};
    fileList.forEach((file) => {
        const parts = strip(file.key, "/").split("/");
        let currentLevel = root;

        let pathSoFar = "";
        parts.forEach((part, index) => {
            const isLastPart = index === parts.length - 1;
            const isFolder = !isLastPart || file.type === "folder";
            const key = pathSoFar + part + (isFolder ? "/" : "");
            pathSoFar = key;

            if (!nodes[key]) {
                const node = {
                    key: key,
                    name: part,
                    type: isFolder ? "folder" : "file",
                    extension: file.extension,
                    modified: isLastPart ? file.modified : null, // Evitar nuevos objetos Moment
                    size: isLastPart ? file.size : null,
                    children: [],
                };
                nodes[key] = node;
                currentLevel.push(node);
            }
            currentLevel = nodes[key].children;
        });
    });
    return root;
};

//------------ COMPONENTE QUE DIBUJA EL ÁRBOL DE ARCHIVOS Y CARPETAS ----------
const FileList = ({
    files,
    expandedFolders,
    toggleFolder,
    handleDownload,
    handleOpenFile,
    handleAction,
    level = 0,
    isVisible,
}) => {
    if (level >= 6) {
        return null;
    } // Limitar la profundidad máxima a 6 niveles

    return (
        <>
            {files.map((file) => (
                <FileItem
                    key={file.key}
                    file={file}
                    expandedFolders={expandedFolders}
                    toggleFolder={toggleFolder}
                    handleDownload={handleDownload}
                    handleAction={handleAction}
                    handleOpenFile={handleOpenFile}
                    level={level}
                    isVisible={isVisible}
                />
            ))}
        </>
    );
};

// ------------ COMPONENTE DE CADA ELEMENTO DEL ÁRBOL DE ARCHIVOS ------------
const FileItem = ({
    file,
    expandedFolders,
    toggleFolder,
    handleDownload,
    handleOpenFile,
    handleAction,
    level,
    isVisible,
}) => {
    const isFolder = file.type === "folder";
    const folderExpanded = expandedFolders[file.key];
    return (
        <>
            <tr>
                <td style={{paddingLeft: level * 20}}>
                    {isFolder ? (
                        <span className="folder" onClick={() => toggleFolder(file.key)}>
                            {folderExpanded ? (
                                <Icono
                                    size="lg"
                                    style={{color: "#e3b549", marginRight: "3px"}}
                                    name="fa-solid fa-folder-open"
                                />
                            ) : (
                                <Icono
                                    size="lg"
                                    style={{color: "#e3b549", marginRight: "3px"}}
                                    name="fa-solid fa-folder"
                                />
                            )}{" "}
                            {file.name}
                        </span>
                    ) : (
                        <span>
                            <IconoTipoFile extension={file.extension} /> {file.name}
                        </span>
                    )}
                </td>
                <td className="text-center">{file.size ? file.size : "--"}</td>
                <td className="text-center">{file.modified ? file.modified : "--"}</td>
                <td className="text-end">
                    <FileActions
                        file={file}
                        isFolder={isFolder}
                        handleDownload={handleDownload}
                        handleAction={handleAction}
                        handleOpenFile={handleOpenFile}
                        isVisible={isVisible}
                    />
                </td>
            </tr>
            {isFolder && folderExpanded && file.children.length > 0 && level < 6 && (
                <FileList
                    files={file.children}
                    expandedFolders={expandedFolders}
                    toggleFolder={toggleFolder}
                    handleDownload={handleDownload}
                    handleAction={handleAction}
                    handleOpenFile={handleOpenFile}
                    level={level + 1}
                    isVisible={isVisible}
                />
            )}
        </>
    );
};

// ------------ COMPONENTE QUE CONFIGURA LAS ACCIONES DE CADA ELEMENTO DEL ÁRBOL --------------
const FileActions = ({file, isFolder, handleDownload, handleAction, handleOpenFile, isVisible}) => {
    const {t} = useTranslation();
    const noActionAvaible = [
        {label: t("Ninguna acción disponible"), onClick: () => {}, clases: "text-muted", disabled: true},
    ];
    const actions = [
        ...(isFolder
            ? [
                  {
                      label: t("Crear Carpeta"),
                      onClick: () => handleAction("folderForm", file.key),
                      clases: "text-success",
                      disabled: !isVisible,
                  },
                  {
                      label: t("Importar Archivo"),
                      onClick: () => handleAction("addFile", file.key),
                      clases: "text-success",
                      disabled: !isVisible,
                  },
                  {
                      label: t("Renombrar"),
                      onClick: () => handleAction("renameFolder", file),
                      clases: "",
                      disabled: !isVisible,
                  },
                  {
                      label: t("Eliminar Carpeta"),
                      onClick: () => handleAction("deleteFolder", file.key),
                      clases: "text-danger",
                      disabled: !isVisible,
                  },
              ]
            : [
                  {
                      label: t("Abrir"),
                      onClick: () => handleOpenFile(file),
                      clases: "",
                      disabled: false,
                  },
                  {
                      label: t("Descargar"),
                      onClick: () => handleDownload(file),
                      clases: "",
                      disabled: false,
                  },
                  {
                      label: t("Eliminar Archivo"),
                      onClick: () => handleAction("deleteFile", file.key),
                      clases: "text-danger",
                      disabled: !isVisible,
                  },
              ]),
    ];
    const actionsToShow = actions.filter((action) => !action.disabled);

    return <PointsDropdown actions={actionsToShow.length > 0 ? actionsToShow : noActionAvaible} />;
};
