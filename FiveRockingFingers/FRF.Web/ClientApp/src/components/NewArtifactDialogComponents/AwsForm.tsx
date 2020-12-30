﻿import { Button, createStyles, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, makeStyles, TextField, Theme } from '@material-ui/core';
import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import Autocomplete from '@material-ui/lab/Autocomplete/Autocomplete';
import AwsArtifactService from '../../services/AwsArtifactService';
import AwsArtifact from '../../interfaces/AwsArtifact';
import ArtifactType from '../../interfaces/ArtifactType';
import { PROVIDERS } from '../../Constants';
import ArtifactService from '../../services/ArtifactService';
import CircularProgress from '@material-ui/core/CircularProgress';
import { useForm } from 'react-hook-form';

const constArtifactTypes = [
    {
        "id": 5,
        "name": "Atype",
        "description": "ADescription"
    }
]

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
      width: '100%',
      display: 'flex',
      alignItems: 'center'
    }
  }),
);

const AwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setArtifactTypeId: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function, setName: Function }) => {
  const classes = useStyles();
  const [open, setOpen] = React.useState(false);
  const [awsNames, setAwsNames] = React.useState([] as AwsArtifact[]);
    const [selectedName, setSelectedName] = React.useState<AwsArtifact | null>({ key: "", value: "" });
    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);
  const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;
    const loading = open && awsNames.length === 0;
    const { register, handleSubmit, errors, control, getValues } = useForm();

    React.useEffect(() => {
        setArtifactTypes(constArtifactTypes);
    let active = true;
    let statusOk = 200;
    if (!loading) {
      return undefined;
    };
    (async () => {
      try {
        const response = await AwsArtifactService.GetNamesAsync();
        if (active && response.status === statusOk) {
          setAwsNames(response.data);
        }
      }
      catch (error) {
        setSnackbarSettings({ message: "Fuera de servicio. Por favor intente mas tarde.", severity: "error" });
        setOpenSnackbar(true);
      }
    })();
    return () => {
      active = false;
    };
  }, [loading])

  //Create the artifact after submit
    const handleConfirm = async () => {
        props.setArtifactTypeId(artifactTypes[0].id);
        const name = selectedName?.key;
        props.setName(name);
        props.handleNextStep();
    }

  const handlePreviousStep = () => {
    props.handlePreviousStep();
  }

  const handleCancel = () => {
    closeNewArtifactDialog();
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
      <FormControl
        className={classes.formControl}
        error={Boolean(selectedName?.key === "")}
      >
        <Autocomplete
          id="name"
          options={awsNames}
          getOptionLabel={(option) => option.value}
          getOptionSelected={(option, value) => option.value === value.value}
          onChange={(__event: any, name: AwsArtifact | null) => {
            setSelectedName({
              key: name?.key as string,
              value: name?.value as string,
            });
          }}
          inputValue={selectedName?.value}
          onInputChange={(event, newValue) => {
            setSelectedName({
              key: "",
              value: newValue
            });
          }}
          className={classes.autoComplete}
          open={open}
          onOpen={() => {
            setOpen(true);
          }}
          onClose={() => {
            setOpen(false);
          }}
          loading={loading}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Nombre del artefacto AWS"
              variant="outlined"
              InputProps={{
                ...params.InputProps,
                endAdornment: (
                  <React.Fragment>
                    {loading ? (
                      <CircularProgress color="inherit" size={20} />
                    ) : null}
                    {params.InputProps.endAdornment}
                  </React.Fragment>
                ),
              }}
            />
          )}
        />
        <FormHelperText>Requerido*</FormHelperText>
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