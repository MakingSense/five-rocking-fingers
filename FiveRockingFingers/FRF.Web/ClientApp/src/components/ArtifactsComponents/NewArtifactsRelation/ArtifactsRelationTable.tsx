import * as React from 'react';
import { Table, Button } from 'reactstrap';
import SnackbarMessage from '../../../commons/SnackbarMessage';
import SnackbarSettings from '../../../interfaces/SnackbarSettings'
import ArtifactService from '../../../services/ArtifactService';
import NewArtifactsRelation from '../../NewArtifactsRelation';
import ArtifactRelationRow from './ArtifactRelationRow';
import ArtifactRelation from '../../../interfaces/ArtifactRelation';

const ArtifactsRelationTable = (props:{artifactId: string} ) => {

    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewArtifactDialog, setShowNewArtifactDialog] = React.useState(false);

    const getArtifactsRelations = async () => {
        try {
            console.log(props.artifactId);
            const response = await ArtifactService.getRelations(props.artifactId);

            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
        }    
    }

    const closeNewArtifactDialog = () => {
        setShowNewArtifactDialog(false);
    }

    const openNewArtifactDialog = () => {
        setShowNewArtifactDialog(true);
    }

    React.useEffect(() => {
        getArtifactsRelations();
    }, [props.artifactId]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    return (
        <React.Fragment>
            <Table striped bordered hover>
                <thead>
                    <tr>
                        <th>Artefacto 1</th>
                        <th>Artefacto 2</th>
                        <th>Setting 1</th>
                        <th>Setting 2</th>
                        <th>Direccion</th>
                        <th>
                           {/* <Button color="success" onClick={openNewArtifactsRelation}>Nueva relación</Button>*/}
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(artifactsRelations)
                        ? artifactsRelations.map((relation,index) => 
                        <ArtifactRelationRow key={index} artifactRelation={relation} openSnackbar={manageOpenSnackbar}/>
                            )
                        : null}
                </tbody>
            </Table>
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
            <Button />
           
        </React.Fragment>
    );
};

export default ArtifactsRelationTable;