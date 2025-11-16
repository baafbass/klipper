import axiosInstance from './axiosConfig';
import { CreateSalonRequest, SalonManagerRequest } from '../types';

export const salonApi = {
getAll: () => axiosInstance.get('/salons'),
getDetails: (id:string) => axiosInstance.get(`/salons/${id}/details`),
create: (data: CreateSalonRequest) => axiosInstance.post('/salons', data),
update: (id: string, data: Partial<CreateSalonRequest>) => axiosInstance.put(`/salons/${id}`, data),
delete: (id: string) => axiosInstance.delete(`/salons/${id}`),
addManager: (salonId: string, data: SalonManagerRequest) => axiosInstance.post(`/salons/${salonId}/manager`, data),
};