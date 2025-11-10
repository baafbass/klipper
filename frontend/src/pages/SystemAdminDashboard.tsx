import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { salonApi } from '../api/salon';
import { CreateSalonRequest, SalonManagerRequest } from '../types';

type Salon = {
  id: string;
  name: string;
  description?: string;
  address?: string;
  city?: string;
  phoneNumber?: string;
  email?: string;
  isActive?: boolean;
  workingHours?: { id: string; dayOfWeek: number; dayName: string; openTime: string; closeTime: string; isOpen: boolean }[];
};

export default function SystemAdminDashboard() {
  const [salons, setSalons] = useState<Salon[]>([]);
  const [form, setForm] = useState<CreateSalonRequest>({
    name: '',
    description: '',
    address: '',
    city: '',
    phoneNumber: '',
    email: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Edit modal state
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [editingSalon, setEditingSalon] = useState<Salon | null>(null);
  const [editLoading, setEditLoading] = useState(false);

  // Manager modal (shows after creating a salon if admin wants to add manager)
  const [isManagerModalOpen, setIsManagerModalOpen] = useState(false);
  const [pendingSalonId, setPendingSalonId] = useState<string | null>(null);
  const [managerForm, setManagerForm] = useState<SalonManagerRequest>({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
  });
  const [managerLoading, setManagerLoading] = useState(false);
  const [managerError, setManagerError] = useState<string | null>(null);
  const [managerSuccess, setManagerSuccess] = useState<string | null>(null);

  const fetchSalons = async () => {
    try {
      const res = await salonApi.getAll();
      setSalons(res.data || []);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchSalons();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
  };

  const handleManagerChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setManagerForm((s) => ({ ...s, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const res = await salonApi.create(form);
      const createdSalon: Salon = res.data;
      setSuccess('Salon created successfully.');
      console.log('created salon',createdSalon);
      // Reset salon form
      setForm({ name: '', description: '', address: '', city: '', phoneNumber: '', email: '' });

      // Open manager modal and set pending salon id
      setPendingSalonId(createdSalon.id);
      setIsManagerModalOpen(true);
      setManagerForm({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' });
      setManagerError(null);
      setManagerSuccess(null);

      await fetchSalons();
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create salon');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this salon?')) return;
    try {
      await salonApi.delete(id);
      fetchSalons();
    } catch (err) {
      alert('Delete failed');
    }
  };

  const openEdit = (s: Salon) => {
    setEditingSalon(s);
    setIsEditOpen(true);
  };

  const closeEdit = () => {
    setIsEditOpen(false);
    setEditingSalon(null);
  };

  const handleEditChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    if (!editingSalon) return;
    setEditingSalon({ ...editingSalon, [e.target.name]: e.target.value });
  };

  const saveEdit = async () => {
    if (!editingSalon) return;
    setEditLoading(true);
    setError(null);
    setSuccess(null);
    try {
      await salonApi.update(editingSalon.id, {
        name: editingSalon.name,
        description: editingSalon.description,
        address: editingSalon.address,
        city: editingSalon.city,
        phoneNumber: editingSalon.phoneNumber,
        email: editingSalon.email,
      });
      setSuccess('Salon updated successfully.');
      closeEdit();
      fetchSalons();
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to update salon');
    } finally {
      setEditLoading(false);
    }
  };

  // Manager modal actions
  const submitManager = async () => {
    if (!pendingSalonId) return;
    setManagerLoading(true);
    setManagerError(null);
    setManagerSuccess(null);

    // Basic client-side validation
    if (!managerForm.email || !managerForm.password || !managerForm.firstName || !managerForm.lastName) {
      setManagerError('Please fill required fields (email, password, first name, last name).');
      setManagerLoading(false);
      return;
    }

    try {
      console.log('--->',pendingSalonId,managerForm);
      await salonApi.addManager(pendingSalonId, managerForm);
      setManagerSuccess('Salon manager created successfully.');
      // optionally close modal automatically after a short delay
      setTimeout(() => {
        setIsManagerModalOpen(false);
        setPendingSalonId(null);
        setManagerForm({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' });
      }, 900);
      await fetchSalons();
    } catch (err: any) {
      setManagerError(err?.response?.data?.message || 'Error adding manager.');
    } finally {
      setManagerLoading(false);
    }
  };

  const skipAddManager = () => {
    // close modal and clear pending state
    setIsManagerModalOpen(false);
    setPendingSalonId(null);
    setManagerForm({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' });
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Header />
      <main className="container mx-auto px-4 py-10 flex-1">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Create form */}
          <section className="lg:col-span-1 bg-white p-6 rounded shadow">
            <h2 className="text-2xl font-semibold mb-4">Create New Salon</h2>
            {error && <div className="text-red-600 mb-2">{error}</div>}
            {success && <div className="text-green-600 mb-2">{success}</div>}

            <form onSubmit={handleSubmit}>
              <input name="name" value={form.name} onChange={handleChange} placeholder="Salon Name" required className="w-full p-2 border mb-2 rounded" />
              <textarea name="description" value={form.description} onChange={handleChange} placeholder="Description" className="w-full p-2 border mb-2 rounded" />
              <input name="address" value={form.address} onChange={handleChange} placeholder="Address" className="w-full p-2 border mb-2 rounded" />
              <input name="city" value={form.city} onChange={handleChange} placeholder="City" className="w-full p-2 border mb-2 rounded" />
              <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} placeholder="Phone Number" className="w-full p-2 border mb-2 rounded" />
              <input name="email" value={form.email} onChange={handleChange} placeholder="Salon Email" className="w-full p-2 border mb-4 rounded" />

              <button type="submit" disabled={loading} className="w-full bg-blue-600 text-white py-2 rounded">
                {loading ? 'Creating...' : 'Create Salon'}
              </button>
            </form>
          </section>

          {/* Salon list / details */}
          <section className="lg:col-span-2">
            <h2 className="text-2xl font-semibold mb-4">Manage Salons</h2>
            <div className="grid md:grid-cols-2 gap-6">
              {salons.length === 0 && <div className="text-gray-500">No salons yet.</div>}
              {salons.map((s) => (
                <article key={s.id} className="bg-white p-4 rounded shadow hover:shadow-md transition">
                  <div className="flex items-start justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">{s.name}</h3>
                      <div className="text-sm text-gray-500">{s.city}</div>
                    </div>
                    <div className="flex items-center gap-2">
                      <button onClick={() => openEdit(s)} className="text-sm px-3 py-1 border rounded bg-yellow-50 hover:bg-yellow-100">Edit</button>
                      <button onClick={() => handleDelete(s.id)} className="text-sm px-3 py-1 border rounded bg-red-50 text-red-600 hover:bg-red-100">Delete</button>
                    </div>
                  </div>

                  <p className="mt-3 text-gray-700">{s.description || <span className="text-gray-400 italic">No description</span>}</p>

                  <div className="mt-3 grid grid-cols-2 gap-2 text-sm text-gray-600">
                    <div><span className="font-medium">Address:</span> {s.address || '-'}</div>
                    <div><span className="font-medium">Phone:</span> {s.phoneNumber || '-'}</div>
                    <div><span className="font-medium">Email:</span> {s.email || '-'}</div>
                    <div><span className="font-medium">Active:</span> {s.isActive ? 'Yes' : 'No'}</div>
                  </div>

                  {/* Working hours (if any) */}
                  {s.workingHours && s.workingHours.length > 0 ? (
                    <div className="mt-3">
                      <div className="text-sm font-medium mb-1">Working Hours</div>
                      <ul className="text-sm text-gray-700">
                        {s.workingHours.map((wh) => (
                          <li key={wh.id} className="flex justify-between">
                            <span>{wh.dayName}</span>
                            <span>{wh.isOpen ? `${wh.openTime} - ${wh.closeTime}` : 'Closed'}</span>
                          </li>
                        ))}
                      </ul>
                    </div>
                  ) : (
                    <div className="mt-3 text-sm text-gray-400">No working hours configured.</div>
                  )}
                </article>
              ))}
            </div>
          </section>
        </div>
      </main>
      <Footer />

      {/* Edit modal */}
      {isEditOpen && editingSalon && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-2xl bg-white rounded shadow-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-semibold">Edit Salon</h3>
              <button onClick={closeEdit} className="text-gray-600">Close</button>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <input name="name" value={editingSalon.name} onChange={handleEditChange} placeholder="Salon Name" className="p-2 border rounded" />
              <input name="city" value={editingSalon.city} onChange={handleEditChange} placeholder="City" className="p-2 border rounded" />
              <input name="phoneNumber" value={editingSalon.phoneNumber} onChange={handleEditChange} placeholder="Phone" className="p-2 border rounded" />
              <input name="email" value={editingSalon.email} onChange={handleEditChange} placeholder="Email" className="p-2 border rounded" />
            </div>

            <textarea name="description" value={editingSalon.description} onChange={handleEditChange} placeholder="Description" className="w-full mt-4 p-2 border rounded" />

            <div className="mt-4 flex justify-end gap-2">
              <button onClick={closeEdit} className="px-4 py-2 border rounded">Cancel</button>
              <button onClick={saveEdit} disabled={editLoading} className="px-4 py-2 bg-green-600 text-white rounded">
                {editLoading ? 'Saving...' : 'Save changes'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Manager modal (appears after creating a salon) */}
      {isManagerModalOpen && pendingSalonId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md bg-white rounded shadow-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-semibold">Add Salon Manager</h3>
              <button onClick={skipAddManager} className="text-gray-600">Skip</button>
            </div>

            {managerError && <div className="text-red-600 mb-2">{managerError}</div>}
            {managerSuccess && <div className="text-green-600 mb-2">{managerSuccess}</div>}

            <div className="grid gap-2">
              <input name="firstName" value={managerForm.firstName} onChange={handleManagerChange} placeholder="First name" className="p-2 border rounded" required />
              <input name="lastName" value={managerForm.lastName} onChange={handleManagerChange} placeholder="Last name" className="p-2 border rounded" required />
              <input name="email" value={managerForm.email} onChange={handleManagerChange} placeholder="Email" className="p-2 border rounded" required />
              <input name="phoneNumber" value={managerForm.phoneNumber} onChange={handleManagerChange} placeholder="Phone" className="p-2 border rounded" />
              <input name="password" value={managerForm.password} onChange={handleManagerChange} placeholder="Password" type="password" className="p-2 border rounded" required />
            </div>

            <div className="mt-4 flex justify-end gap-2">
              <button onClick={skipAddManager} className="px-4 py-2 border rounded">Not now</button>
              <button onClick={submitManager} disabled={managerLoading} className="px-4 py-2 bg-blue-600 text-white rounded">
                {managerLoading ? 'Adding...' : 'Add Manager'}
              </button>
            </div>
            <div className="text-xs text-gray-500 mt-3">
              You can add a salon manager now or skip and add one later from the Add Manager page.
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
