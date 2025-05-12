import React from 'react'
import { useTranslation } from 'react-i18next';
import { Icono } from './Icono';

export const EmptyDatatable = ({fontSize="",size=90, iconName="fa solid fa-circle-exclamation", color="#a0a1a1e0", informacion="No existen datos almacenados", height=27, ...rest}) => {
    const {t} = useTranslation();
    return (
        <div className="w-100 mt-3 d-flex flex-column justify-content-center" style={{height: `${height}vh`}}>
            <Icono name={iconName} style={{fontSize:`${size}px`, color:color}}/>
            <p className="text-center mt-3" style={{color: color, fontSize:`${fontSize}px`}}>
                <i>{t(informacion)}</i>
            </p>
        </div>
    );
}
