import {createContext, useContext, useState, ReactNode} from "react";
import {decodeToken} from "../utils/jwt.ts";

interface AuthContextType {
    token: string | null;
    userId: string | null;
    login: (newToken: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [token, setToken] = useState<string | null>(localStorage.getItem("token"));

    function login(newToken: string) {
        localStorage.setItem("token", newToken);
        setToken(newToken);
    }

    function logout() {
        localStorage.removeItem("token");
        setToken(null);
    }

    const userId = token ? (decodeToken(token)?.sub ?? null) : null;
    return (
        <AuthContext.Provider value={{token, userId, login, logout}}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
}