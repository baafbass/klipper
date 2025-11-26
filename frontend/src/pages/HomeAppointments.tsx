// src/pages/HomeAppointments.tsx
import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { salonApi } from '../api/salon';
import { appointmentApi } from '../api/appointments';
import { useAuthStore } from '../store/authStore';
import dayjs from 'dayjs';

export default function HomeAppointments() {
  const [salons, setSalons] = useState<any[]>([]);
  const [selectedSalon, setSelectedSalon] = useState<any | null>(null);
  const [date, setDate] = useState<string>(new Date().toISOString().slice(0,10));
  const [availability, setAvailability] = useState<any[]>([]);
  const [selectedServiceIds, setSelectedServiceIds] = useState<string[]>([]);
  const [selectedEmployeeId, setSelectedEmployeeId] = useState<string | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<any | null>(null);
  const [notes, setNotes] = useState('');
  const [myAppointments, setMyAppointments] = useState<any[]>([]);
  const [loadingAppointments, setLoadingAppointments] = useState(false);

  const auth = useAuthStore();

  useEffect(() => {
    loadSalons();
    if (auth.isAuthenticated) loadMyAppointments();
    // refresh appointments when user logs in/out
  }, [auth.isAuthenticated]);

  async function loadSalons() {
    try {
      const res = await salonApi.getAll();
      setSalons(res.data || []);
    } catch (err:any) {
      console.error('Failed loading salons', err);
    }
  }

  async function loadSalonDetails(id: string) {
    try {
      const res = await salonApi.getDetails(id);
      setSelectedSalon(res.data);
      // clear previous selection/state
      setAvailability([]);
      setSelectedServiceIds([]);
      setSelectedEmployeeId(null);
    } catch (err:any) {
      console.error('Failed loading salon details', err);
      alert(err?.response?.data?.error || 'Failed to load salon');
    }
  }

  function selectService(id: string) {
    setSelectedServiceIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id]);
  }

  async function checkAvailabilityForEmployee(employeeId: string) {
    if (!selectedSalon) return;
    if (selectedServiceIds.length === 0) { alert('Choose one or more services'); return; }

    setSelectedEmployeeId(employeeId);
    try {
      const payload = {
        salonId: selectedSalon.id,
        employeeId,
        date,
        serviceIds: selectedServiceIds,
      };
      const res = await appointmentApi.getAvailability(payload);
      // backend returns StartTime/EndTime as timespan strings or HH:mm:ss — keep as-is
      setAvailability(res.data || []);
    } catch (err:any) {
      console.error(err);
      alert(err?.response?.data?.error || 'Failed to check availability');
    }
  }

  // helper to format times from backend (support TimeSpan strings)
  const fmtTime = (t: string | null | undefined) => {
    if (!t) return '';
    // If t is "HH:mm:ss" or "hh:mm", use dayjs to format to "HH:mm"
    const parsed = dayjs(`1970-01-01T${t}`);
    return parsed.isValid() ? parsed.format('HH:mm') : t;
  };

  // ensure startTime is HH:mm:ss for server
  const toTimeString = (time: string) => {
    // if already "HH:mm:ss", return
    if (/^\d{1,2}:\d{2}:\d{2}$/.test(time)) return time;
    if (/^\d{1,2}:\d{2}$/.test(time)) return `${time}:00`;
    return time;
  };

  async function book(slot: any) {
    if (!auth.user) { alert('Please login as customer'); return; }
    try {
      const payload = {
        customerId: auth.user.id,
        employeeId: slot.employeeId,
        salonId: selectedSalon.id,
        appointmentDate: date,
        startTime: toTimeString(slot.startTime),
        serviceIds: selectedServiceIds,
        notes
      };
      const res = await appointmentApi.createAppointment(payload);
      alert('Appointment created successfully (pending).');
      setAvailability([]);
      setSelectedServiceIds([]);
      setSelectedSlot(null);
      setNotes('');
      // reload customer's appointments
      loadMyAppointments();
    } catch (err:any) {
      console.error('Booking error', err);
      alert(err?.response?.data?.error || 'Booking failed');
    }
  }

  async function loadMyAppointments() {
    if (!auth.isAuthenticated) {
      setMyAppointments([]);
      return;
    }
    setLoadingAppointments(true);
    try {
      const res = await appointmentApi.getMyAppointments();
      setMyAppointments(res.data || []);
    } catch (err:any) {
      console.error('Failed loading appointments', err);
      setMyAppointments([]);
    } finally {
      setLoadingAppointments(false);
    }
  }

  async function cancelAppointment(id: string) {
    if (!window.confirm('Cancel this appointment?')) return;
    try {
      await appointmentApi.cancelAppointment(id, 'Cancelled by customer');
      alert('Appointment cancelled.');
      loadMyAppointments();
    } catch (err:any) {
      console.error(err);
      alert(err?.response?.data?.error || 'Failed to cancel');
    }
  }

  return (
    <div >
      <Header />
      <main className="min-h-screen container mx-auto p-6">
        <div className="grid md:grid-cols-4 gap-6">
          {/* Salons list */}
          <div className="col-span-1 bg-white p-4 rounded shadow">
            <h2 className="font-semibold">Salons</h2>
            <ul>
              {salons.map(s => (
                <li key={s.id} className="py-2 flex justify-between">
                  <div>
                    <div className="font-medium">{s.name}</div>
                    <div className="text-sm text-gray-500">{s.city}</div>
                  </div>
                  <button onClick={() => loadSalonDetails(s.id)} className="text-blue-600">Open</button>
                </li>
              ))}
            </ul>

            {/* My Appointments */}
            <div className="mt-6">
              <h3 className="font-semibold">My Appointments</h3>
              {!auth.isAuthenticated && <div className="text-sm text-gray-500 mt-2">Log in as a customer to see your bookings.</div>}
              {auth.isAuthenticated && (
                <div className="mt-2">
                  {loadingAppointments ? <div>Loading...</div> : (
                    myAppointments.length === 0 ? (
                      <div className="text-sm text-gray-500">No appointments yet.</div>
                    ) : (
                      <ul className="space-y-2">
                        {myAppointments.map((a:any) => (
                          <li key={a.id} className="p-2 border rounded">
                            <div className="flex justify-between items-start">
                              <div>
                                <div className="font-medium">{a.salonName} • {a.employeeName}</div>
                                <div className="text-sm text-gray-600">
                                  {dayjs(a.appointmentDate).format('YYYY-MM-DD')} {fmtTime(a.startTime)} - {fmtTime(a.endTime)}
                                </div>
                                <div className="text-sm">Services:
                                  <ul className="ml-3 list-disc">
                                    {a.services?.map((s:any) => (
                                      <li key={s.serviceId} className="text-sm text-gray-700">
                                        {s.serviceName} — {s.durationMinutes} min — {s.price} ₺
                                      </li>
                                    ))}
                                  </ul>
                                </div>
                                <div className="text-sm mt-1">Total: <strong>{a.totalPrice} ₺</strong> • Status: <strong>{a.status}</strong></div>
                              </div>
                              <div className="text-right">
                                {/* If pending or confirmed, allow cancel */}
                                {(a.status === 'Pending' || a.status === 'Confirmed') && (
                                  <button onClick={() => cancelAppointment(a.id)} className="px-2 py-1 bg-red-600 text-white rounded text-sm">Cancel</button>
                                )}
                                <div className="text-xs text-gray-400 mt-2">#{a.id?.slice(0,8)}</div>
                              </div>
                            </div>
                          </li>
                        ))}
                      </ul>
                    )
                  )}
                </div>
              )}
            </div>
          </div>

          {/* Main: selected salon and booking UI */}
          <div className="col-span-3">
            {!selectedSalon && <div className="bg-white p-6 rounded shadow">Select a salon</div>}

            {selectedSalon && (
              <div className="bg-white p-4 rounded shadow">
                <h2 className="text-xl font-semibold">{selectedSalon.name}</h2>
                <p className="text-sm text-gray-600">{selectedSalon.address} • {selectedSalon.city}</p>

                <div className="mt-4 grid md:grid-cols-2 gap-4">
                  <div>
                    <h3 className="font-medium">Services</h3>
                    <div className="grid gap-2 mt-2">
                      {selectedSalon.services.map((svc:any) => (
                        <div key={svc.id} className="p-3 border rounded">
                          <div className="flex justify-between">
                            <div>
                              <div className="font-medium">{svc.name}</div>
                              <div className="text-sm text-gray-500">{svc.durationMinutes} min • {svc.price} ₺</div>
                            </div>
                            <input type="checkbox"
                                   checked={selectedServiceIds.includes(svc.id)}
                                   onChange={() => selectService(svc.id)} />
                          </div>

                          <div className="mt-2">
                            <div className="text-sm text-gray-600">Employees who can do this:</div>
                            <ul className="mt-1">
                              {svc.employees.map((emp:any) => (
                                <li key={emp.id} className="flex justify-between items-center py-1">
                                  <div>{emp.firstName} {emp.lastName}</div>
                                  <div>
                                    <button onClick={() => checkAvailabilityForEmployee(emp.id)} className="px-2 py-1 text-sm bg-blue-600 text-white rounded">Check availability</button>
                                  </div>
                                </li>
                              ))}
                            </ul>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div>
                    <h3 className="font-medium">Choose Date</h3>
                    <input type="date" value={date} onChange={(e)=>setDate(e.target.value)} className="p-2 border rounded mt-2 w-full" />

                    <div className="mt-4">
                      <h4 className="font-medium">Availability</h4>
                      <div className="mt-2">
                        {availability.length === 0 ? <div className="text-sm text-gray-500">No slots loaded. Choose services and click "Check availability" next to an employee.</div> : (
                          <ul>
                            {availability.map((slot:any, i:number) => (
                              <li key={i} className="flex justify-between items-center py-2 border-b">
                                <div>
                                  <div className="font-medium">{slot.employeeName}</div>
                                  <div className="text-sm text-gray-600">{fmtTime(slot.startTime)} - {fmtTime(slot.endTime)} ({slot.durationMinutes ?? '—'} min)</div>
                                </div>
                                <div className="flex gap-2">
                                  <button onClick={() => { setSelectedSlot(slot); }} className="px-3 py-1 bg-green-600 text-white rounded">Select</button>
                                </div>
                              </li>
                            ))}
                          </ul>
                        )}
                      </div>

                      {selectedSlot && (
                        <div className="mt-4 p-3 border rounded bg-gray-50">
                          <div>Selected: {selectedSlot.employeeName} — {fmtTime(selectedSlot.startTime)} to {fmtTime(selectedSlot.endTime)}</div>
                          <label className="block mt-2">Notes
                            <textarea value={notes} onChange={(e)=>setNotes(e.target.value)} className="w-full p-2 border rounded mt-1" />
                          </label>
                          <div className="mt-2 flex gap-2">
                            <button onClick={() => book(selectedSlot)} className="px-3 py-1 bg-blue-600 text-white rounded">Book</button>
                            <button onClick={() => setSelectedSlot(null)} className="px-3 py-1 border rounded">Cancel</button>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                </div>

              </div>
            )}

          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
