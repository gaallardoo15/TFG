import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class PanelGeneralService extends BaseCrudService {
    getPanelGeneral(filters) {
        return RestUtilities.get(`${this.apiUrl}?${filters.toString()}`);
    }

    reasignarOrden(formData) {
        return RestUtilities.put(`${this.apiUrl}/${formData.idOrden}/reasignar-usuario`, formData);
    }
    getHistorialModificacionesUsuariosOrden(idOrden) {
        return RestUtilities.get(`${this.apiUrl}/${idOrden}/historial-reasignaciones`);
    }
}

export const panelGeneralService = new PanelGeneralService("/api/panelGeneral", "id");
