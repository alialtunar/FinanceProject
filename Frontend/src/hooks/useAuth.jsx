import React from "react";
import Cookies from "universal-cookie";
import { verifyJwtToken } from '../libs/auth';

const fromServer = async () => {
  try {
    const cookies = require("next/headers").cookies;
    const cookieList = cookies();
    const { value: token } = cookieList.get("JWTToken") ?? { value: null };
    const verifiedToken = await verifyJwtToken(token);
    return verifiedToken;
  } catch (error) {
    console.error("Error in fromServer:", error);
    return null;
  }
};

export function useAuth() {
  const [auth, setAuth] = React.useState(null);

  const getVerifiedToken = async () => {
    try {
      const cookies = new Cookies();
      const token = cookies.get("JWTToken") ?? null;
      const verifiedToken = await verifyJwtToken(token);
      setAuth(verifiedToken);
    } catch (error) {
      console.error("Error in getVerifiedToken:", error);
      setAuth(null);
    }
  };

  React.useEffect(() => {
    getVerifiedToken();
  }, []);

  return auth;
}

useAuth.fromServer = fromServer;
