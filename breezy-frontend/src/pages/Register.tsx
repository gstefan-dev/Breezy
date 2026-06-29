import {useState} from "react";
function Register() {
    const [userEmail, setUserEmail] = useState("");
    const [userName, setUserName] = useState("");
    const [userPassword, setUserPassword] = useState("");

    function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        console.log("Register attempt:", {userEmail, userName, userPassword});
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
                <button type="submit">Register</button>
            </form>
        </div>
    );
}

export default Register;