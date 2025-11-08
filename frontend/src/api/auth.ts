import axiosInstance from './axiosConfig';
import { LoginRequest, RegisterRequest, LoginResponse } from '../types';

export const authApi = {
    login: (payload: LoginRequest) => axiosInstance.post<LoginResponse>('/auth/login', payload),
    register: (payload: RegisterRequest) => axiosInstance.post('/auth/register', payload),
    me: () => axiosInstance.get('/auth/me'),
};