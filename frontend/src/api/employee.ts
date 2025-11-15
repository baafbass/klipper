import axiosInstance from './axiosConfig';

export const employeeApi = {
  getSalonWorkingHours: () => axiosInstance.get('/employee/working-hours'),
  getMySchedules: () => axiosInstance.get('/employee/schedules'),
  addSchedule: (data: { dayOfWeek: number; startTime: string; endTime: string }) => axiosInstance.post('/employee/schedules', data),
  //updateSchedule: (id: string, data: { dayOfWeek: number; startTime: string; endTime: string }) => axiosInstance.put(`/employee/schedules/${id}`, data),
  deleteSchedule: (id: string) => axiosInstance.delete(`/employee/schedules/${id}`),

  getSalonServices: () => axiosInstance.get('/employee/services'),
  getMyServices: () => axiosInstance.get('/employee/my-services'),
  assignService: (data: { serviceId: string }) => axiosInstance.post('/employee/my-services', data),
  removeMyService: (id: string) => axiosInstance.delete(`/employee/my-services/${id}`)
};
