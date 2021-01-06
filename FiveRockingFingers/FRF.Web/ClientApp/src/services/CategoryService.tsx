import axios from 'axios';
import { BASE_URL } from '../Constants';
import Category from '../interfaces/Category';

const CATEGORIES_URL = `${BASE_URL}api/Categories/`;

class CategoryService {

    static getAll = async () => {
        return await axios.get(`${CATEGORIES_URL}GetAll`)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static get = async (id: number) => {
        return await axios.get(`${CATEGORIES_URL}Get/${id}`)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static save = async (category: Category) => {
        return await axios.post(`${CATEGORIES_URL}Save`, category)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static update = async (category: Category) => {
        return await axios.put(`${CATEGORIES_URL}Delete/${category.id}`, category)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static delete = async (id: number) => {
        return await axios.delete(`${CATEGORIES_URL}Delete/${id}`)
        .then(response => {
            if (response.status === 204) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }
}

export default CategoryService;