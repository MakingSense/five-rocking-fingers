import axios from 'axios';
import { BASE_URL } from '../Constants';
import Category from '../interfaces/Category';

const CATEGORIES_URL = `${BASE_URL}Categories/`;

class CategoryService {

    static getAll = async () => {
        try {
            return await axios.get(`${CATEGORIES_URL}GetAll`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${CATEGORIES_URL}Get/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (category: Category) => {
        try {
            return await axios.post(`${CATEGORIES_URL}Save`, category);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (category: Category) => {
        try {
            return await axios.put(`${CATEGORIES_URL}Delete/${category.id}`, category);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${CATEGORIES_URL}Delete/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default CategoryService;