import axios from 'axios';
import { BASE_URL } from '../Constants';
import Category from '../interfaces/Category';

const CATEGORIES_URL = `${BASE_URL}api/Categories/`;

class CategoryService {

    static getAll = async () => {
        const response = await axios.get(`${CATEGORIES_URL}GetAll`);
        return response;
    }

    static get = async (id: number) => {
        const response = await axios.get(`${CATEGORIES_URL}Get/${id}`);
        return response;
    }

    static save = async (category: Category) => {
        const response = await axios.post(`${CATEGORIES_URL}Save`, category);
        return response;
    }

    static update = async (category: Category) => {
        const response = await axios.put(`${CATEGORIES_URL}Delete/${category.id}`, category);
        return response;
    }

    static delete = async (id: number) => {
        const response = await axios.delete(`${CATEGORIES_URL}Delete/${id}`);
        return response;
    }
}

export default CategoryService;