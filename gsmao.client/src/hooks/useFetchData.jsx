/**
 * Hook personalizado para realizar una solicitud de datos y manejar el estado de carga.
 * @param {Object} service - El servicio utilizado para realizar la solicitud de datos.
 * @returns {Object} - Un objeto que contiene los elementos de la lista, el estado de carga
 *  y la función para realizar la solicitud de datos.
 */
// import { activosService } from "@/services/ActivoService";
// import { centrosCostesService } from "@/services/CentroCosteService";
// import { componentesService } from "@/services/ComponenteService";
// import { incidenciasService } from "@/services/IncidenciaService";
// import { localizacionesService } from "@/services/LocalizacionService";
// import { mecanismosFallosService } from "@/services/MecanismoFalloService";
// import { resolucionesService } from "@/services/ResolucionService";
// import { userService } from "@/services/UserService";
// import { makeActivos, makeCentrosCostes, makeComponentes, makeIncidencias, makeLocalizaciones, makeMecanismosFallo, makeResoluciones, makeUsers } from "@/utils/data/makeData";
import {useEffect, useState} from "react";

export const useFetchListData = (fetch) => {
    const [items, setItems] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [isRefetching, setIsRefetching] = useState(false);

    const fetchData = () => {
        setIsLoading(true);

        fetch().then((response) => {
            if (!response.is_error) {
                setItems(response.content || []);
            } else {
                console.error(response.error_content);
            }
            setIsLoading(false);
            setIsRefetching(false);
        });
    };

    useEffect(() => {
        fetchData();

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return {items, isLoading, isRefetching, fetchData};
};

/**
 * Hook personalizado para realizar una solicitud de datos y manejar el estado de carga.
 * @param {Object} service - El servicio utilizado para realizar la solicitud de datos.
 * @returns {Object} - Un objeto que contiene el elemento, el estado de carga
 * y la función para realizar la solicitud de datos.
 */
export const useFetchItemData = (service, id) => {
    const [item, setItem] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [isRefetching, setIsRefetching] = useState(false);

    const fetchItemData = () => {
        setIsLoading(true);
        service.fetch(id).then((response) => {
            if (!response.is_error) {
                setItem(response.content || {});
            } else {
                console.error(response.error_content);
            }
            setIsLoading(false);
            setIsRefetching(false);
        });
    };

    useEffect(() => {
        fetchItemData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return {item, isLoading, isRefetching, fetchItemData};
};
