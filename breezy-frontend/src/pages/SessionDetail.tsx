import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface Session {
    id: string;
    code: string;
    name: string;
    isActive: boolean;
    hostId: string;
    createdAt: string;
}

interface Participant {
    id: string;
    sessionId: string;
    userId: string;
    userName: string;
    joinedAt: string;
}

interface Submission {
    id: string;
    sessionId: string;
    submitterId: string;
    fileName: string;
    fileUrl: string;
    createdAt: string;
}

function SessionDetail() {
    const { code } = useParams();
    const { token } = useAuth();
    const [session, setSession] = useState<Session | null>(null);
    const [participants, setParticipants] = useState<Participant[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function fetchSession() {
            try {
                const [sessionRes, submissionsRes] = await Promise.all([
                    fetch(`https://localhost:44365/api/sessions/${code}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    fetch(`https://localhost:44365/api/sessions/${code}/submissions`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);

                if (!sessionRes.ok || !submissionsRes.ok) {
                    throw new Error("Failed to load session");
                }

                const sessionData = await sessionRes.json();
                const submissionsData = await submissionsRes.json();

                setSession(sessionData);
                setSubmissions(submissionsData);

                if (sessionData.participants) {
                    setParticipants(sessionData.participants);
                }
            } catch (err) {
                console.error(err);
                setError("Could not load session.");
            } finally {
                setLoading(false);
            }
        }

        fetchSession();
    }, [code, token]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;
    if (!session) return <p>Session not found.</p>;

    return (
        <div>
            <h1>{session.name}</h1>
            <p>Code: <strong>{session.code}</strong></p>
            <p>Status: {session.isActive ? "Active" : "Closed"}</p>
            <p>Created: {new Date(session.createdAt).toLocaleDateString()}</p>

            <h2>Participants</h2>
            {participants.length === 0 && <p>No participants yet.</p>}
            <ul>
                {participants.map((p) => (
                    <li key={p.id}>
                        {p.userName} — joined {new Date(p.joinedAt).toLocaleDateString()}
                    </li>
                ))}
            </ul>

            <h2>Submissions</h2>
            {submissions.length === 0 && <p>No submissions yet.</p>}
            <ul>
                {submissions.map((s) => (
                    <li key={s.id}>
                        {s.fileName} — {new Date(s.createdAt).toLocaleDateString()}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default SessionDetail;