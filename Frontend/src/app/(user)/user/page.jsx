"use client";
import React, { useState, useEffect } from 'react';
import ECommerce from '../../../components/Dashboard/E-commerce'; // ECommerce bileşeninin yolu
import { useAuth } from '@/hooks/useAuth';

const API_URL = 'http://localhost:5233/api';

const fetchTransactionVolumeLast24Hours = async (userId) => {
  const response = await fetch(`${API_URL}/TransactionHistory/totalamountlast24hours/${userId}`);
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return response.json();
};

const fetchAccountDetails = async (userId) => {
  const response = await fetch(`${API_URL}/account/details/${userId}`);
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return response.json();
};

const fetchLast5Transaction = async (userId) => {
  const response = await fetch(`${API_URL}/TransactionHistory/last5/${userId}`);
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return response.json();
};

const fetchLast5TransferUsers = async (userId) => {
  const response = await fetch(`${API_URL}/TransactionHistory/lastUsers/${userId}`);
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return response.json();
};

const Page = () => {
  const [transactionVolume, setTransactionVolume] = useState(null);
  const [accountDetails, setAccountDetails] = useState(null);
  const [last5Transactions, setLast5Transactions] = useState([]);
  const [last5TransferUsers, setLast5TransferUsers] = useState([]);
  const [status, setStatus] = useState('idle');
  const [error, setError] = useState(null);

  const token = useAuth(); 

  useEffect(() => {
    const fetchData = async () => {
      if (token && token.nameid) {
        setStatus('loading');
        try {
          const [volume, account, transactions, users] = await Promise.all([
            fetchTransactionVolumeLast24Hours(token.nameid),
            fetchAccountDetails(token.nameid),
            fetchLast5Transaction(token.nameid),
            fetchLast5TransferUsers(token.nameid),
          ]);

          setTransactionVolume(volume);
          setAccountDetails(account);
          setLast5Transactions(transactions);
          setLast5TransferUsers(users);
          setStatus('succeeded');
        } catch (err) {
          setStatus('failed');
          setError(err.message);
        }
      }
    };

    fetchData();
  }, [token]);

  if (status === 'loading') {
    return <p>Loading...</p>;
  }

  if (status === 'failed') {
    return <p>Error: {error}</p>;
  }

  if (status === 'succeeded') {
    return (
      <div>
        <ECommerce 
          transactionVolume={transactionVolume} 
          accountDetails={accountDetails} 
          last5Transactions={last5Transactions} 
          last5TransferUsers={last5TransferUsers}
        />
      </div>
    );
  }

  return null; // Yükleme durumu, hata durumu ya da başarılı durum dışında bir şey döndürmeye gerek yok
};

export default Page;
