﻿import { Button, createStyles, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, makeStyles, TextField, Theme } from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import Autocomplete from '@material-ui/lab/Autocomplete/Autocomplete';
import * as React from 'react';
import ArtifactType from '../../interfaces/ArtifactType';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    container: {
      display: 'flex',
      flexWrap: 'wrap',
    }, autoComplete: {
      padding: 2,
      marginTop: 10,
      width: 350
    },
    formControl: {
      margin: theme.spacing(1),
      width: '90%',
      display: 'flex',
      alignItems: 'center'
    }
  }),
);

const AwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setArtifactTypeId: Function, handleNextStep: Function, handlePreviousStep: Function, setName: Function, getArtifactTypes: Function }) => {
    const classes = useStyles();
    const [formError, setFormError] = React.useState(false);
    const [selectedArtifactType, setSelectedArtifactType] = React.useState<ArtifactType | null>(null);
    const [inputSelectedArtifactType, setInputSelectedArtifactType] = React.useState("");
    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    React.useEffect(() => {
        (async () => {
            let artifactTypes = await props.getArtifactTypes(0);
            setArtifactTypes(artifactTypes);
        })();
    }, [])

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (selectedArtifactType === null) {
            setFormError(true);
            return;
        }
        props.setArtifactTypeId(selectedArtifactType.id);
        const name = selectedArtifactType.name;
        props.setName(name);
        props.handleNextStep();
    }

    const handlePreviousStep = () => {
        props.handlePreviousStep();
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const handleInputChange = (event: object, newValue: string) => {
        setInputSelectedArtifactType(newValue);
        let selected = artifactTypes.filter(at => at.name === newValue);
        if (selected.length === 1) {
            setSelectedArtifactType(selected[0]);
            setFormError(false);
        } else {
            setSelectedArtifactType(null)
        }
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">
                Formulario de artefactos AWS
            </DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación seleccione el nombre de su nuevo artefacto AWS
                </Typography>
            </DialogContent>
            <FormControl className={classes.formControl} >
                    <Autocomplete
                        id="artifactType"
                        options={artifactTypes}
                        getOptionLabel={(option) => option.name}
                        getOptionSelected={(option, value) => {
                            return option.name === value.name
                        }}
                        inputValue={inputSelectedArtifactType}
                        onInputChange={handleInputChange}
                        className={classes.autoComplete}
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                label="Nombre del artefacto AWS"
                                variant="outlined"
                                error={formError}
                            />
                        )}
                    />
                <FormHelperText error={formError}>Requerido*</FormHelperText>
            </FormControl>

            <DialogActions>
                <Button size="small" color="primary" onClick={handlePreviousStep}>
                    Atrás
                </Button>
                <Button size="small" color="primary" type="submit" onClick={handleConfirm}>
                    Siguiente
                </Button>
                <Button size="small" color="secondary" onClick={handleCancel}>
                    Cancelar
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default AwsForm;