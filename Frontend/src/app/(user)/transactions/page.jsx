"use client";
import React, { useState, useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import TableOne from '@/components/Tables/TableOne2';

const API_URL = 'http://localhost:5233/api/TransactionHistory/paged';

const TransactionHistoryPage = () => {
  const auth = useAuth();
  const [transactions, setTransactions] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [hasLoaded, setHasLoaded] = useState(false);

  useEffect(() => {
    const fetchTransactions = async () => {
      if (!auth?.nameid) return;

      setLoading(true);
      setError('');
      setHasLoaded(false);

      try {
        const response = await fetch(`${API_URL}/${auth.nameid}?page=${page}&pageSize=${pageSize}`);
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        const result = await response.json();
        if (result.isSuccess) {
          setTransactions(result.result);
        } else {
          throw new Error(result.errorMessages.join(', ') || 'An error occurred while fetching transactions.');
        }
      } catch (err) {
        setError(err.message || 'An error occurred while fetching transactions.');
      } finally {
        setLoading(false);
        setHasLoaded(true);
      }
    };

    fetchTransactions();
  }, [auth?.nameid, page, pageSize]);

  const handlePreviousPage = () => {
    setPage(prevPage => Math.max(prevPage - 1, 1));
  };

  const handleNextPage = () => {
    setPage(prevPage => prevPage + 1);
  };

  if (!auth?.nameid) return <p>Loading authentication...</p>;
  if (loading) return <p>Loading transactions...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <TableOne 
      transactions={transactions} 
      handlePreviousPage={handlePreviousPage} 
      handleNextPage={handleNextPage} 
      page={page}
      hasLoaded={hasLoaded} // Pass hasLoaded if needed in TableOne
    />
  );
};

export default TransactionHistoryPage;
