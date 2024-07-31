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
  const [error, setError] = useState(null);
  const [hasLoaded, setHasLoaded] = useState(false); 

  useEffect(() => {
   
    if (auth && auth.nameid) {
      const fetchTransactions = async () => {
        setLoading(true);
        setError(null);
        try {
          const response = await fetch(`${API_URL}/${auth.nameid}?page=${page}&pageSize=${pageSize}`);
          if (!response.ok) {
            throw new Error('Network response was not ok');
          }
          const data = await response.json();
          setTransactions(data);
        } catch (err) {
          setError(err.message);
        } finally {
          setLoading(false);
          setHasLoaded(true); 
        }
      };

      fetchTransactions();
    }
  }, [auth?.nameid, page, pageSize]);


  const handlePreviousPage = () => {
    setPage((prevPage) => (prevPage > 1 ? prevPage - 1 : 1));
  };

  const handleNextPage = () => {
    setPage((prevPage) => prevPage + 1);
  };

  // Eğer veri hala yüklenmiyorsa ve `auth` mevcut değilse, kullanıcıya bir yüklenme ekranı gösterebilirsiniz
  if (!auth || !auth.nameid) return <p>Loading authentication...</p>;
  if (loading) return <p>Loading transactions...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <TableOne transactions={transactions} handlePreviousPage={handlePreviousPage} handleNextPage={handleNextPage} page={page}/>
  );
};

export default TransactionHistoryPage;
