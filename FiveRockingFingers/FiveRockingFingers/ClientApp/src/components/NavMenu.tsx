import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'

const FaHome = () => (
    <div>
        <FontAwesomeIcon icon={faHome} />
    </div>
);

export default class NavMenu extends React.PureComponent<{}, { isOpen: boolean }> {
    public state = {
        isOpen: false
    };    

    public render() {
        return (
            <ProSidebar>
               <SidebarHeader>
                    FiveRockingFingers
                    <Link to="/" />
                </SidebarHeader>
                <SidebarContent>
                    <Menu iconShape="circle">
                        <SubMenu title="Projects">
                            <MenuItem>Project 1</MenuItem>
                            <SubMenu title="Infraestructure">
                            </SubMenu>
                        </SubMenu>
                    </Menu>
                </SidebarContent>
                <SidebarFooter>
                </SidebarFooter>
            </ProSidebar>                            
        );
    }

    private toggle = () => {
        this.setState({
            isOpen: !this.state.isOpen
        });
    }
}
