import {fakerES as faker} from "@faker-js/faker";

import {IS_DEBUG} from "@/components/App";

faker.seed(42);
// affects all future faker.date.* calls
// faker.setDefaultRefDate('2023-01-01T00:00:00.000Z');

// @See https://fakerjs.dev/api/
const newUser = () => ({
    id: faker.string.uuid(),
    name: faker.person.firstName(),
    lastName: faker.person.lastName(),
    password: faker.internet.password(),
    email: faker.internet.email(),
    rol: {id: faker.number.int({max: 3}), name: faker.helpers.arrayElement(["ADMINISTRADOR", "RESPONSABLE", "SUPER_ADMINISTRADOR"])},
    empresa: {id: 1, descripcion: "Hitachi Energy"},
    estado: {id: faker.number.int({max: 3}), descripcion: faker.helpers.arrayElement(["Activo", "Borrado", "Inactivo"])},
    planta: {id: faker.number.int({max: 1}), descripcion:"Planta de Córdoba" },
    
});

const newLocalizacion = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es: faker.location.city(),
    descripcion_en: faker.location.city(),
    localizacion_sap: faker.number.int({max: 333}),
    latitud: faker.number.float({max:99.99}),
    longitud: faker.number.float({max: 99.99}),
    planta: {id: 1, descripcion_es:"Planta de Córdoba" }
});

const newCentroCoste = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es: faker.lorem.words(),
    descripcion_en: faker.lorem.words(),
    centroCoste_sap: faker.number.int({max: 333}),
    planta: {id: 1, descripcion_es:"Planta de Córdoba"}
});

const newMecanismoFallo = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es: faker.lorem.words(),
    descripcion_en: faker.lorem.words(),
});
const newIncidencia = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es:faker.lorem.words(),
    descripcion_en:faker.lorem.words(),
    mecanismoFallo :{id: faker.number.int({max: 30}), descripcion_es: faker.lorem.words()}
});
const newResolucion = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es: faker.lorem.words(),
    descripcion_en: faker.lorem.words()
});

const newActivo = () => ({
    id: faker.number.int({max: 99999}),
    descripcion_es: faker.lorem.words(),
    descripcion_en: faker.lorem.words(),
    activo_sap: faker.string.alphanumeric(6),
    Localizacion: {id:faker.number.int({max: 30}), descripcion_es:faker.location.city() },
    CentroCoste: {id:faker.number.int({max: 30}), descripcion_es:faker.lorem.words(1)},
    actividad: faker.helpers.arrayElement(["Activo", "Borrado", "Inactivo"]),
    criticidad: faker.helpers.arrayElement(["MC", "CA", "SC","CM", "CB" ]),
    redundancia: faker.number.int({max: 30}),
    valor_criticidad: faker.number.int({max: 100}),
    coste: faker.number.int({max: 30}),
    usabilidad: faker.number.int({max: 30}),
    hse: faker.number.int({max: 30}),
    
});

const newComponente = () => ({
    id: faker.number.int({max: 999999}),
    descripcion_es:faker.lorem.words(),
    kks: faker.string.alphanumeric(6),
});

const range = (len) => {
    const arr = [];
    for (let i = 0; i < len; i++) {
        arr.push(i);
    }
    return arr;
};

function makeData(len, type) {
    return IS_DEBUG ? range(len).map(() => type()) : [];
}

export function makeUsers(len) {
    return makeData(len, newUser);
}

export function makeLocalizaciones(len) {
    return makeData(len, newLocalizacion);
}

export function makeCentrosCostes(len) {
    return makeData(len, newCentroCoste);
}
export function makeMecanismosFallo(len) {
    return makeData(len, newMecanismoFallo);
}
export function makeIncidencias(len) {
    return makeData(len, newIncidencia);
}
export function makeResoluciones(len) {
    return makeData(len, newResolucion);
}
export function makeActivos(len) {
    return makeData(len, newActivo);
}
export function makeComponentes(len) {
    return makeData(len, newComponente);
}