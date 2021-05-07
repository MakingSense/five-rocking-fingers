import { DialogContent, TextField } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import Module from '../../interfaces/Module';
import { Controller, useForm, useFormContext } from 'react-hook-form';

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
        dialog: {
            "& .MuiDialogContent": {
                padding: 0
            }
        },
        error: {
            color: 'red'
        }
    }),
);

const ModulesForm = (props: {
    module: Module,
    setModule: Function
}) => {

    const { module, setModule } = props;

    const classes = useStyles();
    const moduleName = 'name';
    const description = 'description';
    const suggestedCost = 'suggestedCost';
    const { control, register, errors } = useFormContext();


    const handleInput = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        let { name, value } = event.target;
        let ModuleChanged = { ...module };
        switch (name) {
            case moduleName:
                ModuleChanged.name = value;
                break;
            case description:
                ModuleChanged.description = value;
                break;
            case suggestedCost:
                ModuleChanged.suggestedCost = Number.parseFloat(value);
                break;
        }
        setModule(ModuleChanged);
    }

    return (
        <>
            <DialogContent>
                <Controller
                    control={control}
                    name={'name'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() != ""
                        },
                        required: true
                    }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.name ? true : false}
                            id="name"
                            name="name"
                            label="Nombre del módulo"
                            helperText="Requerido*"
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={module.name}
                            autoComplete='off'
                            {...register('name', {
                                validate: {
                                    isValid: value => value.trim() != ""
                                },
                                required: true
                            })}
                        />
                    )}
                />
            </DialogContent>
            <DialogContent>
                <Controller
                    control={control}
                    name={'description'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() != ""
                        },
                        required: true
                    }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.description ? true : false}
                            id="description"
                            name="description"
                            label="Descripción del módulo"
                            helperText="Requerido*"
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={module.description}
                            autoComplete='off'
                            {...register('description', {
                                validate: {
                                    isValid: value => value.trim() != ""
                                },
                                required: true
                            })}
                        />
                    )}
                />
            </DialogContent>
            <DialogContent>
                <Controller
                    control={control}
                    name={'suggestedCost'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() != "",
                            positive: value => parseInt(value, 10) > 0
                        },
                        required: true
                    }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.suggestedCost ? true : false}
                            id="suggestedCost"
                            name="suggestedCost"
                            label="Costo de trabajo sugerido"
                            helperText={errors.suggestedCost && errors.suggestedCost.type === 'positive' ? "El costo de trabajo debe ser mayor o igual que 0" : "Requerido*"}
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={module.suggestedCost}
                            autoComplete='off'
                            type="number"
                            {...register('suggestedCost', {
                                validate: {
                                    isValid: value => value.trim() != "",
                                    positive: value => parseInt(value, 10) > 0
                                },
                                required: true
                            })}
                        />
                    )}
                />
            </DialogContent>
        </>
    );
}

export default ModulesForm; 