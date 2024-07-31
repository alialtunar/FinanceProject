"use client";
import React, { useEffect, useState } from 'react';
import Cookies from "universal-cookie";
import { useAuth } from '../../../hooks/useAuth';
import { useRouter } from 'next/navigation';

const TransferPage = () => {
  const [showFullName, setShowFullName] = useState(false);
  const [amount, setAmount] = useState('');
  const [accountNumber, setAccountNumber] = useState('');
  const [explanation, setExplanation] = useState('');
  const [fullName, setFullName] = useState('');
  const router = useRouter();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState('');
  const auth = useAuth();


  const handleWithdrawRequest = async () => {
    if (!auth || !auth.nameid) {
      alert("User is not authenticated or does not have a userId");
      return;
    }

    setLoading(true);
    setError('');
    setStatus('');

    try {
      const response = await fetch('http://localhost:5233/api/TransactionHistory/initiate-transfer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId: auth.nameid, amount: parseFloat(amount),recipientAccountNumber:accountNumber }), 
      });
      const data = await response.json();
      if (response.ok) {
        setShowFullName(true);
        setStatus(data.message);
      } else {
        setError(data.message || 'Bir hata oluştu.');
      }
    } catch (error) {
      setError('Bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyCode = async () => {
    setLoading(true);
    setError('');
    setStatus('');

    try {
      const response = await fetch('http://localhost:5233/api/TransactionHistory/transfer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId: auth.nameid,amount: parseFloat(amount),recipientAccountNumber:accountNumber,recipientName:fullName,description:explanation }),
      });
      const data = await response.json();
      console.log('API Yanıtı:', data);
      if (response.ok) {
        setStatus(data.message);
        setTimeout(()=>{
          router.push('/user');
        },[2000])
        
      } else {
        setError(data.message || 'Bir hata oluştu.');
      }
    } catch (error) {
      setError('Bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="rounded-sm border border-stroke bg-white shadow-default dark:border-strokedark dark:bg-boxdark">
        <div className="border-b border-stroke px-7 py-4 dark:border-strokedark">
          <h3 className="font-medium text-black dark:text-white">Para Transferi</h3>
        </div>
        <div className="p-7">
          <form onSubmit={(e) => e.preventDefault()}>
            <div className="mb-5.5 flex flex-col gap-5.5 sm:flex-row">
              <div className="w-full sm:w-1/2">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="amount"
                >
                  Miktar
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="text"
                  name="amount"
                  id="amount"
                  placeholder="Transfer edilecek miktar"
                  value={amount}
                  onChange={(e) => setAmount(e.target.value)}
                />
              </div>
              <div className="w-full sm:w-1/2">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="amount"
                >
                  Hesap Numarası
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="text"
                  name="amount"
                  id="amount"
                  placeholder="Alıcının Hesap Numarası"
                  value={accountNumber}
                  onChange={(e) => setAccountNumber(e.target.value)}
                />
              </div>
            </div>

            {showFullName && (
              <div>
              <div className="mb-5.5">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="fullName"
                >
                  Ad Ve Soyad
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="text"
                  name="fullName"
                  id="fullName"
                  placeholder="Alıcının ad ve soyadı"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                />
              </div>
              <div className="mb-5.5">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="explanation"
                >
                 Açıklama
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="text"
                  name="explanation"
                  id="explanation"
                  placeholder="İşlem açıklaması"
                  value={explanation}
                  onChange={(e) => setExplanation(e.target.value)}
                />
              </div>
              </div>
            )}

            {error && <div className="text-red-500">{error}</div>}
            {status && <div className="text-green-500">{status}</div>}

            <div className="flex justify-end gap-4.5">
              {!showFullName && (
                <button
                  className="flex justify-center rounded bg-primary px-6 py-2 font-medium text-gray hover:bg-opacity-90"
                  type="button"
                  onClick={handleWithdrawRequest}
                  disabled={loading}
                >
                  {loading ? 'Yükleniyor...' : 'İstek Yap'}
                </button>
              )}
              {showFullName && (
                <button
                  className="flex justify-center rounded bg-primary px-6 py-2 font-medium text-gray hover:bg-opacity-90"
                  type="button"
                  onClick={handleVerifyCode}
                  disabled={loading}
                >
                  {loading ? 'Yükleniyor...' : 'Doğrulama Yap'}
                </button>
              )}
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default TransferPage;
