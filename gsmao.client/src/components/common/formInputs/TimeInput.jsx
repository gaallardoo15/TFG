import React from 'react'
import { TextInput } from './TextInput';

export const TimeInput = ({...rest}) => {
    return <TextInput {...rest} type="time" />;
}
