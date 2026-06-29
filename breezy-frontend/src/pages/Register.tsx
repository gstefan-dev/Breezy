import { useState } from "react";

function Register() {
    const [userEmail, setUserEmail] = useState("");
    const [userName, setUserName] = useState("");
    const [userPassword, setUserPassword] = useState("");
    const [error, setError] = useState("");

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch("https://localhost:44365/api/users/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    UserEmail: userEmail,
                    UserName: userName,
                    Password: userPassword,
                }),
            });

            if (!response.ok) {
                throw new Error("Register failed");
            }

            const user = await response.json();
            console.log("User registered:", user);
        } catch (err) {
            console.error(err);
            setError("Could not register. Please check your details.");
        }
    }

    return (
        <div>
            <h1>Register</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="userEmail">Email address</label>
                    <input
                        id="userEmail"
                        type="email"
                        value={userEmail}
                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserEmail(e.target.value)}
                    />
                </div>
                <div>
                    <label htmlFor="userName">UserName</label>
                    <input
                        id="userName"
                        type="text"
                        value={userName}
                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserName(e.target.value)}
                    />
                </div>
                <div>
                    <label htmlFor="userPassword">Password</label>
                    <input
                        id="userPassword"
                        type="password"
                        value={userPassword}
                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserPassword(e.target.value)}
                    />
                </div>
                {error && <p style={{ color: "red" }}>{error}</p>}
                <button type="submit">Register</button>
            </form>
        </div>
    );
}

export default Register;