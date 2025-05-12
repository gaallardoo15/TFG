import { useCallback, useEffect, useRef, useState } from "react";
import { Form } from "react-bootstrap";
import { useTranslation } from "react-i18next";
import { useGridFilter } from "ag-grid-react";

export const DatatableSelectFilter = (props) => {
  const { model, onModelChange, getValue, api, customUniqueValues = [] } = props;
  const refSelect = useRef(null);
  const gridApiRef = useRef(api);
  const [uniqueValues, setUniqueValues] = useState([]);
  const { t } = useTranslation();

  // Función que recorre todos los nodos y extrae los valores únicos.
  const updateUniqueValues = useCallback(() => {
    let nuevosValores = [];
    if (customUniqueValues.length === 0) {
      const uniqueSet = new Set();
      if (gridApiRef.current && gridApiRef.current.forEachNode) {
        gridApiRef.current.forEachNode((node) => {
          const value = getValue(node);
          if (value) {
            uniqueSet.add(value);
          }
        });
      }
      nuevosValores = Array.from(uniqueSet);
    } else {
      nuevosValores = customUniqueValues;
    }

    // Solo actualizamos el estado si hay cambios
    setUniqueValues((prevValues) =>
      JSON.stringify(prevValues) === JSON.stringify(nuevosValores)
        ? prevValues
        : nuevosValores
    );
  }, [customUniqueValues, getValue]);

  useEffect(() => {
    if (!api) return;
    
    gridApiRef.current = api;
    updateUniqueValues();

    api.addEventListener("modelUpdated", updateUniqueValues);
    return () => {
      if (api && !api.isDestroyed()) {
        api.removeEventListener("modelUpdated", updateUniqueValues);
      }
    };
  }, [api, customUniqueValues, updateUniqueValues]);

  // Este efecto fuerza la actualización del filtro una vez que uniqueValues ya están listos
  useEffect(() => {
    if (model && uniqueValues.length > 0 && api && !api.isDestroyed()) {
      api.onFilterChanged();
    }
  }, [uniqueValues, model, api]);

  // Función que determina si una fila pasa el filtro.
  const doesFilterPass = useCallback(
    (params) => {
      const { node } = params;
      if (model === null || model === undefined) {
        return true;
      }
      if (customUniqueValues.length > 0) {
        // Separamos el string (que puede ser una concatenación) y comprobamos si incluye el modelo
        const valuesArray = getValue(node)
          .split(",")
          .map((item) => item.trim());
        return valuesArray.includes(model);
      }
      return getValue(node) === model;
    },
    [model, getValue, customUniqueValues]
  );

  // Para enfocar el select cuando se muestre el filtro.
  const afterGuiAttached = useCallback((params) => {
    if (!params || !params.suppressFocus) {
      refSelect.current.focus();
    }
  }, []);

  // Indica si el filtro está activo.
  const isFilterActive = useCallback(() => {
    return model !== null && model !== undefined;
  }, [model]);

  // Método que devuelve el estado actual del filtro.
  const getModel = useCallback(() => {
    return model ? { value: model } : null;
  }, [model]);

  // Método que permite restaurar el estado del filtro.
  const setModel = useCallback(
    (filterModel) => {
      if (filterModel && filterModel.value) {
        if (customUniqueValues.length > 0) {
          // Si usamos customUniqueValues, asignamos directamente el valor restaurado
          onModelChange(filterModel.value);
        } else {
          // Si no se han calculado aún o no hay coincidencia exacta, usamos el valor restaurado
          const formattedValue =
            uniqueValues.find((val) => val === filterModel.value) || filterModel.value;
          onModelChange(formattedValue);
        }
      } else {
        onModelChange(null);
      }
      // Forzamos la actualización del grid
      if (api && !api.isDestroyed()) {
        api.onFilterChanged();
      }
    },
    [onModelChange, uniqueValues, customUniqueValues, api]
  );

  // Registro de la API del filtro en ag‑Grid.
  useGridFilter({
    doesFilterPass,
    afterGuiAttached,
    isFilterActive,
    getModel,
    setModel,
  });

  return (
    <div className="mt-2 p-2">
      <Form.Select
        ref={refSelect}
        value={model ? model : ""}
        onChange={(e) => {
          onModelChange(e.target.value === "0" ? null : e.target.value);
          if (api && !api.isDestroyed()) {
            api.onFilterChanged();
          }
        }}
        placeholder={t("Seleccionar para filtrar...")}
      >
        <option value="0">{t("Todos")}</option>
        {uniqueValues.map((val, index) => (
          <option key={index} value={val}>
            {val}
          </option>
        ))}
      </Form.Select>
    </div>
  );
};
