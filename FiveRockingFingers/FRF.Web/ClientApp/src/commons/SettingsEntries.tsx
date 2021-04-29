import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { DialogContent, DialogTitle, TextField } from '@material-ui/core';
import Artifact from '../interfaces/Artifact';
import Setting from '../interfaces/Setting';
import ArtifactRelation from '../interfaces/ArtifactRelation';
import SettingEntry from './SettingEntry';
import { useArtifact } from './useArtifact';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
            display: 'flex',
            flexWrap: 'wrap',
        },
        formControl: {
            margin: theme.spacing(1),
            width: '100%'
        },
        inputF: {
            padding: 2,
            marginTop: 10
        },
        select: {
            marginBottom: 24,
            marginRight: 0,
            marginLeft: 0,
            marginTop: 12,
            "& .MuiSelect-outlined": {
                paddingBottom: 13
            }
        },
        error: {
            color: 'red'
        }
    }),
);

const SettingsEntries = (props: { artifactToEdit?: Artifact, artifactsRelations?: ArtifactRelation[] }) => {

    const classes = useStyles();
    const { control, register, setError, clearErrors } = useFormContext();
    const { isPriceValid, price, handleChangePrice, settingsList, settingsMap } = useArtifact();

    const isSettingAtTheEndOfAnyRelation = (settingName: string): boolean => {
        if (props.artifactsRelations === undefined) {
            return false;
        }
        const flag: ArtifactRelation | undefined = props.artifactsRelations
            .find((relation) =>
                isSettingAtTheEndOfRelation(settingName, relation));

        return flag === undefined ? false : true;
    }

    const isSettingAtTheEndOfRelation = (settingName: string, relation: ArtifactRelation): boolean => {
        if (props.artifactToEdit === undefined) {
            return false;
        }
        if (relation.relationTypeId === 0) {
            return props.artifactToEdit.id === relation.artifact2.id && relation.artifact2Property === settingName;
        }
        else {
            return props.artifactToEdit.id === relation.artifact1.id && relation.artifact1Property === settingName;
        }
    }

    //Set errors if the setting's name the user enters are repeat
    const setNameSettingsErrors = () => {
        for (let [key, array] of Object.entries(settingsMap)) {
            if (array.length > 1) {
                for (let i = 0; i < array.length; i++) {
                    setError(`settings[${array[i]}].name`, {
                        type: "repeat",
                        message: "Los nombres no pueden repetirse"
                    });
                }
            }
            else if (array.length === 1) {
                clearErrors(`settings[${array[0]}].name`);
            }
        }
    }

    React.useEffect(() => {
        setNameSettingsErrors();
    }, [settingsMap, props.artifactToEdit]);

    return (
        <>
            <DialogContent>
                <TextField
                    disabled
                    label="Nombre de la propiedad"
                    helperText={"Requerido*"}
                    variant="outlined"
                    defaultValue='Precio'
                    value='Precio'
                    className={classes.inputF}
                />
                <Controller
                    control={control}
                    name={'price.value'}
                    rules={{ validate: { isValid: () => isPriceValid() } }}
                    defaultValue={price}
                    render={({ onChange }) => (
                        <TextField
                            error={!isPriceValid()}
                            label="Valor"
                            helperText="Requerido*"
                            variant="outlined"
                            value={price}
                            className={classes.inputF}
                            onChange={event => { handleChangePrice(event); onChange(event); }}
                            autoComplete='off'
                            type="number"
                            {...register('price.value', {
                                validate: {
                                    isValid: () => isPriceValid()
                                }
                            })}
                        />
                    )}
                />
            </DialogContent>
            <DialogTitle style={{marginBottom: '-14px', paddingTop:1}} >Propiedades: </DialogTitle>
            {
                settingsList.map((setting: Setting, index: number) => (
                    <SettingEntry
                        index={index}
                        settingListLenght={settingsList.length}
                        setting={setting}
                        isSettingAtTheEndOfAnyRelation={isSettingAtTheEndOfAnyRelation}
                    />
                ))
            }
        </>
    );
}

export default SettingsEntries;