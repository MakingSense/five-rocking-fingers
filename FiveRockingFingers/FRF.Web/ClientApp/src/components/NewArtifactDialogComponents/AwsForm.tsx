﻿﻿import { Button, createStyles, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, makeStyles, TextField, Theme } from '@material-ui/core';
import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import Autocomplete from '@material-ui/lab/Autocomplete/Autocomplete';
import AwsArtifactService from '../../services/AwsArtifactService';
import AwsArtifact from '../../interfaces/AwsArtifact';
import { PROVIDERS } from '../../Constants'
import ArtifactService from '../../services/ArtifactService';
import CircularProgress from '@material-ui/core/CircularProgress';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
            display: 'flex',
            flexWrap: 'wrap',
        },autoComplete: {
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

const AwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handlePreviousStep: Function }) => {
    const classes = useStyles();
    const [open, setOpen] = React.useState(false);
    const [awsNames, setAwsNames] = React.useState([] as AwsArtifact[]);
    const [selectedName, setSelectedName] = React.useState<AwsArtifact | null>({ key: "", value: "" });
    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;
    const loading = open && awsNames.length === 0;

    React.useEffect(() => {
        let active = true;

        if (!loading) {
            return undefined;
        };
        (async () => {
            const response = await AwsArtifactService.GetNamesAsync();
            if (active) {
                setAwsNames(response.data);
            }
        })();
        return () => {
            active = false;
        };
    }, [loading])

    //Create the artifact after submit
    const handleConfirm = async () => {
        const artifactToCreate = {
            name: selectedName?.key,
            provider: PROVIDERS[0],
            artifactTypeId: 1,
            projectId: projectId,
            settings: null
        };

        try {
            const response = await ArtifactService.save(artifactToCreate);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El artefacto ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateList();
            } else {
                setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
                setOpenSnackbar(true);
            }
        }
        catch (error) {
            setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
            setOpenSnackbar(true);
        }
        closeNewArtifactDialog();
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
          <Button
            size="small"
            color="primary"
            type="submit"
            onClick={handleConfirm}
          >
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