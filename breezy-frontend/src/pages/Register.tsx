import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { API_BASE_URL } from "../config/api";

function Register() {
    const [userEmail, setUserEmail] = useState("");
    const [userName, setUserName] = useState("");
    const [userPassword, setUserPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch(`${API_BASE_URL}/api/users/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    UserEmail: userEmail,
                    UserName: userName,
                    Password: userPassword,
                }),
            });

            if (!response.ok) {
                setError("Could not register. Please check your details.");
                return;
            }

            navigate("/login");
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
                        <label htmlFor="userEmail">Email</label>
                        <input
                            id="userEmail"
                            type="email"
                            value={userEmail}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="userName">Username</label>
                        <input
                            id="userName"
                            type="text"
                            value={userName}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserName(e.target.value)}
                            required
                        />
                    </div>
                    <div>
                        <label htmlFor="userPassword">Password</label>
                        <input
                            id="userPassword"
                            type="password"
                            value={userPassword}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserPassword(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{error}</p>}
                    <button type="submit" style={{ width: "100%" }}>Register</button>
                </form>
                <div className="auth-footer">
                    Already have an account? <Link to="/login">Login</Link>
                </div>
            </div>
        </div>
    );
}

export default Register;