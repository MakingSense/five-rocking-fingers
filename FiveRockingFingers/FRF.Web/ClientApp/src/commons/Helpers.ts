import { formatISO, isValid, parseISO } from 'date-fns';
import format from 'date-fns/format';
import * as React from 'react';
import * as Errors from '../ErrorCodes';

interface IErrorResponse {
    code: number;
    message: string;
}

export function handleErrorMessage(responseData: IErrorResponse, baseErrorMessage: string, setSnackbarSettings: Function, setOpenSnackbar: Function | undefined) {
    if (setOpenSnackbar === undefined) setOpenSnackbar = () => { };
    switch (responseData.code) {
        case Errors.PROJECT_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ El proyecto no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.ARTIFACT_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ El artefacto no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.INVALID_ARTIFACT_SETTINGS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ Las propiedades son invalidas o alguna de ellas no corresponden a su tipo`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.ARTIFACT_TYPE_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ El tipo de artefacto seleccionado no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ La relacion no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.ARTIFACT_FROM_ANOTHER_PROJECT:
            setSnackbarSettings({ message: `Al menos uno de los artefactos corresponde a otro proyecto`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_NOT_VALID_DIFFERENT_BASE_ARTIFACT:
            setSnackbarSettings({ message: `Al menos una de las relaciones corresponde a otro artefacto base`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_NOT_VALID_REPEATED:
            setSnackbarSettings({ message: `Al menos una de las relaciones está repetida`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_NOT_VALID_DIFFERENT_TYPE:
            setSnackbarSettings({ message: `Al menos una de las relaciones es de tipo incompatible`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_AL_READY_EXISTED:
            setSnackbarSettings({ message: `Al menos una de las relaciones ya existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.RELATION_CYCLE_DETECTED:
            setSnackbarSettings({ message: `Al menos una de las relaciones generaran un ciclo infinito`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.PROVIDER_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ El proveedor solicitado no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.USER_NOT_EXISTS:
            setSnackbarSettings({ message: `Hubo un error al obtener el perfil del usuario o el mismo no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.CATEGORY_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ Al menos una de las categorias no existe`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.PROJECT_RESOURCE_NOT_EXISTS:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ Al menos uno de los recursos no esta asignado al proyecto`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.INVALID_BEGINDATE_FOR_PROJECT_RESOURCE:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ La fecha de inicio no puede ser anterior a la fecha de inicio del proyecto`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case Errors.INVALID_ENDDATE_FOR_PROJECT_RESOURCE:
            setSnackbarSettings({ message: `${baseErrorMessage}:\ La fecha de finalizacion no puede ser anterior a la fecha de inicio del proyecto`, severity: "error" });
            setOpenSnackbar(true);
            break;
        case null:
            setSnackbarSettings({ message: `El artefacto no existe o no tienes autorizacion al mismo 🔒`, severity: "error" });
            setOpenSnackbar(true);
            break;
        default:
            setSnackbarSettings({ message: `${baseErrorMessage}`, severity: "error" });
            setOpenSnackbar(true);
            break;
    }
}

export function ToFormatedDate(date: Date): string {
    let stringDate = date.toString();
    let result = format(Date.parse(stringDate), 'dd/MM/yyyy');
    return result;
}

export function tryExtractDate(value: string) {
    let eventDate = new Date(value as string);
    if (isValid(eventDate)) {
        let date = eventDate.toISOString().slice(0, 10);
        return new Date(date);
    }
    return null;
}

export function isDateAfterNow(value: Date) {
    return formatISO(value, { representation: 'date' }) >= formatISO(new Date(), { representation: 'date' });
}

export function isDateBefore(beginDate: Date, endDate: Date) {
    return formatISO(endDate, { representation: 'date' }) >= formatISO(beginDate, { representation: 'date' });
}