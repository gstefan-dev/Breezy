export interface DecodedToken {
    sub: string;
    email: string;
    role: string;
    exp: number;
}

export function decodeToken(token: string): DecodedToken | null {
    try {
        const parts = token.split(".");
        if (parts.length !== 3) return null;
        const decoded = atob(parts[1].replace(/-/g, "+").replace(/_/g, "/"));
        return JSON.parse(decoded);
    } catch {
        return null;
    }
}