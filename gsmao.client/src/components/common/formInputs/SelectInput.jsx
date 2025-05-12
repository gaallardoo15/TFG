import {Form} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import Select, {components, createFilter} from "react-select";
import PropTypes from "prop-types";

import {RequiredSpan} from "./RequiredSpan";

export const SelectInput = ({
    value,
    onChange,
    label,
    name,
    options,
    required,
    emptyLabel,
    isLoading,
    disabled,
    searchable,
    dataIsHuge,
    md,
    multiple = false,
    extraClases = "",
    ...rest
}) => {
    const {t} = useTranslation();

    const handleInputChange = (props) => {
        const {value: newValue} = props || {value: ""};
        // El componente de react select devuelve directamente {valor, label}, en lugar del evento.
        // Pasar los atributos necesarios, incluido el nuevo valor, al manejador onChange del componente padre
        onChange({
            target: {
                name,
                value: newValue,
                description: props?.label,
            },
        });
    };

    const selectedOption = options.find((o) => o.value == value);

    return (
        <div className={`mb-3 col-md-${md || 12} ${extraClases}`} {...rest}>
            <Form.Label className="fw-semibold">
                {label}
                {!!required && <RequiredSpan />}
                {/* {isLoading && (
                    <>
                        {" "}
                        <Spinner variant="secondary" size="sm" />
                    </>
                )} */}
            </Form.Label>
            <Select
                components={dataIsHuge && {Option}}
                filterOption={createFilter({ignoreAccents: false})} // this makes all the difference!
                required={!!required}
                className="basic-single"
                classNamePrefix="select"
                // defaultValue={selectedOption}
                isDisabled={disabled || isLoading}
                isLoading={isLoading}
                isClearable={true}
                isRtl={false}
                isSearchable={searchable}
                name={name}
                value={selectedOption}
                onChange={handleInputChange}
                options={options}
                multiple={multiple}
                menuPortalTarget={document.body} // Esto permite que el menú se renderice a nivel de body
                styles={{menuPortal: (base) => ({...base, zIndex: 9999})}} // Ajustar el zIndex según sea necesario
                placeholder={emptyLabel || t("Seleccionar...")}
            />
        </div>
    );
};

const Option = ({children, ...props}) => {
    const {onMouseMove, onMouseOver, ...rest} = props.innerProps;
    const newProps = Object.assign(props, {innerProps: rest});
    return <components.Option {...newProps}>{children}</components.Option>;
};

SelectInput.propTypes = {
    onChange: PropTypes.func.isRequired,
    options: PropTypes.arrayOf(
        PropTypes.shape({
            value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
            label: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
        }).isRequired,
    ),
    emptyLabel: PropTypes.string,
    isLoading: PropTypes.bool,
    searchable: PropTypes.bool,
    name: PropTypes.string,
};
