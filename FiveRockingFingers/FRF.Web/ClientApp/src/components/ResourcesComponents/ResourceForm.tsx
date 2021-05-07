import { DialogContent, TextField } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import Resource from '../../interfaces/Resource';
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

const ResourceForm = (props: {
    resource: Resource,
    setResource: Function
}) => {

    const { resource, setResource } = props;

    const classes = useStyles();
    const roleName = 'roleName';
    const description = 'description';
    const salaryPerMonth = 'salaryPerMonth';
    const workloadCapacity = 'workloadCapacity';
    const { control, register, errors } = useFormContext();


    const handleInput = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        let { name, value } = event.target;
        let resourceChanged = { ...resource };
        switch (name) {
            case roleName:
                resourceChanged.roleName = value;
                break;
            case description:
                resourceChanged.description = value;
                break;
            case salaryPerMonth:
                resourceChanged.salaryPerMonth = Number.parseFloat(value);
                break;
            case workloadCapacity:
                resourceChanged.workloadCapacity = Number.parseInt(value);
                break;
        }
        setResource(resourceChanged);
    }

    return (
        <>
            <DialogContent>
                <Controller
                    control={control}
                    name={'roleName'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() !== ""
                        },
                        required: true }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.roleName ? true : false}
                            id="roleName"
                            name="roleName"
                            label="Nombre del rol"
                            helperText="Requerido*"
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={resource.roleName}
                            autoComplete='off'
                            {...register('roleName', {
                                validate: {
                                    isValid: value => value.trim() !== ""
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
                            isValid: value => value.trim() !== ""
                        },
                        required: true }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.description ? true : false}
                            id="description"
                            name="description"
                            label="Descripción del rol"
                            helperText="Requerido*"
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={resource.description}
                            autoComplete='off'
                            {...register('roleName', {
                                validate: {
                                    isValid: value => value.trim() !== ""
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
                    name={'salaryPerMonth'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() !== "",
                            positive: value => parseInt(value, 10) > 0
                        },
                        required: true }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.salaryPerMonth ? true : false}
                            id="salaryPerMonth"
                            name="salaryPerMonth"
                            label="Salario por mes"
                            helperText={errors.salaryPerMonth && errors.salaryPerMonth.type === 'positive' ? "El salario debe ser mayor que 0" : "Requerido*"}
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={resource.salaryPerMonth}
                            autoComplete='off'
                            type="number"
                            {...register('roleName', {
                                validate: {
                                    isValid: value => value.trim() !== "",
                                    positive: value => parseInt(value, 10) > 0
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
                    name={'workloadCapacity'}
                    rules={{
                        validate: {
                            isValid: value => value.trim() !== "",
                            positive: value => parseInt(value, 10) > 0
                        },
                        required: true }}
                    render={({ onChange }) => (
                        <TextField
                            error={errors.workloadCapacity ? true : false}
                            id="workloadCapacity"
                            name="workloadCapacity"
                            label="Capacidad de trabajo"
                            helperText={errors.salaryPerMonth && errors.salaryPerMonth.type === 'positive' ? "La carga de trabajo debe ser mayor que 0" : "Requerido*"}
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleInput(event); onChange(event); }}
                            fullWidth
                            defaultValue={resource.workloadCapacity}
                            autoComplete='off'
                            type="number"
                            {...register('roleName', {
                                validate: {
                                    isValid: value => value.trim() !== "",
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

export default ResourceForm;