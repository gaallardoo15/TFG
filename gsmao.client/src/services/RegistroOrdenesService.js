import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class RegistroOrdenesService extends BaseCrudService {
    //Crea una orden vacía y me devuelve el id de la orden que acaba de crear
    guardarOrdenVacia() {
        return RestUtilities.post(`${this.apiUrl}`);
    }
}

export const registroOrdenService = new RegistroOrdenesService("/api/registroOrdenes", "id");
