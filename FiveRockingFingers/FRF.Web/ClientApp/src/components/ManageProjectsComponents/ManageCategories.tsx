import { TextField } from '@material-ui/core';
import Autocomplete, { createFilterOptions } from '@material-ui/lab/Autocomplete';
import * as React from 'react';
import Category from '../../interfaces/Category';

const filter = createFilterOptions<Category>();

const ManageCategories = (props: { categories: Category[], selectedCategories: Category[], setSelectedCategories: Function }) => {
    const [tempCategories, setTempCategories] = React.useState([...props.categories]);

    React.useEffect(() => {
        setTempCategories([...props.categories]);
    }, [props.categories.length]);

    const handleChangeCategories = (event: React.ChangeEvent<{}>, value: Category[]) => {
        if (value.length === 0) {
            props.setSelectedCategories([]);
            return
        }
        if (tempCategories.filter(c => c.name === value[value.length - 1].name).length === 0) {
            let aux = [...tempCategories];
            aux.push(value[value.length - 1]);
            setTempCategories(aux);
        }
        props.setSelectedCategories(value);
    }

    return (
        <>
            <Autocomplete
                multiple
                id="tags-standard"
                options={tempCategories}
                fullWidth
                onChange={handleChangeCategories}
                autoHighlight
                filterOptions={(options, params) => {
                    var filtered = filter(options, params);

                    if (params.inputValue !== '' && tempCategories.find(c => c.name === params.inputValue) === undefined) {
                        filtered.unshift({
                            id: -1,
                            name: params.inputValue,
                            description: ""
                        });
                    }
                    return filtered;
                }}
                getOptionLabel={(option) => {
                    return option.name
                }}
                getOptionSelected={(option, value) => option.name === value.name}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        variant="outlined"
                        label="Categorías"
                        placeholder="Escriba el nombre de la categoría"
                    />
                )}
            />
        </>
        )
}

export default ManageCategories;