// ComponentSelectorForm.js
import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";

import {SelectInput} from "./SelectInput";

import {componentesService} from "@/services/ComponenteService";

export const DynamicSmartSelectInputs = ({isLoading, onChange, idActivo, componentesBase, ...rest}) => {
    const {t} = useTranslation();
    const [selections, setSelections] = useState([]);
    const [selectedComponent, setSelectedComponent] = useState(null);

    const fetchComponentes = async (idComponente) => {
        try {
            const response = await componentesService.getComponentes(idActivo, idComponente);
            if (!response.is_error) {
                const components = (response.content || []).sort((a, b) => a.id - b.id);
                return components;
            } else {
                console.log(response.error_content);
                return [];
            }
        } catch (error) {
            console.error("Error fetching componentes:", error);
            return [];
        }
    };

    const handleComponentChange = async (level, event) => {
        console.log("start of handlecomponentchange");
        const selectedId = event.target.value;

        // Actualizar la selección en el nivel actual
        const newSelections = selections.slice(0, level);

        const noHaCambiado = newSelections[level - 1]?.id == selectedId;
        if (noHaCambiado) {
            return;
        }

        newSelections[level - 1] = {
            ...newSelections[level - 1],
            id: selectedId,
            isLoading: true,
        };
        //Almacenar el valor del id del último nivel seleccionado, es decir, el último con id no vacío
        let lastSelectedId = newSelections.findLast((s) => s.id !== "")?.id;

        // Asumiendo que `options` es un array dentro del último objeto encontrado:
        let selectedOption = newSelections
            .findLast((s) => s.id !== "")
            ?.options?.find((option) => option.id === parseInt(lastSelectedId, 10));

        let children;

        console.log("about to fetch");
        if (selectedId) {
            children = await fetchComponentes(selectedId);

            if (children != undefined && children.length > 0) {
                // Limpiar los niveles inferiores y agregar un nuevo nivel vacío con sus opciones
                newSelections.push({id: "", options: children});
            }
        } else {
            children = await fetchComponentes(selectedOption?.id || 0);
        }

        // Establecer las nuevas selecciones, reseteando valores en los niveles inferiores
        setSelections(newSelections);
        // Guardar el componente seleccionado completo en el estado `selectedComponent`
        setSelectedComponent(selectedOption?.id || null);
        !!onChange && onChange(selectedOption, children);
    };

    useEffect(() => {
        // Cargar los componentes de nivel superior al montar el componente
        setSelections([{id: "", options: componentesBase}]);
        console.log("componentesBase: ", componentesBase);
    }, [componentesBase]);

    useEffect(() => {
        const handleActivoChange = async () => {
            const componentesBase = await fetchComponentes(idActivo.id || 0);

            console.log(":::::::::::::: componentesBase", componentesBase);
            setSelections([{id: "", options: componentesBase}]);
        };

        handleActivoChange();
    }, [idActivo]);

    return (
        <>
            {selections.map((selection, index) => (
                <SelectInput
                    {...rest}
                    key={`${index}---${selection.id}`}
                    label={t(`Componente Nivel ${index + 1}`)}
                    name={`componentLevel${index + 1}`}
                    value={selection.id} // Esto asegura que el valor seleccionado se resetee
                    options={(Array.isArray(selection.options) ? selection.options : []).map((r) => ({
                        value: r.id,
                        label: `${t(r.denominacion)} - ${r.descripcionES}`, // Ajuste de formato según tu requerimiento
                    }))}
                    onChange={(e) => handleComponentChange(index + 1, e)}
                    disabled={isLoading}
                />
            ))}
        </>
    );
};
