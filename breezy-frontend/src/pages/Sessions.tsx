import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

function Sessions() {
    const { token } = useAuth();
    const navigate = useNavigate();

    const [sessionName, setSessionName] = useState("");
    const [maxParticipants, setMaxParticipants] = useState("5");
    const [createError, setCreateError] = useState("");

    const [joinCode, setJoinCode] = useState("");
    const [joinError, setJoinError] = useState("");

    async function handleCreate(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setCreateError("");

        try {
            const response = await fetch(`${API_BASE_URL}/api/sessions`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    SessionName: sessionName,
                    MaxParticipants: Number(maxParticipants),
                }),
            });

            if (!response.ok) {
                throw new Error("Failed to create session");
            }

            const session = await response.json();
            navigate(`/sessions/${session.code}`);
        } catch (err) {
            console.error(err);
            setCreateError("Could not create session.");
        }
    }

    async function handleJoin(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setJoinError("");

        const code = joinCode.trim().toUpperCase();

        try {
            const response = await fetch(`${API_BASE_URL}/api/sessions/${code}/participants`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!response.ok) {
                throw new Error("Failed to join session");
            }

            navigate(`/sessions/${code}`);
        } catch (err) {
            console.error(err);
            setJoinError("Could not join session. Check the code.");
        }
    }

    return (
        <div>
            <h1>Sessions</h1>

            <h2>Create Session</h2>
            <form onSubmit={handleCreate}>
                <div>
                    <label htmlFor="sessionName">Session Name</label>
                    <input
                        id="sessionName"
                        type="text"
                        value={sessionName}
                        onChange={(e) => setSessionName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="maxParticipants">Max Participants</label>
                    <input
                        id="maxParticipants"
                        type="number"
                        min="2"
                        value={maxParticipants}
                        onChange={(e) => setMaxParticipants(e.target.value)}
                        required
                    />
                </div>
                {createError && <p style={{ color: "red" }}>{createError}</p>}
                <button type="submit">Create</button>
            </form>

            <h2>Join Session</h2>
            <form onSubmit={handleJoin}>
                <div>
                    <label htmlFor="joinCode">Session Code</label>
                    <input
                        id="joinCode"
                        type="text"
                        value={joinCode}
                        onChange={(e) => setJoinCode(e.target.value)}
                        required
                    />
                </div>
                {joinError && <p style={{ color: "red" }}>{joinError}</p>}
                <button type="submit">Join</button>
            </form>
        </div>
    );
}

export default Sessions;