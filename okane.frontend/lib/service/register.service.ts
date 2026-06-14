import { getApi } from "../api";
import { RegisterFormSchema } from "../types/register";
import login from "./login.service";

export default async function register(data: RegisterFormSchema) {
    const api = await getApi();
    
    const response = await api.post('/auth/register', data);

    if (response.status != 201) {
        throw new Error(response.data.message);
    }

    await login({
        email: data.email,
        password: data.password
    });

    return;
}