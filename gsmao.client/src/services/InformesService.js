import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class InformeService extends BaseCrudService {
    getInforme(filters) {
        return RestUtilities.get(`${this.apiUrl}?${filters.toString()}`);
    }
}

export const informesService = new InformeService("/api/informes", "id");
