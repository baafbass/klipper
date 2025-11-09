import axiosInstance from './axiosConfig';
import { CreateSalonRequest, SalonManagerRequest } from '../types';

export const salonApi = {
getAll: () => axiosInstance.get('/salon'),
create: (data: CreateSalonRequest) => axiosInstance.post('/salon', data),
update: (id: string, data: Partial<CreateSalonRequest>) => axiosInstance.put(`/salon/${id}`, data),
delete: (id: string) => axiosInstance.delete(`/salon/${id}`),
addManager: (salonId: string, data: SalonManagerRequest) => axiosInstance.post(`/salon/${salonId}/manager`, data),
};