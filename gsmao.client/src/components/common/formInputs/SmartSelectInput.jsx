import {useEffect, useState} from "react";
import PropTypes from "prop-types";

import {SelectInput} from "./SelectInput";

export const SmartSelectInput = ({
    fetcher,
    valueKey,
    labelKey,
    labelComponent,
    filter,
    onChange,
    selectFirst = false,
    value,
    ...rest
}) => {
    const [data, setData] = useState([]);
    const [filteredData, setFilteredData] = useState([]);
    const [selectedValue, setSelectedValue] = useState(null); // Estado para el valor seleccionado
    const [isLoading, setIsLoading] = useState(false);

    const fetchData = () => {
        setIsLoading(true);

        fetcher().then((response) => {
            if (!response.is_error) {
                const items = response.content || [];
                setData(items);
            } else {
                console.error(response.error_content);
            }
            setIsLoading(false);
        });
    };

    useEffect(() => {
        fetchData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    useEffect(() => {
        const newFilteredData = filter ? data.filter(filter) : data;
        setFilteredData(newFilteredData);

        // Auto-seleccionar valor si coincide con alguno en el select
        const initialValue = value || (selectFirst && newFilteredData.length > 0 && newFilteredData[0][valueKey]);
        if (initialValue) {
            const initialItem = newFilteredData.find((item) => item[valueKey] === initialValue);
            if (initialItem) {
                setSelectedValue(initialItem[valueKey]);
                onChange({
                    target: {
                        name: initialItem[labelKey],
                        value: initialItem[valueKey],
                        itemData: initialItem,
                    },
                });
            }
        }

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [data, filter]);

    // Detectar cambios en `value` externo y actualizar `selectedValue` si es necesario
    useEffect(() => {
        if (value !== undefined && data.length > 0) {
            const matchingItem = data.find((item) => item[valueKey] === value);
            if (matchingItem) {
                setSelectedValue(matchingItem[valueKey]);
                return;
            }
        }
    }, [value, data, valueKey]);

    const handleChange = (event) => {
        const {value: key} = event.target;
        setSelectedValue(key); // Actualiza el valor seleccionado
        onChange({
            target: {
                ...event.target,
                itemData: data.find((i) => i[valueKey] === key),
            },
        });
    };

    return (
        <SelectInput
            {...rest}
            onChange={handleChange}
            options={filteredData.map((o) => ({
                value: o[valueKey],
                label: labelComponent ? labelComponent(o) : o[labelKey],
            }))}
            value={selectedValue} // Establece el valor inicial
            isLoading={isLoading}
        />
    );
};

SmartSelectInput.propTypes = {
    fetcher: PropTypes.func,
    valueKey: PropTypes.string.isRequired,
    labelKey: PropTypes.string,
    labelComponent: PropTypes.func,
    filter: PropTypes.func,
    value: PropTypes.any,
};
