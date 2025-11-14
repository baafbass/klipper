import axiosInstance from './axiosConfig';
import { LoginRequest, RegisterRequest, LoginResponse } from '../types';

export const authApi = {
    login: (payload: LoginRequest) => axiosInstance.post<LoginResponse>('/auth/login', payload),
    loginSysAdmin: (payload: LoginRequest) => axiosInstance.post<LoginResponse>('/auth/login-sysadmin', payload),
    loginSalonManager: (payload: LoginRequest) => axiosInstance.post<LoginResponse>('/auth/login-salonmanager', payload),
    loginEmployee: (payload: LoginRequest) => axiosInstance.post<LoginResponse>('/auth/login-employee', payload),
    register: (payload: RegisterRequest) => axiosInstance.post('/auth/register', payload),
    me: () => axiosInstance.get('/auth/me'),
};