import React, { useState } from 'react';
import { RegisterRequest } from '../../types';
import { authApi } from '../../api/auth';
import { useNavigate } from 'react-router-dom';

export default function RegisterForm() {
    const [form, setForm] = useState<RegisterRequest>({
        email: '', password: '', firstName: '', lastName: '', phoneNumber: '', dateOfBirth: undefined,
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const navigate = useNavigate();


    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
        setForm((s) => ({ ...s, [e.target.name]: e.target.value }));


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            await authApi.register(form as RegisterRequest);
            setSuccess('Registration successful. You can now login.');
            setTimeout(() => navigate('/login'), 1200);
        } catch (err: any) {
            setError(err?.response?.data?.message || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="max-w-md mx-auto p-6 bg-white rounded shadow">
            <h2 className="text-2xl mb-4">Register</h2>


            {error && <div className="mb-3 text-red-600">{error}</div>}
            {success && <div className="mb-3 text-green-600">{success}</div>}


            <label className="block mb-2">First name
                <input name="firstName" value={form.firstName} onChange={handleChange} required className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-2">Last name
                <input name="lastName" value={form.lastName} onChange={handleChange} required className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-2">Email
                <input name="email" value={form.email} onChange={handleChange} required type="email" className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-2">Phone
                <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-2">Password
                <input name="password" value={form.password} onChange={handleChange} type="password" required className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-4">Date of birth
                <input name="dateOfBirth" value={form.dateOfBirth || ''} onChange={handleChange} type="date" className="w-full mt-1 p-2 border rounded" />
            </label>


            <button type="submit" disabled={loading} className="w-full py-2 rounded bg-green-600 text-white">
                {loading ? 'Registering...' : 'Register'}
            </button>
        </form>
    );
}