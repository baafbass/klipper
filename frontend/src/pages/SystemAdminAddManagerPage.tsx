import React, { useState } from 'react';
import AdminHeader from '../components/layout/AdminHeader';
import Footer from '../components/layout/Footer';
import { SalonManagerRequest } from '../types';
import { salonApi } from '../api/salon';

export default function SystemAdminAddManagerPage(){
	const [salonId, setSalonId] = useState('');
	const [form, setForm] = useState<SalonManagerRequest>({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' });
	const [message, setMessage] = useState<string | null>(null);


	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
	};


	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		try {
			await salonApi.addManager(salonId, form);
			setMessage('Salon manager created successfully.');
			setForm({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' });
		} catch (err) {
			setMessage('Error adding manager.');
		}
	};

	return (
		<div className="min-h-screen flex flex-col bg-gray-50">
		<AdminHeader />
		<main className="container mx-auto px-4 py-12 flex-1">
		<form onSubmit={handleSubmit} className="max-w-md mx-auto bg-white p-6 rounded shadow">
		<h2 className="text-xl font-semibold mb-4">Add Salon Manager</h2>


		{message && <div className="mb-3 text-blue-600">{message}</div>}


		<input value={salonId} onChange={(e) => setSalonId(e.target.value)} placeholder="Salon ID" className="w-full p-2 border rounded mb-3" required />
		<input name="firstName" value={form.firstName} onChange={handleChange} placeholder="First Name" className="w-full p-2 border rounded mb-3" required />
		<input name="lastName" value={form.lastName} onChange={handleChange} placeholder="Last Name" className="w-full p-2 border rounded mb-3" required />
		<input name="email" value={form.email} onChange={handleChange} placeholder="Email" className="w-full p-2 border rounded mb-3" required />
		<input name="password" value={form.password} onChange={handleChange} placeholder="Password" type="password" className="w-full p-2 border rounded mb-3" required />
		<input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} placeholder="Phone" className="w-full p-2 border rounded mb-4" />


		<button type="submit" className="w-full bg-green-600 text-white py-2 rounded">Add Manager</button>
		</form>
		</main>
		<Footer />
		</div>
		);
}