import * as React from 'react';
import { Menu, ProSidebar, SidebarContent, SidebarFooter, SubMenu } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import Project from '../interfaces/Project';
import Category from "../interfaces/Category";
import '../styles/NavMenu.css';
import NavMenuItem from './NavMenuItem';
import NavMenuFilter from "./NavMenuFilter";
import ProjectService from '../services/ProjectService';
import CategoryService from "../services/CategoryService";
import TuneTwoToneIcon from "@material-ui/icons/TuneTwoTone";
import { Theme, withStyles, createStyles } from "@material-ui/core/styles";
import Badge from "@material-ui/core/Badge";
import {green} from "@material-ui/core/colors";
import { Typography } from '@material-ui/core';

const StyledBadge = withStyles((theme: Theme) =>
  createStyles({
    badge: {
      right: -3,
      top: 0,
      fontSize: 13,
      border: `2px solid ${theme.palette.background.paper}`,
      padding: "0 4px",
      color: green[500],
    },
  })
)(Badge);

const NavMenu = () => {
  const [projects, setProjects] = React.useState<Project[]>([]);
  const [projectsFiltered, setProjectsFiltered] = React.useState<Project[]>([]);
  const [categories, setCategories] = React.useState<Category[]>([]);
  const [categoriesFilter, setCategoriesFilter] = React.useState<Category[]>([]);

  const getProjects = async () => {
    try {
      const response = await ProjectService.getAll();
      if (response.status === 200) {
        setProjects(response.data);
      }
    } catch {
      setProjects([]);
    }
  };

  const getCategories = async () => {
    try {
      const response = await CategoryService.getAll();
      if (response.status === 200) setCategories(response.data);
    } catch {
      setCategories([]);
    }
  };

  React.useEffect(() => {
    getProjects();
    getCategories();
  }, []);

  const filterProjects = () => {
    const filterProjects = projects.filter((p) => {
      return p.projectCategories.some((pc) => {
        return categoriesFilter.some((cf) => {
          return cf.id === pc.category.id;
        });
      });
    });
    setProjectsFiltered(filterProjects);
  };

  React.useEffect(() => {
    filterProjects();
  }, [categoriesFilter]);

  const handleFilterCleaner = () => {
    setCategoriesFilter([]);
    setProjectsFiltered([]);
  }

  const handleNavMenuItem = () => {
    return projectsFiltered.length === 0 && categoriesFilter.length === 0
      ? projects.map((project) => (
          <NavMenuItem key={project.id} project={project} />
        ))
      :  
      projectsFiltered.length === 0 ? 
      <Typography align="center" variant="h6" gutterBottom>Categoría sin proyectos</Typography>
      : 
      projectsFiltered.map((project) => (
          <NavMenuItem key={project.id} project={project} />
        ));
  };

  return (
    <ProSidebar>
      <SidebarContent>
        <Menu iconShape="circle">
          <SubMenu
            title={"Filtros"}
            icon={
              <StyledBadge badgeContent={categoriesFilter.length}>
                <TuneTwoToneIcon />
              </StyledBadge>
            }
          >
            <NavMenuFilter
              categories={categories}
              setCategoriesFilter={setCategoriesFilter}
              cleaner={handleFilterCleaner}
            />
          </SubMenu>
        </Menu>
        <div
          style={{
            paddingTop: 0,
            marginTop: "-2vh",
          }}
        >
          <Menu iconShape="circle">
            {Array.isArray(projects) ? handleNavMenuItem() : "No hay projectos"}
          </Menu>
        </div>
      </SidebarContent>
      <SidebarFooter>
        <Link to="/administrarProyectos">Administrar proyectos</Link>
      </SidebarFooter>
    </ProSidebar>
  );
};

export default NavMenu;