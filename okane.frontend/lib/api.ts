import axios from 'axios';
import { createHeaders } from './service/retrieve-token.service';

export const getApi = async () => {
    const headers = await createHeaders();
    
    const api = axios.create({
        baseURL: process.env.API_URL,
        headers
    });

    return api;
}