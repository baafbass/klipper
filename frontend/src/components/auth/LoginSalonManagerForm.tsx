import React, { useState } from 'react';
import { authApi } from '../../api/auth';
import { LoginRequest } from '../../types';
import { useAuthStore } from '../../store/authStore';
import { useNavigate } from 'react-router-dom';


export default function LoginSalonManagerForm() {
    const [form, setForm] = useState<LoginRequest>({ email: '', password: '' });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const loginStore = useAuthStore();
    const navigate = useNavigate();


    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
        setForm((s) => ({ ...s, [e.target.name]: e.target.value }));


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            const res = await authApi.loginSalonManager(form);
            const { token, user } = res.data;
            loginStore.login(user, token);
            navigate('/salon-manager/dashboard');
        } catch (err: any) {
            setError(err?.response?.data?.message || 'Login failed');
        } finally {
            setLoading(false);
        }
    };


    return (
        <form onSubmit={handleSubmit} className="max-w-md mx-auto p-6 bg-white rounded shadow">
            <h2 className="text-2xl mb-4">Login - Salon Manager</h2>


            {error && <div className="mb-3 text-red-600">{error}</div>}


            <label className="block mb-2">Email
                <input name="email" value={form.email} onChange={handleChange} required className="w-full mt-1 p-2 border rounded" />
            </label>


            <label className="block mb-4">Password
                <input name="password" value={form.password} onChange={handleChange} type="password" required className="w-full mt-1 p-2 border rounded" />
            </label>


            <button type="submit" disabled={loading} className="w-full py-2 rounded bg-blue-600 text-white">
                {loading ? 'Logging...' : 'Login'}
            </button>
        </form>
    );
}