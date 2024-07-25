import {StoreProvider} from '../stores/store-provider'
import "jsvectormap/dist/jsvectormap.css";
import "flatpickr/dist/flatpickr.min.css";
import "@/css/satoshi.css";
import "@/css/style.css";




export default function RootLayout({
  children,
}) {




  return (
    <html lang="en">
      <body>
       <StoreProvider >
      
       {children}
     
        </StoreProvider>
      </body>
    </html>
  );
}
