import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
import axios from 'axios';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface ProjectsState {
    isLoading: boolean;
    projects: Project[];
}

export interface Project {
    id: number;
    name: string;
    owner: string;
    client: string;
    budget: number;
    createdDate: Date;
    modifiedDate: Date;
    projectCategories: ProjectCategory[];
}

export interface ProjectCategory {
    projectCategory: Category;
}

export interface Category {
    id: number;
    name: string;
    description: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestProjectsAction {
    type: 'REQUEST_PROJECTS';
}

interface ReceiveProjectsAction {
    type: 'RECEIVE_PROJECTS';
    projects: Project[];
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestProjectsAction | ReceiveProjectsAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestProjects: (): AppThunkAction<KnownAction> => async (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        
            const response = await axios.get("http://localhost:4000/projects");
            console.log(response);
            dispatch({ type: 'RECEIVE_PROJECTS', projects: response.data });
            dispatch({ type: 'REQUEST_PROJECTS'});
        
    }
};

export function getProjectsAction() {
    return async (dispach: Function) => {
        dispach(requestProjectsAction());

        try {
            const response = await axios.get("http://localhost:4000/projects");
            dispach(loadProjectsOkAction(response.data));
        } catch {
            dispach(loadProjectsErrorAction());
        }
    }
}

const requestProjectsAction = () => ({

})

const loadProjectsOkAction = (projects: Project[]) => ({
    type: 'START_LOADING_PROJECTS',
    payload: true
})

const loadProjectsErrorAction = () => ({

})

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ProjectsState = { projects: [], isLoading: false };

export const reducer: Reducer<ProjectsState> = (state: ProjectsState | undefined, incomingAction: Action): ProjectsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_PROJECTS':
            return {
                projects: state.projects,
                isLoading: true
            };
        case 'RECEIVE_PROJECTS':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            return {
                projects: action.projects,
                isLoading: false
            };
        default:
            return state;
    }
};