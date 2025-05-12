import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class OrdenService extends BaseCrudService {
    getEstadosOrdenes() {
        return RestUtilities.get(`${this.apiUrl}/estados`);
    }
    getTiposOrdenes() {
        return RestUtilities.get(`${this.apiUrl}/tipos`);
    }
    getUsuariosOrden(id) {
        return RestUtilities.get(`${this.apiUrl}/${id}/usuarios`);
    }
    getUsuariosDisponiblesOrden(idOrden) {
        return RestUtilities.get(`${this.apiUrl}/${idOrden}/usuarios-disponibles`);
    }
    getIncidenciasOrden(idOrden) {
        return RestUtilities.get(`${this.apiUrl}/${idOrden}/incidencias`);
    }
    
    eliminarUsuarioOrden(idOrden, idUsuario) {
        return RestUtilities.delete(`${this.apiUrl}/${idOrden}/eliminar-usuario/${idUsuario}`);
    }
    asignarUsuarioOrden(idOrden, idUsuario) {
        return RestUtilities.post(`${this.apiUrl}/${idOrden}/asignar-usuario/${idUsuario}`);
    }
    getJerarquiaComponente(idComponente) {
        return RestUtilities.get(`${this.apiUrl}/obtener-jerarquia/${idComponente}`);
    }
    editarIncidenciaOrden(formdata) {
        return RestUtilities.put(`${this.apiUrl}/${formdata.idOrden}/editar-incidencia/${formdata.id}`, formdata);
    }
    editarResolucionOrden(formdata) {
        return RestUtilities.put(`${this.apiUrl}/${formdata.idOrden}/asignar-resolucion/${formdata.id}`, formdata);
    }
    eliminarIncidenciaOrden(formdata, idIncidencia) {
        console.log(formdata);
        return RestUtilities.delete(`${this.apiUrl}/${formdata.id}/eliminar-incidencia/${idIncidencia}`);
    }
    addNuevasIncidencias(formdata) {
        return RestUtilities.post(`${this.apiUrl}/${formdata.idOrden}/nueva-incidencia`, formdata);
    }
    getYears() {
        return RestUtilities.get(`${this.apiUrl}/years`);
    }
    getCriticidades() {
        return RestUtilities.get(`${this.apiUrl}/criticidades`);
    }

    constructor(...args) {
        super(...args);

        this.getEstadosOrdenes = this.getEstadosOrdenes.bind(this);
        this.getTiposOrdenes = this.getTiposOrdenes.bind(this);
        this.getUsuariosOrden = this.getUsuariosOrden.bind(this);
        this.getUsuariosDisponiblesOrden = this.getUsuariosDisponiblesOrden.bind(this);
        this.eliminarUsuarioOrden = this.eliminarUsuarioOrden.bind(this);
    }
}

export const ordenService = new OrdenService("/api/ordenes", "id");
