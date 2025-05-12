import {TextInput} from "./TextInput";

export const NumberInput = ({min = 0, step = "1", ...rest}) => (
    <TextInput {...rest} controlProps={{min, step}} type="number" />
);
