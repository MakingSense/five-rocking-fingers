import * as React from "react";
import { createMuiTheme, makeStyles, TextField, Theme, ThemeProvider } from "@material-ui/core";
import { lime } from "@material-ui/core/colors";
import { Autocomplete } from "@material-ui/lab";
import Category from "../interfaces/Category";

const useStyles = makeStyles((theme: Theme) => ({
  inputRoot: {
    width: "90%",
    marginTop: 0,
    color: "#fafafa",
    fontWeight: "bold",
    background: "#545454",
    "& .MuiOutlinedInput-notchedOutline": {
      borderColor: "#b0bec6",
      border: `2px solid`
    },
    "&:hover .MuiOutlinedInput-notchedOutline": {
      borderColor: "#b0bec6",
      border: `2px solid`
    },
    "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
      borderColor: "#b0bec6",
      border: `2px solid`
    },
  },
}));

const theme = createMuiTheme({
  overrides: {
    MuiFormLabel: {
      root: {
        color: "#fafafa",
        "&$focused": {
          fontSize: 18,
          color: "#fafafa",
        },
      },
    },
  },
});

const NavMenuFilter = (props: {categories: Category[], setCategoriesFilter: Function ,cleaner: Function}) => {
  const classes = useStyles();
  const { categories, setCategoriesFilter, cleaner } = props;

  const handleChange = (_event: React.ChangeEvent<{}>, value: Category[], reason: string) => {
    if (reason === "clear") {
      cleaner();
      return;
    }
    if (value.length === 0) {
      setCategoriesFilter([]);
      return;
    }
    setCategoriesFilter(value);
  };

  return (
    <Autocomplete
      multiple
      limitTags={1}
      size="small"
      classes={classes}
      options={categories}
      getOptionSelected={(option, value) => option.id === value.id}
      onChange={handleChange}
      getOptionLabel={(option) => option.name}
      renderInput={(params) => (
        <ThemeProvider theme={theme}>
          <TextField
            {...params}
            variant="outlined"
            margin="normal"
            label="Filtro por categorias:"
          />
        </ThemeProvider>
      )}
    />
  );
};

export default NavMenuFilter;
