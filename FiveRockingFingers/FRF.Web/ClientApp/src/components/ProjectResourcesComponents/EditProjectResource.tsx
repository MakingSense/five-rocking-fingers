import * as React from 'react';
import { Button, Checkbox, createStyles, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormControlLabel, FormHelperText, Grid, InputLabel, makeStyles, MenuItem, Select, TextField, Theme, Tooltip, Typography } from '@material-ui/core';
import { Controller, useForm } from 'react-hook-form';
import ProjectResource from '../../interfaces/ProjectResource';
import Resource from '../../interfaces/Resource';
import { handleErrorMessage, isDateAfterNow, isDateBefore, tryExtractDate } from '../../commons/Helpers';
import ProjectResourceService from '../../services/ProjectResourceService';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
            display: 'flex',
            flexWrap: 'wrap',
            alignSelf: 'center'
        },
        formControl: {
            margin: theme.spacing(1),
            alignSelf: 'center',
            width: '55vh'
        },
        inputF: {
            padding: 2,
            marginTop: 10
        },
        gridDate: {
            marginLeft: "-4px",
            marginRight: 0,
            alignItems: "flex-end"
        }
    }),
);

const NewResourceDialog = (props: { open: boolean, handleClose: Function, manageOpenSnackbar: Function, updateList: Function, projectResource: ProjectResource, resources: Resource[] }) => {
    const classes = useStyles();
    const { open, handleClose, manageOpenSnackbar, updateList, projectResource, resources } = props;
    const [resourceId, setResourceId] = React.useState<number>(projectResource.resourceId);
    const [beginDate, setBeginDate] = React.useState<Date>(new Date());
    const [endDate, setEndDate] = React.useState<Date>(new Date());
    const [invalidEndDate, setInvalidEndDate] = React.useState<boolean>(false);
    const [invalidBeginDate, setInvalidBeginDate] = React.useState<boolean>(false);
    const [haveBeginDate, setHaveBeginDate] = React.useState<boolean>(projectResource.beginDate? true: false);
    const [haveEndDate, setHaveEndDate] = React.useState<boolean>(projectResource.endDate? true: false);
    const [projectResourceToUpdate, setProjectResourceToUpdate] = React.useState<ProjectResource>(projectResource);
    const {handleSubmit, errors, control } = useForm();

    const assignDates = () => {
        haveBeginDate ? projectResourceToUpdate.beginDate = beginDate : projectResourceToUpdate.beginDate = undefined;
        haveEndDate ? projectResourceToUpdate.endDate = endDate : projectResourceToUpdate.endDate = undefined;
    }

    const handleConfirm = async () => {
        assignDates();
        try {
            const response = await ProjectResourceService.update(projectResource.id,projectResourceToUpdate);
            if (response.status == 200) {
                updateList();
                manageOpenSnackbar({ message: "Recurso asignado al projecto modificado correctamente!", severity: "success" });
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al modificar el recurso asignado al proyecto!",
                    manageOpenSnackbar,
                    undefined
                );
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al modificar el recurso asignado al proyecto!", severity: "error" });
        }
        handleCancel();
    }

    const handleChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown; }>) => {
        let date: Date | null = null;
        if (event.target.name === 'resourceId') {
            setResourceId(event.target.value as number);
            setProjectResourceToUpdate({ ...projectResourceToUpdate, ['resource']: resources.find(r => r.id === event.target.value as number)! });
        }
        else if (event.target.name === 'beginDate') {
            date = tryExtractDate(event.target.value as string);
            if (date !== null) {
                setInvalidEndDate(false);
                setBeginDate(date);
                setProjectResourceToUpdate({ ...projectResourceToUpdate, [event.target.name]: beginDate });
            }
            else setInvalidBeginDate(true);
        }
        else if (event.target.name === 'endDate') {
            date = tryExtractDate(event.target.value as string);
            if (date !== null) {
                setInvalidEndDate(false);
                setEndDate(date);
                setProjectResourceToUpdate({ ...projectResourceToUpdate, [event.target.name]: endDate });
            }
            else setInvalidEndDate(true);
        }
        setProjectResourceToUpdate({ ...projectResourceToUpdate, [event.target.name!]: event.target.value });
    }

    const handleCancel = () => {
        setBeginDate(new Date());
        setEndDate(new Date());
        setInvalidEndDate(false);
        setInvalidBeginDate(false);
        handleClose();
    }

    return (
        <Dialog
            disableBackdropClick
            disableEscapeKeyDown
            fullWidth={true}
            maxWidth='sm'
            open={open}
        >
            <DialogTitle>Modificar recurso asignado</DialogTitle>
            <DialogContent dividers className={classes.container}>
                <form>
                    <FormControl variant="outlined" className={classes.formControl} error={Boolean(resourceId === 0) && errors.resourceId !== undefined}>
                        <InputLabel id="resourceId">Recurso</InputLabel>
                        <Controller
                            rules={{ validate: { isValid: () => resourceId !== 0 } }}
                            render={() =>
                                <Select
                                    labelId="resourceId"
                                    label="Recurso"
                                    name='resourceId'
                                    id='resourceId'
                                    onChange={event => { handleChange(event) }}
                                    defaultValue={projectResource.resourceId}
                                    error={Boolean(resourceId === 0) && errors.resourceId !== undefined}

                                >
                                    <MenuItem value={0}>
                                        <em>None</em>
                                    </MenuItem>
                                    {resources.map(r => <MenuItem key={r.id} value={r.id}>{r.roleName}</MenuItem>)}
                                </Select>
                            }
                            name='resourceId'
                            control={control}
                            defaultValue={projectResource.resourceId}
                        />
                        <FormHelperText>Requerido*</FormHelperText>
                    </FormControl>
                    <Grid>
                        <Grid container spacing={3} className={classes.gridDate}>
                            <Grid item xs={8}>
                                <FormControl style={{ width: "100%" }} error={!isDateAfterNow(beginDate) || invalidBeginDate}>
                                    <Controller
                                        rules={{ validate: { isValid: () => isDateAfterNow(beginDate) && !invalidBeginDate } }}
                                        render={() =>
                                            <TextField
                                                label="Fecha de inicio"
                                                placeholder="Seleccione una fecha de inicio para el recurso"
                                                variant="outlined"
                                                id="beginDate"
                                                type="date"
                                                name="beginDate"
                                                error={!isDateAfterNow(beginDate) || invalidBeginDate}
                                                defaultValue={projectResource.beginDate ? new Date(projectResource.beginDate).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10)}
                                                onChange={event => { handleChange(event) }}
                                                className={classes.inputF}
                                                InputLabelProps={{
                                                    shrink: true,
                                                }}
                                                helperText={
                                                    !isDateAfterNow(beginDate) ? "La fecha no puede ser anterior a hoy" :
                                                    invalidBeginDate ? "Formato de fecha invalido!" : ""}
                                                disabled={!haveBeginDate}
                                            />
                                        }
                                        name="beginDate"
                                        control={control}
                                        defaultValue={projectResource.beginDate ? new Date(projectResource.beginDate).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10)}
                                    />
                                </FormControl>
                            </Grid>
                            <Tooltip title={
                                <Typography variant="body2">Asignar una fecha de inicio es opcional, recuerda que siempre puedes editarla.
                            </Typography>} placement="right" arrow>
                                <FormControlLabel
                                    control={<Checkbox checked={haveBeginDate} onChange={() => { setHaveBeginDate((prev) => !prev); setInvalidBeginDate(false); }} color="primary" />}
                                    label="Fecha de inicio"
                                    style={{ marginRight: 7, height: '100%', paddingBottom: 15 }}
                                />
                            </Tooltip>
                        </Grid>
                    </Grid>
                    <Grid>
                        <Grid container spacing={3} className={classes.gridDate}>
                            <Grid item xs={8}>
                                <FormControl style={{ width: "100%" }} error={!isDateAfterNow(endDate) || invalidEndDate || !isDateBefore(beginDate, endDate)}>
                                    <Controller
                                        rules={{ validate: { isValid: () => isDateAfterNow(endDate) && !invalidEndDate && isDateBefore(beginDate, endDate) } }}
                                        render={() =>
                                            <TextField
                                                label="Fecha de fin"
                                                placeholder="Seleccione una fecha de finalizacion para el recurso"
                                                variant="outlined"
                                                id="endDate"
                                                type="date"
                                                name="endDate"
                                                error={!isDateAfterNow(endDate) || invalidEndDate || !isDateBefore(beginDate, endDate)}
                                                defaultValue={projectResource.endDate ? new Date(projectResource.endDate).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10)}
                                                onChange={event => { handleChange(event) }}
                                                className={classes.inputF}
                                                InputLabelProps={{
                                                    shrink: true,
                                                }}
                                                helperText={
                                                    !isDateAfterNow(beginDate) ? "La fecha no puede ser anterior a hoy" :
                                                    invalidEndDate ? "Formato de fecha invalido!" :
                                                    !isDateBefore(beginDate, endDate)? "La fecha no puede ser anterior a la fecha de inicio":""}
                                                disabled={!haveEndDate}
                                            />
                                        }
                                        name="endDate"
                                        control={control}
                                        defaultValue={projectResource.endDate ? new Date(projectResource.endDate).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10)}
                                    />
                                </FormControl>
                            </Grid>
                            <Tooltip title={
                                <Typography variant="body2">Asignar una fecha de finalizacion es opcional, recuerda que siempre puedes editarla.
                            </Typography>} placement="right" arrow>
                                <FormControlLabel
                                    control={<Checkbox
                                        checked={haveEndDate}
                                        onChange={() => {
                                          setHaveEndDate((prev) => !prev);
                                          setInvalidEndDate(false);
                                          setHaveBeginDate(true);
                                        }}
                                        color="primary"
                                      />}
                                    label="Fecha de fin"
                                    style={{ marginRight: 7, height: '100%', paddingBottom: 15 }}
                                />
                            </Tooltip>
                        </Grid>
                    </Grid>
                    <FormControl className={classes.formControl} error={errors.dedicatedHours ? true : false}>
                        <Controller
                            control={control}
                            name={'dedicatedHours'}
                            rules={{ validate: { isValid: value => value > 0 }, required: true }}
                            defaultValue={projectResource.dedicatedHours}
                            render={({ onChange }) => (
                                <TextField
                                    error={errors.dedicatedHours ? true : false}
                                    id="dedicatedHours"
                                    type="number"
                                    name="dedicatedHours"
                                    label="Horas de trabajo dedicadas"
                                    helperText={errors.dedicatedHours ? "Debe ser un valor mayor a cero" : "Requerido*"}
                                    variant="outlined"
                                    className={classes.inputF}
                                    onChange={event => { handleChange(event); onChange(event); }}
                                    defaultValue={projectResource.dedicatedHours}
                                    InputLabelProps={{
                                        shrink: true,
                                    }}
                                />
                            )}
                        />
                    </FormControl>
                </form>
            </DialogContent >
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog >
    )
}

export default NewResourceDialog;