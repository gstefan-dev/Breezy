import { useEffect, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";

type MyStatus = "Member" | "Left" | "RemovedByOwner";

interface UserSession {
    id: string;
    code: string;
    name: string;
    isActive: boolean;
    hostId: string;
    createdAt: string;
    visibility: number;
    myStatus: MyStatus;
    isArchived: boolean;
}

const MY_STATUS_LABELS: Record<MyStatus, { text: string; badge: string }> = {
    Member: { text: "Member", badge: "badge badge-green" },
    Left: { text: "You left", badge: "badge badge-amber" },
    RemovedByOwner: { text: "Removed by owner", badge: "badge badge-red" },
};

function Sessions() {
    const { token, userId } = useAuth();
    const navigate = useNavigate();
    const [sessions, setSessions] = useState<UserSession[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [actionError, setActionError] = useState("");
    const [showArchived, setShowArchived] = useState(false);

    const [panel, setPanel] = useState<"none" | "create" | "join">("none");

    const [sessionName, setSessionName] = useState("");
    const [maxParticipants, setMaxParticipants] = useState("5");
    const [visibility, setVisibility] = useState("0");
    const [createError, setCreateError] = useState("");

    const [joinCode, setJoinCode] = useState("");
    const [joinError, setJoinError] = useState("");

    async function loadSessions() {
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/api/sessions/user?includeArchived=${showArchived}`, {
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

    useEffect(() => {
        loadSessions();
    }, [token, showArchived]);

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
                const body = await response.json().catch(() => null);
                setJoinError(body?.error ?? "Could not join session. Check the code.");
                return;
            }

            navigate(`/sessions/${code}`);
        } catch (err) {
            console.error(err);
            setJoinError("Something went wrong. Please try again.");
        }
    }

    async function toggleArchive(session: UserSession) {
        setActionError("");
        const action = session.isArchived ? "restore" : "archive";

        const response = await fetch(`${API_BASE_URL}/api/sessions/${session.code}/${action}`, {
            method: "POST",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            loadSessions();
        } else {
            const body = await response.json().catch(() => null);
            setActionError(body?.error ?? `Could not ${action} session.`);
        }
    }

    return (
        <>
            <PageHeader
                title="Sessions"
                actions={
                    <>
                        <button type="button" className="btn" onClick={() => setShowArchived(!showArchived)}>
                            <i className="ti ti-archive" /> {showArchived ? "Hide archived" : "Show archived"}
                        </button>
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

                {actionError && <p style={{ color: "var(--text-danger)", fontSize: 13, marginBottom: 10 }}>{actionError}</p>}
                {loading && <p style={{ color: "var(--text-muted)" }}>Loading...</p>}
                {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
                {!loading && !error && sessions.length === 0 && (
                    <p style={{ color: "var(--text-muted)" }}>No sessions yet. Create one or join with a code.</p>
                )}

                {!loading && sessions.length > 0 && (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Code</th>
                                <th>Role</th>
                                <th>Session</th>
                                <th>You</th>
                                <th>Created</th>
                                <th></th>
                            </tr>
                            </thead>
                            <tbody>
                            {sessions.map((s) => {
                                const isOwner = s.hostId === userId;
                                const archiveBlocked = isOwner && s.isActive && !s.isArchived;
                                const status = MY_STATUS_LABELS[s.myStatus];

                                return (
                                    <tr key={s.id} style={{ opacity: s.isArchived ? 0.6 : 1 }}>
                                        <td>
                                            <NavLink to={`/sessions/${s.code}`} style={{ fontWeight: 500 }}>{s.name}</NavLink>
                                            {s.isArchived && <span className="badge badge-amber" style={{ marginLeft: 6 }}>Archived</span>}
                                        </td>
                                        <td style={{ fontFamily: "ui-monospace, monospace", letterSpacing: "0.1em" }}>{s.code}</td>
                                        <td>
                                            {isOwner
                                                ? <span className="badge badge-blue">Owner</span>
                                                : <span className="badge badge-amber">Participant</span>}
                                        </td>
                                        <td>
                        <span className={s.isActive ? "badge badge-green" : "badge badge-red"}>
                          {s.isActive ? "Open" : "Closed"}
                        </span>
                                        </td>
                                        <td><span className={status.badge}>{status.text}</span></td>
                                        <td>{new Date(s.createdAt).toLocaleDateString()}</td>
                                        <td style={{ textAlign: "right" }}>
                                            <button
                                                type="button"
                                                className="btn"
                                                onClick={() => toggleArchive(s)}
                                                disabled={archiveBlocked}
                                                title={archiveBlocked ? "Close the session before archiving it" : undefined}
                                            >
                                                <i className={s.isArchived ? "ti ti-archive-off" : "ti ti-archive"} />
                                                {s.isArchived ? "Restore" : "Archive"}
                                            </button>
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </>
    );
}

export default Sessions;