import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class ComponenteService extends BaseCrudService {
    //Obtiene los componentes hijos (no se si hijos o padres) de un activo en un nivel específico
    getComponentes(idActivo, idComponente) {
        // Construye la URL con los parámetros de consulta
        const url = `${this.apiUrl}?idActivo=${idActivo}&idComponente=${idComponente}`;
        return RestUtilities.get(url);
    }
    //Obtiene todos los componentes del activo sin importar el nivel
    getComponentesActivo(idActivo) {
        // Construye la URL con los parámetros de consulta
        const url = `${this.apiUrl}/activo/${idActivo}`;
        return RestUtilities.get(url);
    }
}

export const componentesService = new ComponenteService("/api/componentes", "id");
