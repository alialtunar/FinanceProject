"use client";
import React, { useState, useEffect } from 'react';
import { useAuth } from '../../../hooks/useAuth';
import { useRouter } from 'next/navigation';

const WithdrawPage = () => {
  const [showVerificationCode, setShowVerificationCode] = useState(false);
  const [amount, setAmount] = useState('');
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState('');
  const router = useRouter();
  const auth = useAuth();

  useEffect(() => {
    if (auth) {
      console.log("Verified Token:", auth.nameid);
    }
  }, [auth]);

  const handleWithdrawRequest = async () => {
    if (!auth || !auth.nameid) {
      alert("User is not authenticated or does not have a userId");
      return;
    }

    setLoading(true);
    setError('');
    setStatus('');

    try {
      const response = await fetch('http://localhost:5233/api/TransactionHistory/InitiateWithdraw', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId: auth.nameid, amount: parseFloat(amount) }),
      });
      const data = await response.json();
      if (response.ok) {
        setShowVerificationCode(true);
        setStatus(data.message);
      } else {
        setError(data.message || 'An error occurred.');
      }
    } catch (error) {
      setError('An error occurred. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyCode = async () => {
    if (!auth || !auth.nameid) {
      alert("User is not authenticated or does not have a userId");
      return;
    }

    setLoading(true);
    setError('');
    setStatus('');

    try {
      const response = await fetch('http://localhost:5233/api/TransactionHistory/Withdraw', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId: auth.nameid, code, amount: parseFloat(amount) }),
      });
      const data = await response.json();
      if (response.ok) {
        setStatus(data.message);
        setTimeout(() => {
          router.push('/user');
        }, 2000);
      } else {
        setError(data.message || 'An error occurred.');
      }
    } catch (error) {
      setError('An error occurred. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="rounded-sm border border-stroke bg-white shadow-default dark:border-strokedark dark:bg-boxdark">
        <div className="border-b border-stroke px-7 py-4 dark:border-strokedark">
          <h3 className="font-medium text-black dark:text-white">Withdraw Funds</h3>
        </div>
        <div className="p-7">
          <form onSubmit={(e) => e.preventDefault()}>
            <div className="mb-5.5 flex flex-col gap-5.5 sm:flex-row">
              <div className="w-full sm:w-1/2">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="amount"
                >
                  Amount
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="number"
                  name="amount"
                  id="amount"
                  placeholder="Amount to withdraw"
                  value={amount}
                  onChange={(e) => setAmount(e.target.value)}
                />
              </div>
            </div>

            {showVerificationCode && (
              <div className="mb-5.5">
                <label
                  className="mb-3 block text-sm font-medium text-black dark:text-white"
                  htmlFor="verificationCode"
                >
                  Verification Code
                </label>
                <input
                  className="w-full rounded border border-stroke bg-gray px-4.5 py-3 text-black focus:border-primary focus-visible:outline-none dark:border-strokedark dark:bg-meta-4 dark:text-white dark:focus:border-primary"
                  type="text"
                  name="verificationCode"
                  id="verificationCode"
                  placeholder="Verification Code"
                  value={code}
                  onChange={(e) => setCode(e.target.value)}
                />
              </div>
            )}

            {error && <div className="text-red-500">{error}</div>}
            {status && <div className="text-green-500">{status}</div>}

            <div className="flex justify-end gap-4.5">
              {!showVerificationCode ? (
                <button
                  className="flex justify-center rounded bg-primary px-6 py-2 font-medium text-gray hover:bg-opacity-90"
                  type="button"
                  onClick={handleWithdrawRequest}
                  disabled={loading}
                >
                  {loading ? 'Loading...' : 'Request Withdrawal'}
                </button>
              ) : (
                <button
                  className="flex justify-center rounded bg-primary px-6 py-2 font-medium text-gray hover:bg-opacity-90"
                  type="button"
                  onClick={handleVerifyCode}
                  disabled={loading}
                >
                  {loading ? 'Loading...' : 'Verify Code'}
                </button>
              )}
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default WithdrawPage;
