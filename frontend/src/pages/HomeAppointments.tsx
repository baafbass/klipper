import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { salonApi } from '../api/salon';
import { appointmentApi } from '../api/appointments';
import { useAuthStore } from '../store/authStore';

export default function HomeAppointments() {
  const [salons, setSalons] = useState<any[]>([]);
  const [selectedSalon, setSelectedSalon] = useState<any | null>(null);
  const [date, setDate] = useState<string>(new Date().toISOString().slice(0,10));
  const [availability, setAvailability] = useState<any[]>([]);
  const [selectedServiceIds, setSelectedServiceIds] = useState<string[]>([]);
  const [selectedEmployeeId, setSelectedEmployeeId] = useState<string | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<any | null>(null);
  const [notes, setNotes] = useState('');
  const auth = useAuthStore();

  useEffect(() => { loadSalons(); }, []);

  async function loadSalons() {
    const res = await salonApi.getAll();
    setSalons(res.data);
  }

  async function loadSalonDetails(id: string) {
    const res = await salonApi.getDetails(id); // new endpoint
    setSelectedSalon(res.data);
    // selectedSalon.Services will have: id,name,durationMinutes,price,employees: [userDto,...]
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
      setAvailability(res.data);
    } catch (err:any) {
      alert(err?.response?.data?.error || 'Failed to check availability');
    }
  }

  async function book(slot: any) {
    console.log('selected slot',slot);
    if (!auth.user) { alert('Please login as customer'); return; }
    try {
      const payload = {
        customerId: auth.user.id,
        employeeId: slot.employeeId,
        salonId: selectedSalon.id,
        appointmentDate: date,
        startTime: slot.startTime,
        serviceIds: selectedServiceIds,
        notes
      };
      await appointmentApi.createAppointment(payload);
      alert('Appointment created successfully (pending).');
      setAvailability([]);
      setSelectedServiceIds([]);
      setSelectedSlot(null);
    } catch (err:any) {
      console.log('error',err);
      alert(err?.response?.data?.error || 'Booking failed');
    }
  }

  return (
    <div>
      <Header />
      <main className="container mx-auto p-6">
        <div className="grid md:grid-cols-3 gap-6">
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
          </div>

          <div className="col-span-2">
            {!selectedSalon && <div>Select a salon</div>}
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
                                  <div className="text-sm text-gray-600">{slot.startTime} - {slot.endTime} ({slot.durationMinutes ?? '—'} min)</div>
                                </div>
                                <div className="flex gap-2">
                                  <button onClick={() => setSelectedSlot(slot)} className="px-3 py-1 bg-green-600 text-white rounded">Select</button>
                                </div>
                              </li>
                            ))}
                          </ul>
                        )}
                      </div>

                      {selectedSlot && (
                        <div className="mt-4 p-3 border rounded bg-gray-50">
                          <div>Selected: {selectedSlot.employeeName} — {selectedSlot.startTime} to {selectedSlot.endTime}</div>
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
