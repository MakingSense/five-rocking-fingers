import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'
import './NavMenu.css';
import axios from 'axios'

const FaHome = () => (
    <div>
        <FontAwesomeIcon icon={faHome} />
    </div>
);

interface IProps {
}

interface IProjects {
    projects: undefined[];
}

export default class NavMenu extends React.PureComponent<IProps,IProjects> {
    
    constructor(props: IProps) {
        super(props);
        this.state = { projects: [] };
    }

    
    componentDidMount() {
        this.getProjectList();
    }

    public render() {        
        return (
            <ProSidebar>
               <SidebarHeader>
                    FiveRockingFingers
                    <Link to="/" />
                </SidebarHeader>
                <SidebarContent>
                    { this.state.projects }
                </SidebarContent>
                <SidebarFooter>
                    Hola
                </SidebarFooter>
            </ProSidebar>                            
        );
    }

    async getProjectList() {
        this.setState({ projects: [] });
        const response = await axios.get("https://localhost:44346/api/Projects/Get");
        let projectsMenuRender;
        projectsMenuRender = response.data.map((project: any) =>
            <Menu iconShape="circle">
                <SubMenu title={project.name}>
                </SubMenu>
            </Menu>
        );
        this.setState({ projects: projectsMenuRender });
    }
}
