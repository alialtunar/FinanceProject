import Link from "next/link";
import Image from "next/image";
import { Chat } from "@/types/chat";



const ChatCard = ({last5TransferUsers}:any) => {
  return (
    <div className="col-span-12 rounded-sm border border-stroke bg-white py-6 shadow-default dark:border-strokedark dark:bg-boxdark xl:col-span-4">
      <h4 className="mb-6 px-7.5 text-xl font-semibold text-black dark:text-white">
       Transfer Yapılan Son Kullanıcılar
      </h4>

      <div>
      {last5TransferUsers?.map((user: any, key: any) => (
  <Link
    href="/"
    className="flex items-center gap-5 px-7.5 py-3 hover:bg-gray-3 dark:hover:bg-meta-4"
    key={key}
  >
    <div className="flex flex-1 items-center justify-between">
      <div>
        <h5 className="font-medium text-black dark:text-white">
          {(user?.fullName)?.toUpperCase()}
        </h5>
      </div>
      <div>
        <p className="text-sm text-black dark:text-white">
          {user?.accountNumber}
        </p>
      </div>
    </div>
  </Link>
))}

      </div>
    </div>
  );
};

export default ChatCard;
