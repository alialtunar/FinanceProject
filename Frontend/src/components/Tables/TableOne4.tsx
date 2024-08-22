import React from "react";

const TableOne = ({ users,handlePreviousPage,handleNextPage,page }: any) => {
  // Tarih formatlayıcı fonksiyon
  const formatDate = (dateString: any) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("tr-TR"); // Türkçe formatta gün/ay/yıl
  };

  return (
    <div className="rounded-sm border border-stroke bg-white px-5 pb-2.5 pt-6 shadow-default dark:border-strokedark dark:bg-boxdark sm:px-7.5 xl:pb-1">
      <h4 className="mb-6 text-xl font-semibold text-black dark:text-white">
        Son İşlemler
      </h4>

      <div className="flex flex-col">
        <div className="grid grid-cols-4 rounded-sm bg-gray-2 dark:bg-meta-4 sm:grid-cols-5">
          <div className="p-2.5 xl:p-5">
            <h5 className="text-sm font-medium uppercase xsm:text-base">
             Email
            </h5>
          </div>
          <div className="p-2.5 text-center xl:p-5">
            <h5 className="text-sm font-medium uppercase xsm:text-base">
              Ad ve Soyad
            </h5>
          </div>
          <div className="p-2.5 text-center xl:p-5">
            <h5 className="text-sm font-medium uppercase xsm:text-base">
              Rol
            </h5>
          </div>
         
        </div>

        {users.map((user: any, key: any) => (
          <div
            className={`grid grid-cols-4 sm:grid-cols-5 ${
              key === user.length - 1
                ? ""
                : "border-b border-stroke dark:border-strokedark"
            }`}
            key={key}
          >
            <div className="flex items-center gap-3 p-2.5 xl:p-5">
              <p className="text-black dark:text-white">
                {user.email}
              </p>
            </div>


            <div className="flex items-center gap-3 p-2.5 xl:p-5">
              <p className="text-black dark:text-white">
                {user.fullName}
              </p>
            </div>

            <div className="flex items-center justify-center p-2.5 xl:p-5">
              <p className="text-black dark:text-white">
                {user.role}
              </p>
            </div>

           
   
          </div>
        ))}
      </div>

      <div className="flex justify-center items-center gap-4 my-4">
  <button 
    onClick={handlePreviousPage} 
    disabled={page === 1}
    className="px-4 py-2 bg-blue-500 text-white rounded-lg shadow-md disabled:bg-gray-400 transition-colors duration-300 hover:bg-blue-600 disabled:cursor-not-allowed"
  >
    Previous
  </button>
  
  <span className="text-lg font-medium text-gray-700">Page {page}</span>
  
  <button 
    onClick={handleNextPage}
    className="px-4 py-2 bg-blue-500 text-white rounded-lg shadow-md transition-colors duration-300 hover:bg-blue-600"
  >
    Next
  </button>
</div>

    </div>
  );
};

export default TableOne;
