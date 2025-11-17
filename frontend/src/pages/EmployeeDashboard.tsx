/* eslint-disable no-restricted-globals */
import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { employeeApi } from '../api/employee';
import dayjs from 'dayjs';

type WorkingHours = { id: string; dayOfWeek: number; dayName: string; openTime: string; closeTime: string; isOpen: boolean };
type Schedule = { id: string; dayOfWeek: number; dayName: string; startTime: string; endTime: string; isActive: boolean };
type Service = { id: string; name: string; description?: string; durationMinutes: number; price: number; category?: string; isActive: boolean };
type Appointment = {
  id: string;
  customerName: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  status: string;
  totalPrice: number;
  totalDurationMinutes: number;
  services: { serviceId: string; serviceName: string; price: number; durationMinutes: number }[];
  salonName?: string;
};

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function EmployeeDashboard() {
  const [tab, setTab] = useState<'schedules'|'services'|'appointments'>('schedules');

  // working hours
  const [workingHours, setWorkingHours] = useState<WorkingHours[]>([]);

  // schedules
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [newSchedule, setNewSchedule] = useState({ dayOfWeek: 1, startTime: '09:00', endTime: '17:00' });

  // services
  const [services, setServices] = useState<Service[]>([]);
  const [myServices, setMyServices] = useState<Service[]>([]);

  // appointments
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loadingAppointments, setLoadingAppointments] = useState(false);

  useEffect(() => { loadAll(); }, []);

  async function loadAll() {
    await Promise.all([loadWorkingHours(), loadSchedules(), loadServices(), loadMyServices(), loadAppointments()]);
  }

  async function loadWorkingHours() {
    try {
      const res = await employeeApi.getSalonWorkingHours();
      setWorkingHours(res.data);
    } catch (err) { console.error(err); }
  }

  async function loadSchedules() {
    try {
      const res = await employeeApi.getMySchedules();
      setSchedules(res.data);
    } catch (err) { console.error(err); }
  }

  async function addSchedule() {
    const wh = workingHours.find(w => w.dayOfWeek === newSchedule.dayOfWeek);
    if (!wh || !wh.isOpen) {
      alert('Salon is closed that day — cannot create schedule.');
      return;
    }

    const start = newSchedule.startTime;
    const end = newSchedule.endTime;

    if (end <= start) { alert('End must be after start'); return; }
    if (start < wh.openTime || end > wh.closeTime) {
      alert(`Schedule must be within salon working hours: ${wh.openTime} - ${wh.closeTime}`);
      return;
    }

    try {
      await employeeApi.addSchedule({ dayOfWeek: newSchedule.dayOfWeek, startTime: start, endTime: end });
      setNewSchedule({ dayOfWeek: 1, startTime: wh.openTime ?? '09:00', endTime: wh.closeTime ?? '17:00' });
      loadSchedules();
    } catch (err:any) {
      alert(err?.response?.data?.error || 'Failed to add schedule');
    }
  }

  async function deleteSchedule(id: string) {
    if (!confirm('Delete schedule?')) return;
    await employeeApi.deleteSchedule(id);
    loadSchedules();
  }

  // Services
  async function loadServices() {
    try {
      const res = await employeeApi.getSalonServices();
      setServices(res.data);
    } catch (err) { console.error(err); }
  }

  async function loadMyServices() {
    try {
      const res = await employeeApi.getMyServices();
      setMyServices(res.data);
    } catch (err) { console.error(err); }
  }

  async function assignService(serviceId: string) {
    try {
      await employeeApi.assignService({ serviceId });
      loadMyServices();
    } catch (err:any) { alert(err?.response?.data?.error || 'Failed'); }
  }

  async function removeService(employeeServiceId: string) {
    if (!confirm('Remove service from your profile?')) return;
    await employeeApi.removeMyService(employeeServiceId);
    loadMyServices();
    loadServices();
  }

  // Appointments
  async function loadAppointments() {
    setLoadingAppointments(true);
    try {
      const res = await employeeApi.getMyAppointments();
      setAppointments(res.data || []);
    } catch (err) { console.error(err); setAppointments([]); }
    finally { setLoadingAppointments(false); }
  }

  async function confirmAppointment(id: string) {
    if (!confirm('Confirm this appointment?')) return;
    try {
      await employeeApi.confirmAppointment(id);
      await loadAppointments();
    } catch (err:any) {
      alert(err?.response?.data?.error || 'Failed to confirm');
    }
  }

  async function completeAppointment(id: string) {
    if (!confirm('Mark appointment as completed?')) return;
    try {
      await employeeApi.completeAppointment(id);
      await loadAppointments();
    } catch (err:any) {
      alert(err?.response?.data?.error || 'Failed to complete');
    }
  }

  async function cancelAppointment(id: string) {
    if (!confirm('Cancel this appointment?')) return;
    try {
      await employeeApi.cancelAppointment(id, 'Cancelled by employee');
      await loadAppointments();
    } catch (err:any) {
      alert(err?.response?.data?.error || 'Failed to cancel');
    }
  }

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Header />
      <main className="container mx-auto px-4 py-10 flex-1">
        <h1 className="text-2xl font-semibold mb-4">Employee Dashboard</h1>

        <nav className="mb-4 flex gap-3">
          <button onClick={()=>setTab('schedules')} className={`px-4 py-2 rounded ${tab==='schedules' ? 'bg-blue-600 text-white' : 'bg-white border'}`}>Schedules</button>
          <button onClick={()=>setTab('services')} className={`px-4 py-2 rounded ${tab==='services' ? 'bg-blue-600 text-white' : 'bg-white border'}`}>Services</button>
          <button onClick={()=>setTab('appointments')} className={`px-4 py-2 rounded ${tab==='appointments' ? 'bg-blue-600 text-white' : 'bg-white border'}`}>Appointments</button>
        </nav>

         {tab === 'schedules' && (
          <section className="bg-white p-6 rounded shadow">
            <h2 className="text-lg font-semibold mb-3">Add Schedule</h2>

            <div className="grid md:grid-cols-3 gap-3 mb-3">
              <select value={newSchedule.dayOfWeek} onChange={(e)=>setNewSchedule({...newSchedule, dayOfWeek: Number(e.target.value)})} className="p-2 border">
                {dayNames.map((d,i)=><option key={i} value={i}>{d}</option>)}
              </select>

              <input type="time" value={newSchedule.startTime} onChange={(e)=>setNewSchedule({...newSchedule, startTime: e.target.value})} className="p-2 border" />
              <input type="time" value={newSchedule.endTime} onChange={(e)=>setNewSchedule({...newSchedule, endTime: e.target.value})} className="p-2 border" />
            </div>

            <div className="flex gap-2">
              <button onClick={addSchedule} className="px-4 py-2 bg-green-600 text-white rounded">Add Schedule</button>
              <div className="text-sm text-gray-500 self-center">Salon hours are shown below per day.</div>
            </div>

            <div className="mt-6">
              <h3 className="font-medium">My schedules</h3>
              <ul className="divide-y mt-2">
                {schedules.map(s => (
                  <li key={s.id} className="py-2 flex justify-between items-center">
                    <div>{dayNames[s.dayOfWeek]} — {s.startTime} - {s.endTime}</div>
                    <div><button onClick={() => deleteSchedule(s.id)} className="px-2 py-1 border rounded text-red-600">Delete</button></div>
                  </li>
                ))}
              </ul>
            </div>

            <div className="mt-6">
              <h3 className="font-medium">Salon working hours</h3>
              <ul className="divide-y mt-2">
                {workingHours.map(w => (
                  <li key={w.id} className="py-2 flex justify-between items-center">
                    <div>{dayNames[w.dayOfWeek]}</div>
                    <div>{w.isOpen ? `${w.openTime} - ${w.closeTime}` : 'Closed'}</div>
                  </li>
                ))}
              </ul>
            </div>
          </section>
        )}

          {tab === 'services' && (
          <section className="bg-white p-6 rounded shadow">
            <h2 className="text-lg font-semibold mb-3">Available Services (Salon)</h2>

            <div className="grid md:grid-cols-2 gap-4">
              <div>
                <ul className="divide-y">
                  {services.map(s => (
                    <li key={s.id} className="py-2 flex justify-between">
                      <div>
                        <div className="font-medium">{s.name}</div>
                        <div className="text-sm text-gray-600">{s.durationMinutes} min • {s.price}</div>
                      </div>
                      <div><button onClick={() => assignService(s.id)} className="px-3 py-1 bg-green-600 text-white rounded">Assign</button></div>
                    </li>
                  ))}
                </ul>
              </div>

              <div>
                <h3 className="font-medium mb-2">My Services</h3>
                <ul className="divide-y">
                  {myServices.map(ms => (
                    <li key={ms.id} className="py-2 flex justify-between">
                      <div>
                        <div className="font-medium">{ms.name}</div>
                        <div className="text-sm text-gray-600">{ms.durationMinutes} min • {ms.price}</div>
                      </div>
                      <div>
                        <button onClick={() => removeService(ms.id)} className="px-3 py-1 border rounded text-red-600">Remove</button>
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          </section>
        )}

        {tab === 'appointments' && (
          <section className="bg-white p-6 rounded shadow">
            <h2 className="text-lg font-semibold mb-3">Appointments Assigned to You</h2>

            {loadingAppointments ? <div>Loading...</div> : (
              appointments.length === 0 ? (
                <div className="text-sm text-gray-500">No appointments assigned.</div>
              ) : (
                <ul className="divide-y">
                  {appointments.map(a => (
                    <li key={a.id} className="py-3 flex justify-between items-start">
                      <div>
                        <div className="font-medium">{a.customerName} • {a.salonName}</div>
                        <div className="text-sm text-gray-600">{dayjs(a.appointmentDate).format('YYYY-MM-DD')} {a.startTime} - {a.endTime}</div>
                        <div className="mt-2">
                          <div className="text-sm font-medium">Services:</div>
                          <ul className="ml-4">
                            {a.services?.map(s => (
                              <li key={s.serviceId} className="text-sm text-gray-700">{s.serviceName} — {s.durationMinutes} min — {s.price} ₺</li>
                            ))}
                          </ul>
                        </div>
                        <div className="text-sm mt-1">Total: <strong>{a.totalPrice} ₺</strong> • Status: <strong>{a.status}</strong></div>
                        {a.status === 'Cancelled' && <div className="text-xs text-red-600 mt-1">Cancelled</div>}
                      </div>

                      <div className="flex flex-col gap-2">
                        {a.status === 'Pending' && <button onClick={() => confirmAppointment(a.id)} className="px-3 py-1 bg-green-600 text-white rounded text-sm">Confirm</button>}
                        {a.status === 'Confirmed' && <button onClick={() => completeAppointment(a.id)} className="px-3 py-1 bg-indigo-600 text-white rounded text-sm">Complete</button>}
                        {(a.status === 'Pending' || a.status === 'Confirmed') && <button onClick={() => cancelAppointment(a.id)} className="px-3 py-1 border rounded text-red-600 text-sm">Cancel</button>}
                      </div>
                    </li>
                  ))}
                </ul>
              )
            )}
          </section>
        )}

      </main>
      <Footer />
    </div>
  );
}

