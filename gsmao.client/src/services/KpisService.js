import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class KpiService extends BaseCrudService {
    getIndicadoresOTs(filters) {
        return RestUtilities.get(`${this.apiUrl}/Ordenes?${filters.toString()}`);
    }
    getIndicadoresConfiabilidad(filters) {
        return RestUtilities.get(`${this.apiUrl}/Confiabilidad?${filters.toString()}`);
    }
    getEstudioComparativoIndicadoresOT(filters) {
        return RestUtilities.get(`${this.apiUrl}/estudio-comparativo-ordenes?${filters.toString()}`);
    }
    getEstudioComparativoConfiabilidad(filters) {
        return RestUtilities.get(`${this.apiUrl}/estudio-comparativo-confiabilidad?${filters.toString()}`);
    }

    constructor(...args) {
        super(...args);

        this.getEstudioComparativoConfiabilidad = this.getEstudioComparativoConfiabilidad.bind(this);
        this.getEstudioComparativoIndicadoresOT = this.getEstudioComparativoIndicadoresOT.bind(this);
    }
}

export const kpisService = new KpiService("/api/KPIs", "id");
