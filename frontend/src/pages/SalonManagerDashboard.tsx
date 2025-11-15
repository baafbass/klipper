/* eslint-disable no-restricted-globals */
import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { managerApi } from '../api/manager';
import { useAuthStore } from '../store/authStore';
import { WorkingHours,Service,Employee } from '../types';

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function SalonManagerDashboard() {
  const [tab, setTab] = useState<'hours' | 'services' | 'employees'>('hours');

  // working hours
  const [hours, setHours] = useState<WorkingHours[]>([]);
  const [whLoading, setWhLoading] = useState(false);
  const [whForm, setWhForm] = useState({ dayOfWeek: 1, openTime: '09:00', closeTime: '17:00', isOpen: true });
  const [editingWh, setEditingWh] = useState<WorkingHours | null>(null);

  // services
  const [services, setServices] = useState<Service[]>([]);
  const [serviceForm, setServiceForm] = useState({ name: '', description: '', durationMinutes: 30, price: 0, category: '' });
  const [editingService, setEditingService] = useState<Service | null>(null);

  // employees
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [employeeForm, setEmployeeForm] = useState({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '', commissionRate: '' });
  const [editingEmployee, setEditingEmployee] = useState<Employee | null>(null);
  const [schedules, setSchedules] = useState<any[]>([]);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);

  useEffect(() => {
    loadAll();
  }, []);

  const loadAll = async () => {
    await Promise.all([loadHours(), loadServices(), loadEmployees()]);
  };

  // Working hours handlers
  const loadHours = async () => {
    setWhLoading(true);
    try {
      const res = await managerApi.getWorkingHours();
      setHours(res.data);
    } catch (err) {
      console.error(err);
    } finally {
      setWhLoading(false);
    }
  };

  const addWh = async () => {
    try {
      await managerApi.addWorkingHours({
        dayOfWeek: whForm.dayOfWeek,
        openTime: whForm.openTime,
        closeTime: whForm.closeTime,
        isOpen: whForm.isOpen
      });
      setWhForm({ dayOfWeek: 1, openTime: '09:00', closeTime: '17:00', isOpen: true });
      loadHours();
    } catch (err: any) {
      alert(err?.response?.data?.error || 'Failed to add working hours');
    }
  };

  const startEditWh = (w: WorkingHours) => setEditingWh(w);
  const saveEditWh = async () => {
    if (!editingWh) return;
    try {
      await managerApi.updateWorkingHours(editingWh.id, { openTime: editingWh.openTime, closeTime: editingWh.closeTime, isOpen: editingWh.isOpen });
      setEditingWh(null);
      loadHours();
    } catch (err) {
      alert('Failed to save');
    }
  };
  const deleteWh = async (id: string) => {
    if (!confirm('Delete?')) return;
    await managerApi.deleteWorkingHours(id);
    loadHours();
  };

  // Services handlers
  const loadServices = async () => {
    try {
      const res = await managerApi.getServices();
      setServices(res.data);
    } catch (err) { console.error(err); }
  };

  const addService = async () => {
    try {
      await managerApi.addService(serviceForm);
      setServiceForm({ name: '', description: '', durationMinutes: 30, price: 0, category: '' });
      loadServices();
    } catch (err: any) { alert(err?.response?.data?.error || 'Failed'); }
  };

  const startEditService = (s: Service) => setEditingService(s);
  const saveEditService = async () => {
    if (!editingService) return;
    try {
      await managerApi.updateService(editingService.id, {
        name: editingService.name,
        description: editingService.description,
        durationMinutes: editingService.durationMinutes,
        price: editingService.price,
        category: editingService.category || ''
      });
      setEditingService(null);
      loadServices();
    } catch (err) { alert('Failed'); }
  };
  const deleteService = async (id: string) => { if (!confirm('Delete?')) return; await managerApi.deleteService(id); loadServices(); };

  // Employees handlers
  const loadEmployees = async () => {
    try {
      const res = await managerApi.getEmployees();
      setEmployees(res.data);
    } catch (err) { console.error(err); }
  };

  const addEmployee = async () => {
    try {
      await managerApi.addEmployee({
        email: employeeForm.email,
        password: employeeForm.password,
        firstName: employeeForm.firstName,
        lastName: employeeForm.lastName,
        phoneNumber: employeeForm.phoneNumber,
        commissionRate: employeeForm.commissionRate ? parseFloat(employeeForm.commissionRate) : undefined
      });
      setEmployeeForm({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '', commissionRate: '' });
      loadEmployees();
    } catch (err: any) { alert(err?.response?.data?.error || 'Failed'); }
  };

  const startEditEmployee = (e: Employee) => setEditingEmployee(e);
  const saveEditEmployee = async () => {
    if (!editingEmployee) return;
    try {
      await managerApi.updateEmployee(editingEmployee.id, {
        firstName: editingEmployee.firstName,
        lastName: editingEmployee.lastName,
        phoneNumber: editingEmployee.phoneNumber || '',
        commissionRate: editingEmployee.commissionRate ?? undefined
      });
      setEditingEmployee(null);
      loadEmployees();
    } catch (err) { alert('Failed'); }
  };
  const deleteEmployee = async (id: string) => { if (!confirm('Delete employee?')) return; await managerApi.deleteEmployee(id); loadEmployees(); };

  // schedules
  const loadSchedules = async (employeeId: string) => {
    try {
      const res = await managerApi.getEmployeeSchedules(employeeId);
      setSchedules(res.data);
    } catch (err) { console.error(err); }
  };

  const addSchedule = async (employeeId: string, dayOfWeek: number, start: string, end: string) => {
    try {
      await managerApi.addEmployeeSchedule({ employeeId, dayOfWeek, startTime: start, endTime: end });
      loadSchedules(employeeId);
    } catch (err) { alert('Failed to add schedule'); }
  };

  // employee-services assign/unassign
  const addEmployeeService = async (employeeId: string, serviceId: string) => {
    try {
      await managerApi.addEmployeeService({ employeeId, serviceId });
      loadEmployees();
    } catch (err) { alert('Failed to add'); }
  };
  const removeEmployeeService = async (id: string) => { if (!confirm('Remove?')) return; await managerApi.removeEmployeeService(id); loadEmployees(); };

  // small helpers to render inputs
  const timeInputValue = (t: string) => t || '09:00';

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Header />
      <main className="container mx-auto px-4 py-10 flex-1">
        <h1 className="text-2xl font-semibold mb-4">Salon Manager Dashboard</h1>

        <div className="mb-6">
          <nav className="flex gap-3">
            <button className={`px-4 py-2 rounded ${tab === 'hours' ? 'bg-blue-600 text-white' : 'bg-white border'}`} onClick={() => setTab('hours')}>Working Hours</button>
            <button className={`px-4 py-2 rounded ${tab === 'services' ? 'bg-blue-600 text-white' : 'bg-white border'}`} onClick={() => setTab('services')}>Services</button>
            <button className={`px-4 py-2 rounded ${tab === 'employees' ? 'bg-blue-600 text-white' : 'bg-white border'}`} onClick={() => setTab('employees')}>Employees</button>
          </nav>
        </div>

        {/* Working hours */}
        {tab === 'hours' && (
          <section className="bg-white p-6 rounded shadow">
            <h2 className="text-lg font-semibold mb-4">Working Hours</h2>
            <div className="grid md:grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block mb-1">Day</label>
                <select className="w-full p-2 border" value={whForm.dayOfWeek} onChange={(e) => setWhForm({ ...whForm, dayOfWeek: parseInt(e.target.value) })}>
                  {dayNames.map((d, i) => <option value={i} key={i}>{d}</option>)}
                </select>
              </div>
              <div>
                <label className="block mb-1">Open - Close</label>
                <div className="flex gap-2">
                  <input type="time" value={whForm.openTime} onChange={(e) => setWhForm({ ...whForm, openTime: e.target.value })} className="p-2 border" />
                  <input type="time" value={whForm.closeTime} onChange={(e) => setWhForm({ ...whForm, closeTime: e.target.value })} className="p-2 border" />
                </div>
              </div>
            </div>
            <div className="flex gap-2">
              <button onClick={addWh} className="px-4 py-2 bg-green-600 text-white rounded">Add Working Hours</button>
              <span className="text-sm text-gray-500 self-center">Existing entries:</span>
            </div>

            <div className="mt-4">
              {hours.length === 0 && <div className="text-gray-400">No working hours set.</div>}
              <ul className="divide-y">
                {hours.map(h => (
                  <li key={h.id} className="py-3 flex justify-between items-center">
                    <div>
                      <div className="font-medium">{dayNames[h.dayOfWeek]}</div>
                      <div className="text-sm text-gray-600">{h.isOpen ? `${h.openTime} — ${h.closeTime}` : 'Closed'}</div>
                    </div>
                    <div className="flex gap-2 items-center">
                      <button onClick={() => startEditWh(h)} className="px-3 py-1 border rounded">Edit</button>
                      <button onClick={() => deleteWh(h.id)} className="px-3 py-1 border rounded text-red-600">Delete</button>
                    </div>
                  </li>
                ))}
              </ul>
            </div>

            {/* Edit modal */}
            {editingWh && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
                <div className="bg-white p-4 rounded w-96">
                  <h3 className="font-semibold mb-2">Edit {dayNames[editingWh.dayOfWeek]}</h3>
                  <div className="grid gap-2">
                    <input type="time" value={editingWh.openTime} onChange={(e) => setEditingWh({ ...editingWh, openTime: e.target.value })} className="p-2 border" />
                    <input type="time" value={editingWh.closeTime} onChange={(e) => setEditingWh({ ...editingWh, closeTime: e.target.value })} className="p-2 border" />
                    <label className="flex items-center gap-2">
                      <input type="checkbox" checked={editingWh.isOpen} onChange={(e) => setEditingWh({ ...editingWh, isOpen: e.target.checked })} />
                      <span>Open</span>
                    </label>
                    <div className="flex justify-end gap-2">
                      <button onClick={() => setEditingWh(null)} className="px-3 py-1 border rounded">Cancel</button>
                      <button onClick={saveEditWh} className="px-3 py-1 bg-green-600 text-white rounded">Save</button>
                    </div>
                  </div>
                </div>
              </div>
            )}
          </section>
        )}

        {/* Services tab */}
        {tab === 'services' && (
          <section className="bg-white p-6 rounded shadow">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-semibold">Services</h2>
            </div>

            <div className="grid md:grid-cols-2 gap-4">
              <div className="p-4 border rounded">
                <h3 className="font-medium mb-2">Add service</h3>
                <input placeholder="Name" value={serviceForm.name} onChange={(e) => setServiceForm({ ...serviceForm, name: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="Category" value={serviceForm.category} onChange={(e) => setServiceForm({ ...serviceForm, category: e.target.value })} className="w-full p-2 border mb-2" />
                <textarea placeholder="Description" value={serviceForm.description} onChange={(e) => setServiceForm({ ...serviceForm, description: e.target.value })} className="w-full p-2 border mb-2" />
                <div className="flex gap-2">
                  <input type="number" value={serviceForm.durationMinutes} onChange={(e) => setServiceForm({ ...serviceForm, durationMinutes: Number(e.target.value) })} className="p-2 border w-1/2" />
                  <input type="number" value={serviceForm.price} onChange={(e) => setServiceForm({ ...serviceForm, price: Number(e.target.value) })} className="p-2 border w-1/2" />
                </div>
                <button onClick={addService} className="mt-3 px-4 py-2 bg-green-600 text-white rounded">Add</button>
              </div>

              <div>
                {services.length === 0 && <div className="text-gray-400">No services exist.</div>}
                <ul className="divide-y">
                  {services.map(s => (
                    <li key={s.id} className="py-3 flex justify-between items-start">
                      <div>
                        <div className="font-medium">{s.name} <span className="text-xs text-gray-500">({s.category})</span></div>
                        <div className="text-sm text-gray-600">{s.description}</div>
                        <div className="text-sm text-gray-600">{s.durationMinutes} min • {s.price} ₺</div>
                      </div>
                      <div className="flex gap-2">
                        <button onClick={() => startEditService(s)} className="px-3 py-1 border rounded">Edit</button>
                        <button onClick={() => deleteService(s.id)} className="px-3 py-1 border rounded text-red-600">Delete</button>
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            </div>

            {/* Edit service modal */}
            {editingService && (
              <div className="fixed inset-0 flex items-center justify-center bg-black/40 z-50">
                <div className="bg-white p-4 rounded w-96">
                  <h3 className="font-semibold mb-2">Edit Service</h3>
                  <input value={editingService.name} onChange={(e) => setEditingService({ ...editingService, name: e.target.value })} className="w-full p-2 border mb-2" />
                  <input value={editingService.category || ''} onChange={(e) => setEditingService({ ...editingService, category: e.target.value })} className="w-full p-2 border mb-2" />
                  <textarea value={editingService.description || ''} onChange={(e) => setEditingService({ ...editingService, description: e.target.value })} className="w-full p-2 border mb-2" />
                  <div className="flex gap-2">
                    <input type="number" value={editingService.durationMinutes} onChange={(e) => setEditingService({ ...editingService, durationMinutes: Number(e.target.value) })} className="p-2 border w-1/2" />
                    <input type="number" value={editingService.price} onChange={(e) => setEditingService({ ...editingService, price: Number(e.target.value) })} className="p-2 border w-1/2" />
                  </div>
                  <div className="flex justify-end gap-2 mt-3">
                    <button onClick={() => setEditingService(null)} className="px-3 py-1 border rounded">Cancel</button>
                    <button onClick={saveEditService} className="px-3 py-1 bg-green-600 text-white rounded">Save</button>
                  </div>
                </div>
              </div>
            )}
          </section>
        )}

        {/* Employees tab */}
        {tab === 'employees' && (
          <section className="bg-white p-6 rounded shadow">
            <div className="grid md:grid-cols-2 gap-4">
              <div className="p-4 border rounded">
                <h3 className="font-medium mb-2">Add Employee</h3>
                <input placeholder="Email" value={employeeForm.email} onChange={(e) => setEmployeeForm({ ...employeeForm, email: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="Password" value={employeeForm.password} onChange={(e) => setEmployeeForm({ ...employeeForm, password: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="First name" value={employeeForm.firstName} onChange={(e) => setEmployeeForm({ ...employeeForm, firstName: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="Last name" value={employeeForm.lastName} onChange={(e) => setEmployeeForm({ ...employeeForm, lastName: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="Phone" value={employeeForm.phoneNumber} onChange={(e) => setEmployeeForm({ ...employeeForm, phoneNumber: e.target.value })} className="w-full p-2 border mb-2" />
                <input placeholder="Commission rate (%)" value={employeeForm.commissionRate} onChange={(e) => setEmployeeForm({ ...employeeForm, commissionRate: e.target.value })} className="w-full p-2 border mb-2" />
                <button onClick={addEmployee} className="px-4 py-2 bg-green-600 text-white rounded">Add Employee</button>
              </div>

              <div>
                <h3 className="font-medium mb-2">Employees</h3>
                <div className="divide-y">
                  {employees.map(emp => (
                    <div key={emp.id} className="py-3 flex justify-between items-start">
                      <div>
                        <div className="font-medium">{emp.firstName} {emp.lastName} <span className="text-xs text-gray-500">({emp.email})</span></div>
                        <div className="text-sm text-gray-600">{emp.phoneNumber}</div>
                        <div className="text-sm text-gray-600">Services: {emp.services?.map(s => s.name).join(', ') || 'None'}</div>
                      </div>
                      <div className="flex gap-2">
                        <button onClick={() => { setSelectedEmployee(emp); loadSchedules(emp.id); }} className="px-3 py-1 border rounded">Schedules</button>
                        <button onClick={() => startEditEmployee(emp)} className="px-3 py-1 border rounded">Edit</button>
                        <button onClick={() => deleteEmployee(emp.id)} className="px-3 py-1 border rounded text-red-600">Delete</button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>

            {/* Edit employee modal */}
            {editingEmployee && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
                <div className="bg-white p-4 rounded w-96">
                  <h3 className="font-semibold mb-2">Edit Employee</h3>
                  <input value={editingEmployee.firstName} onChange={(e) => setEditingEmployee({ ...editingEmployee, firstName: e.target.value })} className="w-full p-2 border mb-2" />
                  <input value={editingEmployee.lastName} onChange={(e) => setEditingEmployee({ ...editingEmployee, lastName: e.target.value })} className="w-full p-2 border mb-2" />
                  <input value={editingEmployee.phoneNumber || ''} onChange={(e) => setEditingEmployee({ ...editingEmployee, phoneNumber: e.target.value })} className="w-full p-2 border mb-2" />
                  <div className="flex justify-end gap-2">
                    <button onClick={() => setEditingEmployee(null)} className="px-3 py-1 border rounded">Cancel</button>
                    <button onClick={saveEditEmployee} className="px-3 py-1 bg-green-600 text-white rounded">Save</button>
                  </div>
                </div>
              </div>
            )}

            {/* Schedules drawer/modal */}
            {selectedEmployee && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
                <div className="bg-white p-4 rounded w-[760px] max-w-full">
                  <div className="flex justify-between items-center mb-3">
                    <h3 className="font-semibold">Schedules for {selectedEmployee.firstName} {selectedEmployee.lastName}</h3>
                    <button onClick={() => { setSelectedEmployee(null); setSchedules([]); }} className="text-gray-600">Close</button>
                  </div>

                  <div>
   {/*                 <div className="grid grid-cols-3 gap-3">
                      <div>
                        <label className="text-sm">Day</label>
                        <select id="sch-day" className="w-full p-2 border">
                          {dayNames.map((d,i) => <option key={i} value={i}>{d}</option>)}
                        </select>
                      </div>
                      <div>
                        <label className="text-sm">Start</label>
                        <input id="sch-start" type="time" defaultValue="09:00" className="w-full p-2 border" />
                      </div>
                      <div>
                        <label className="text-sm">End</label>
                        <input id="sch-end" type="time" defaultValue="17:00" className="w-full p-2 border" />
                      </div>
                    </div>
                    <div className="mt-3 flex gap-2">
                      <button onClick={() => {
                        const day = Number((document.getElementById('sch-day') as HTMLSelectElement).value);
                        const start = (document.getElementById('sch-start') as HTMLInputElement).value;
                        const end = (document.getElementById('sch-end') as HTMLInputElement).value;
                        addSchedule(selectedEmployee.id, day, start, end);
                      }} className="px-3 py-1 bg-green-600 text-white rounded">Add schedule</button>
                    </div>*/}

                    <div className="mt-4">
                      {schedules.length === 0 && <div className="text-gray-400">No schedules</div>}
                      <ul className="divide-y">
                        {schedules.map((s:any) => (
                          <li key={s.id} className="py-2 flex justify-between">
                            <div>{dayNames[s.dayOfWeek]} — {s.startTime} - {s.endTime}</div>
                            <div>
                              <button onClick={() => { if(confirm('Delete schedule?')) managerApi.deleteEmployeeSchedule(s.id).then(()=>loadSchedules(selectedEmployee.id)); }} className="px-2 py-1 border rounded text-red-600">Delete</button>
                            </div>
                          </li>
                        ))}
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            )}
          </section>
        )}
      </main>
      <Footer />
    </div>
  );
}
