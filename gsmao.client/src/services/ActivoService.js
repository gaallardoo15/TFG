import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class ActivoService extends BaseCrudService {
    changeState(id, state) {
        return RestUtilities.put(`${this.apiUrl}/${id}/change-state/${state}`);
    }
}

export const activosService = new ActivoService("/api/activos", "id");
