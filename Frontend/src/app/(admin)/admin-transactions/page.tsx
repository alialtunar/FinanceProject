"use client";
import React, { useState, useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import TableOne from '@/components/Tables/TableOne5';

const API_URL = 'http://localhost:5233/api/TransactionHistory/Admin/paged';

const TransactionHistoryPage = () => {
  const auth = useAuth();
  const [transactions, setTransactions] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(6); // Sayfa boyutunu 6 olarak ayarladık.
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTransactions = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await fetch(`${API_URL}?page=${page}&pageSize=${pageSize}`);
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        const data = await response.json();
        
        if (!data.isSuccess) {
          throw new Error(data.errorMessages.join(', '));
        }

        setTransactions(data.result); // `result` alanını `setTransactions` ile güncelliyoruz
      } catch (err:any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTransactions();
  }, [page, pageSize]);

  const handlePreviousPage = () => {
    setPage(prevPage => (prevPage > 1 ? prevPage - 1 : 1));
  };

  const handleNextPage = () => {
    setPage(prevPage => prevPage + 1);
  };

  if (loading) return <p>Loading transactions...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <TableOne
      transactions={transactions}
      handlePreviousPage={handlePreviousPage}
      handleNextPage={handleNextPage}
      page={page}
    />
  );
};

export default TransactionHistoryPage;
