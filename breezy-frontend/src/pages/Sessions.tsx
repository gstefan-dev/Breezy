import { useEffect, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";

interface Session {
    id: string;
    code: string;
    name: string;
    isActive: boolean;
    hostId: string;
    createdAt: string;
    visibility: number;
}

function Sessions() {
    const { token, userId } = useAuth();
    const navigate = useNavigate();
    const [sessions, setSessions] = useState<Session[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [panel, setPanel] = useState<"none" | "create" | "join">("none");

    const [sessionName, setSessionName] = useState("");
    const [maxParticipants, setMaxParticipants] = useState("5");
    const [visibility, setVisibility] = useState("0");
    const [createError, setCreateError] = useState("");

    const [joinCode, setJoinCode] = useState("");
    const [joinError, setJoinError] = useState("");

    useEffect(() => {
        async function loadSessions() {
            try {
                const response = await fetch(`${API_BASE_URL}/api/sessions/user`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!response.ok) {
                    throw new Error("Failed to load sessions");
                }

                setSessions(await response.json());
            } catch (err) {
                console.error(err);
                setError("Could not load your sessions.");
            } finally {
                setLoading(false);
            }
        }

        loadSessions();
    }, [token]);

    function togglePanel(target: "create" | "join") {
        setPanel(panel === target ? "none" : target);
    }

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
                    Visibility: Number(visibility),
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
        <>
            <PageHeader
                title="Sessions"
                actions={
                    <>
                        <button type="button" className="btn" onClick={() => togglePanel("join")}>
                            <i className="ti ti-login" /> Join session
                        </button>
                        <button type="button" className="btn btn-primary" onClick={() => togglePanel("create")}>
                            <i className="ti ti-plus" /> New session
                        </button>
                    </>
                }
            />

            <div className="content">
                {panel === "create" && (
                    <div className="card" style={{ maxWidth: 520, marginBottom: 16 }}>
                        <form onSubmit={handleCreate}>
                            <div>
                                <label htmlFor="sessionName">Session Name</label>
                                <input id="sessionName" value={sessionName} onChange={(e) => setSessionName(e.target.value)} required />
                            </div>
                            <div className="grid-2">
                                <div>
                                    <label htmlFor="maxParticipants">Max Participants</label>
                                    <input id="maxParticipants" type="number" min="2" value={maxParticipants}
                                           onChange={(e) => setMaxParticipants(e.target.value)} required />
                                </div>
                                <div>
                                    <label htmlFor="visibility">Session Type</label>
                                    <select id="visibility" value={visibility} onChange={(e) => setVisibility(e.target.value)}>
                                        <option value="0">Curated (only you see submissions)</option>
                                        <option value="1">Collaborative (everyone sees submissions)</option>
                                    </select>
                                </div>
                            </div>
                            {createError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{createError}</p>}
                            <button type="submit" style={{ marginTop: 12 }}>Create session</button>
                        </form>
                    </div>
                )}

                {panel === "join" && (
                    <div className="card" style={{ maxWidth: 360, marginBottom: 16 }}>
                        <form onSubmit={handleJoin}>
                            <div>
                                <label htmlFor="joinCode">Session Code</label>
                                <input id="joinCode" value={joinCode} onChange={(e) => setJoinCode(e.target.value)}
                                       placeholder="e.g. AB12CD" required />
                            </div>
                            {joinError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{joinError}</p>}
                            <button type="submit">Join</button>
                        </form>
                    </div>
                )}

                {loading && <p style={{ color: "var(--text-muted)" }}>Loading...</p>}
                {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
                {!loading && !error && sessions.length === 0 && (
                    <p style={{ color: "var(--text-muted)" }}>No sessions yet. Create one or join with a code.</p>
                )}

                {sessions.length > 0 && (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Code</th>
                                <th>Role</th>
                                <th>Type</th>
                                <th>Status</th>
                                <th>Created</th>
                            </tr>
                            </thead>
                            <tbody>
                            {sessions.map((s) => (
                                <tr key={s.id}>
                                    <td>
                                        <NavLink to={`/sessions/${s.code}`} style={{ fontWeight: 500 }}>{s.name}</NavLink>
                                    </td>
                                    <td style={{ fontFamily: "ui-monospace, monospace", letterSpacing: "0.1em" }}>{s.code}</td>
                                    <td>
                                        {s.hostId === userId
                                            ? <span className="badge badge-blue">Owner</span>
                                            : <span className="badge badge-amber">Participant</span>}
                                    </td>
                                    <td>{s.visibility === 1 ? "Collaborative" : "Curated"}</td>
                                    <td>
                      <span className={s.isActive ? "badge badge-green" : "badge badge-red"}>
                        {s.isActive ? "Active" : "Closed"}
                      </span>
                                    </td>
                                    <td>{new Date(s.createdAt).toLocaleDateString()}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </>
    );
}

export default Sessions;