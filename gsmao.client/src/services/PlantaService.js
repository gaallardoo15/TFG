import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";
class PlantaService extends BaseCrudService {
    getPlantasEmpresa(idEmpresa) {
        return RestUtilities.get(`${this.apiUrl}/por-empresa/${idEmpresa}`);
    }

    constructor(...args) {
        super(...args);

        this.getPlantasEmpresa = this.getPlantasEmpresa.bind(this);
    }
}

export const plantasService = new PlantaService("/api/plantas", "id");
