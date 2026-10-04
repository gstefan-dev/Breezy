import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch(`${API_BASE_URL}/api/users/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: email, Password: password }),
            });

            if (!response.ok) {
                setError("Invalid email or password.");
                return;
            }

            const token = await response.text();
            login(token);
            navigate("/dashboard");
        } catch (err) {
            console.error(err);
            setError("Something went wrong. Please try again.");
        }
    }

    return (
        <div className="auth-page">
            <div className="auth-card">
                <div className="auth-logo">Bree<span>zy</span></div>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{error}</p>}
                    <button type="submit" style={{ width: "100%" }}>Login</button>
                </form>
                <div className="auth-footer">
                    Don't have an account? <Link to="/register">Register</Link>
                </div>
            </div>
        </div>
    );
}

export default Login;