import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class FolderService extends BaseCrudService {
    documentacion(id, gestor, params) {
        return RestUtilities.get(`/api/${gestor}/${id}/documentacion?${params.toString()}`);
    }

    crearCarpeta(id, gestor, formdata) {
        return RestUtilities.post(`/api/${gestor}/${id}/crear-carpeta`, formdata);
    }
    renameFolder(id, gestor, formdata) {
        return RestUtilities.put(`/api/${gestor}/${id}/rename-carpeta`, formdata);
    }
    deleteFolder(id, gestor, formdata) {
        return RestUtilities.put(`/api/${gestor}/${id}/eliminar-carpeta`, formdata);
    }
    deleteFile(id, gestor, formdata) {
        return RestUtilities.put(`/api/${gestor}/${id}/eliminar-archivo`, formdata);
    }
    uploadFile(id, gestor, formdata) {
        return RestUtilities.post(`/api/${gestor}/${id}/subir-archivo`, formdata);
    }
}

export const folderService = new FolderService();
