
import React from "react";

import ChatCard from "../Chat/ChatCard";
import TableOne from "../Tables/TableOne";
import CardDataStats from "../CardDataStats";


const ECommerce= ({transactionVolume,accountDetails,last5Transactions,last5TransferUsers}) => {
  return (
    <>
      <div className="grid grid-cols-1 gap-4 md:grid-cols-3 md:gap-6 xl:grid-cols-4 2xl:gap-7.5">
        <CardDataStats title="Hesap Numarası" total={accountDetails?.accountNumber} rate="0.43%" levelUp>
      
        </CardDataStats>
        <CardDataStats title="Bakiye" total={accountDetails?.balance} rate="0.43%" levelUp>
      
      </CardDataStats>
      <CardDataStats title="Ad ve Soyad" total={(accountDetails?.fullName)?.toUpperCase()} rate="0.43%" levelUp>
      
      </CardDataStats>
      <CardDataStats title="Son 24 saatteki işlem hacmi" total={`${transactionVolume} ₺`} rate="0.43%" levelUp>
      
      </CardDataStats>

      </div>

      <div className="mt-4 grid grid-cols-12 gap-4 md:mt-6 md:gap-6 2xl:mt-7.5 2xl:gap-7.5">

        <div className="col-span-12 xl:col-span-8">
          <TableOne transactions={last5Transactions} />
        </div>
        <ChatCard last5TransferUsers={last5TransferUsers} />
      </div>
    </>
  );
};

export default ECommerce;
