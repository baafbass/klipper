import React, { useState } from 'react';
import { authApi } from '../../api/auth';
import { LoginRequest } from '../../types';
import { useAuthStore } from '../../store/authStore';
import { useNavigate } from 'react-router-dom';
import { Scissors, Calendar, Clock, User, Mail, Lock, Phone, Cake, CheckCircle, AlertCircle, Menu, X, LogOut, Users, Settings, Briefcase } from 'lucide-react';

export default function LoginForm(){
  
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
            const res = await authApi.login(form);
            const { token, user } = res.data;
            loginStore.login(user, token);
            navigate('/appointments');
        } catch (err: any) {
            setError(err?.response?.data?.message || 'Login failed');
        } finally {
            setLoading(false);
        }
    };

  return (
    <div className="max-w-md mx-auto">
      <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
        <div className="bg-gradient-to-br from-slate-800 to-slate-900 p-8 text-center">
          <div className="inline-block bg-gradient-to-br from-amber-400 to-amber-600 p-4 rounded-2xl shadow-lg mb-4">
            <Scissors className="w-10 h-10 text-slate-900" />
          </div>
          <h2 className="text-3xl font-bold text-white mb-2">Welcome Back</h2>
          <p className="text-slate-300">Sign in to manage your appointments</p>
        </div>

        <div className="p-8 space-y-6">
          {error && (
            <div className="bg-red-50 border-l-4 border-red-500 p-4 rounded-lg flex items-start space-x-3">
              <AlertCircle className="w-5 h-5 text-red-500 flex-shrink-0 mt-0.5" />
              <div className="text-sm text-red-700">{error}</div>
            </div>
          )}

          <div className="space-y-2">
            <label className="block text-sm font-semibold text-slate-700">Email Address</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Mail className="w-5 h-5 text-slate-400" />
              </div>
              <input name="email" value={form.email} onChange={handleChange} required className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none" placeholder="your@email.com"/>
            </div>
          </div>

          <div className="space-y-2">
            <label className="block text-sm font-semibold text-slate-700">Password</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Lock className="w-5 h-5 text-slate-400" />
              </div>
              <input
                type="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                placeholder="••••••••"
              />
            </div>
          </div>

          <button
            onClick={handleSubmit}
            disabled={loading}
            className="w-full bg-gradient-to-r from-amber-500 to-amber-600 hover:from-amber-600 hover:to-amber-700 text-white font-bold py-3.5 rounded-xl shadow-lg hover:shadow-xl transition-all transform hover:scale-[1.02] disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <span className="flex items-center justify-center space-x-2">
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                <span>Signing in...</span>
              </span>
            ) : (
              'Sign In'
            )}
          </button>

          <div className="text-center pt-4 border-t border-slate-200">
            <p className="text-slate-600">
              Don't have an account?{' '}
              <a href="/register" className="text-amber-600 hover:text-amber-700 font-semibold">
                Sign up now
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};