/* eslint-disable no-restricted-globals */
import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { employeeApi } from '../api/employee';

type WorkingHours = { id: string; dayOfWeek: number; dayName: string; openTime: string; closeTime: string; isOpen: boolean };
type Schedule = { id: string; dayOfWeek: number; dayName: string; startTime: string; endTime: string; isActive: boolean };
type Service = { id: string; name: string; description?: string; durationMinutes: number; price: number; category?: string; isActive: boolean };

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function EmployeeDashboard() {
  const [tab, setTab] = useState<'schedules'|'services'>('schedules');

  // working hours
  const [workingHours, setWorkingHours] = useState<WorkingHours[]>([]);

  // schedules
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [newSchedule, setNewSchedule] = useState({ dayOfWeek: 1, startTime: '09:00', endTime: '17:00' });

  // services
  const [services, setServices] = useState<Service[]>([]);
  const [myServices, setMyServices] = useState<Service[]>([]);

  useEffect(() => { loadAll(); }, []);

  async function loadAll() {
    await Promise.all([loadWorkingHours(), loadSchedules(), loadServices(), loadMyServices()]);
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
    // client validation: find salon hours for the day
    const wh = workingHours.find(w => w.dayOfWeek === newSchedule.dayOfWeek);
    if (!wh || !wh.isOpen) {
      alert('Salon is closed that day — cannot create schedule.');
      return;
    }

    // convert "HH:MM" to "HH:mm:ss" if your backend expects seconds; backend accepts "HH:mm"
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

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Header />
      <main className="container mx-auto px-4 py-10 flex-1">
        <h1 className="text-2xl font-semibold mb-4">Employee Dashboard</h1>

        <nav className="mb-4 flex gap-3">
          <button onClick={()=>setTab('schedules')} className={`px-4 py-2 rounded ${tab==='schedules' ? 'bg-blue-600 text-white' : 'bg-white border'}`}>Schedules</button>
          <button onClick={()=>setTab('services')} className={`px-4 py-2 rounded ${tab==='services' ? 'bg-blue-600 text-white' : 'bg-white border'}`}>Services</button>
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
                        {/* remove requires employeeServiceId; if backend returns only service DTO, you may need an endpoint to remove by serviceId */}
                        <button onClick={() => alert('Use Remove service from manager panel or implement endpoint to remove by serviceId')} className="px-3 py-1 border rounded text-red-600">Remove</button>
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          </section>
        )}
      </main>
      <Footer />
    </div>
  );
}
