import axiosInstance from './axiosConfig';

import {
  CreateWorkingHoursRequest,
  UpdateWorkingHoursRequest,
  CreateServiceRequest,
  UpdateServiceRequest,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
  CreateEmployeeScheduleRequest,
  CreateEmployeeServiceRequest
} from '../types';

export const managerApi = {
  // Working hours
  getWorkingHours: () => axiosInstance.get('/manager/working-hours'),
  addWorkingHours: (data: CreateWorkingHoursRequest) => axiosInstance.post('/manager/working-hours', data),
  updateWorkingHours: (id: string, data: UpdateWorkingHoursRequest) => axiosInstance.put(`/manager/working-hours/${id}`, data),
  deleteWorkingHours: (id: string) => axiosInstance.delete(`/manager/working-hours/${id}`),

  // Services
  getServices: () => axiosInstance.get('/manager/services'),
  addService: (data: CreateServiceRequest) => axiosInstance.post('/manager/services', data),
  updateService: (id: string, data: UpdateServiceRequest) => axiosInstance.put(`/manager/services/${id}`, data),
  deleteService: (id: string) => axiosInstance.delete(`/manager/services/${id}`),

  // Employees
  getEmployees: () => axiosInstance.get('/manager/employees'),
  addEmployee: (data: CreateEmployeeRequest) => axiosInstance.post('/manager/employees', data),
  updateEmployee: (id: string, data: UpdateEmployeeRequest) => axiosInstance.put(`/manager/employees/${id}`, data),
  deleteEmployee: (id: string) => axiosInstance.delete(`/manager/employees/${id}`),

  // Schedules & employee services
  getEmployeeSchedules: (employeeId: string) => axiosInstance.get(`/manager/employees/${employeeId}/schedules`),
  addEmployeeSchedule: (data: CreateEmployeeScheduleRequest) => axiosInstance.post('/manager/employees/schedules', data),
  deleteEmployeeSchedule: (id: string) => axiosInstance.delete(`/manager/employees/schedules/${id}`),

  addEmployeeService: (data: CreateEmployeeServiceRequest) => axiosInstance.post('/manager/employees/services', data),
  removeEmployeeService: (id: string) => axiosInstance.delete(`/manager/employees/services/${id}`),
};
