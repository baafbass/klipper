import React, { useEffect, useState } from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';
import { salonApi } from '../api/salon';
import { CreateSalonRequest } from '../types';

export default function SystemAdminDashboard(){
const [salons, setSalons] = useState<any[]>([]);
const [form, setForm] = useState<CreateSalonRequest>({
name: '', description: '', address: '', city: '', phoneNumber: '', email: '',
});
const [loading, setLoading] = useState(false);
const [error, setError] = useState<string | null>(null);
const [success, setSuccess] = useState<string | null>(null);


const fetchSalons = async () => {
try {
const res = await salonApi.getAll();
setSalons(res.data);
} catch (err) {
console.error(err);
}
};

useEffect(() => { fetchSalons(); }, []);


const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
};


const handleSubmit = async (e: React.FormEvent) => {
e.preventDefault();
setLoading(true);
setError(null);
setSuccess(null);
try {
await salonApi.create(form);
setSuccess('Salon created successfully.');
setForm({ name: '', description: '', address: '', city: '', phoneNumber: '', email: '' });
fetchSalons();
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


return (
<div className="min-h-screen flex flex-col bg-gray-50">
<Header />
<main className="container mx-auto px-4 py-10 flex-1">
<div className="grid md:grid-cols-2 gap-8">
<form onSubmit={handleSubmit} className="bg-white p-6 rounded shadow">
<h2 className="text-xl font-semibold mb-4">Create New Salon</h2>
{error && <div className="text-red-600 mb-2">{error}</div>}
{success && <div className="text-green-600 mb-2">{success}</div>}


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


<div className="bg-white p-6 rounded shadow">
<h2 className="text-xl font-semibold mb-4">Manage Salons</h2>
<ul>
{salons.map((s) => (
<li key={s.id} className="border-b py-3 flex justify-between items-center">
<div>
<div className="font-medium">{s.name}</div>
<div className="text-sm text-gray-500">{s.city}</div>
</div>
<button onClick={() => handleDelete(s.id)} className="text-red-600 hover:underline">Delete</button>
</li>
))}
</ul>
</div>
</div>
</main>
<Footer />
</div>
);
}