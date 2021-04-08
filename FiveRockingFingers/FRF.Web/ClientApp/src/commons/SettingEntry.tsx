﻿import * as React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, IconButton, ButtonGroup, Select, MenuItem, Grid, FormGroup, FormHelperText, FormControl, InputLabel } from '@material-ui/core';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';

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

const SettingEntry = (index: number, settingListLenght: number) => {

    const classes = useStyles();
    const { handleSubmit, errors, setError, clearErrors, control } = useForm();

    return (
        <DialogContent key={index}>
            <FormGroup row>
                <Grid container>
                    <Grid item xs={5} zeroMinWidth spacing={0} style={{ flexBasis: '31%' }}>
                        <Controller
                            control={control}
                            name={`settings[${index}].name`}
                            key={index}
                            rules={{ validate: { isValid: () => !isFieldEmpty(index, "name", false), isRepeate: () => !areNamesRepeated(index) } }}
                            defaultValue={setting.name}
                            render={({ onChange }) => (
                                <TextField
                                    error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined' || setting.name === 'price'}
                                    id={`name[${index}]`}
                                    name={`settings[${index}].name`}
                                    label="Nombre"
                                    helperText={areNamesRepeated(index) ? "Los nombres no pueden repetirse" : "Requerido*"}
                                    variant="outlined"
                                    value={setting.name}
                                    className={classes.inputF}
                                    onChange={event => { handleInputChange(event, index); onChange(event); }}
                                    autoComplete='off'
                                />
                            )}
                        />
                    </Grid>

                    <Grid item xs={3} zeroMinWidth spacing={0}>
                        <Controller
                            control={control}
                            name={`settings[${index}].value`}
                            key={index}
                            rules={{ validate: { isValid: () => isValidNumber(index), isEmpty: () => !isFieldEmpty(index, "value", false) } }}
                            defaultValue={setting.value}
                            render={({ onChange }) => (
                                <TextField
                                    error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.value !== 'undefined' || !isValidNumber(index) || !isNumberSameType(index)}
                                    id={`value[${index}]`}
                                    name={`settings[${index}].value`}
                                    label="Valor"
                                    helperText={!isValidNumber(index) ? "Solo puede contener numeros positivos" : "Requerido*"}
                                    variant="outlined"
                                    value={setting.value}
                                    className={classes.inputF}
                                    onChange={event => { handleInputChange(event, index); onChange(event); }}
                                    autoComplete='off'
                                    type="number"
                                />
                            )}
                        />
                    </Grid>
                    <FormControl variant="outlined" className={classes.select} error={errors.relationalSettings && errors.relationalSettings[index] && typeof errors.relationalSettings[index] !== 'undefined' || !isNumberSameType(index)}>
                        <InputLabel id="settingTypeLabel">{!isNumberSameType(index) ? <Typography gutterBottom className={classes.error}>Tipo</Typography> : "Tipo"}</InputLabel>
                        <Controller
                            control={control}
                            name={`relationalSettings[${index}].type`}
                            key={index}
                            error={!isNumberSameType(index)}
                            rules={{ validate: { isValid: () => isValidNumber(index), isEmpty: () => !isFieldEmpty(index, "value", true) } }}
                            defaultValue={settingTypes[settingsList[index].name] === undefined ? '' : settingTypes[settingsList[index].name]}
                            render={({ onChange }) => (
                                <Select
                                    style={{ paddingTop: 5 }}
                                    labelId="settingTypeLabel"
                                    id="settingTypeLabel"
                                    name={`types[${index}]`}
                                    label="Tipo"
                                    autoWidth
                                    value={settingTypes[settingsList[index].name] === undefined ? '' : settingTypes[settingsList[index].name]}
                                    onChange={event => { handleTypeChange(event, index); onChange(event); }}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {SETTINGTYPES.map((value: string) => {
                                        return <MenuItem value={value}><em>{`${value.charAt(0).toUpperCase()}${value.slice(1).replace(/([a-z])([A-Z])/g, '$1 $2')}`}</em></MenuItem>
                                    })}
                                </Select>
                            )}
                        />
                        <FormHelperText>{!isNumberSameType(index) ? <Typography gutterBottom className={classes.error}>Tipo invalido</Typography> : "Requerido*"}</FormHelperText>
                    </FormControl>
                    <ButtonGroup size='small' key={index}>
                        {settingListLenght - 1 === index &&
                            <IconButton onClick={handleAddSetting} aria-label="add" color="primary">
                                <AddCircleIcon />
                            </IconButton>
                        }

                        {settingListLenght !== 1 &&
                            <IconButton onClick={() => handleDeleteSetting(index)} aria-label="delete" color="secondary">
                                <DeleteIcon />
                            </IconButton>
                        }
                    </ButtonGroup>
                </Grid>
            </FormGroup>
        </DialogContent>    
    );
}