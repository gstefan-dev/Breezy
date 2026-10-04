import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

interface Session {
    id: string;
    code: string;
    name: string;
    isActive: boolean;
    hostId: string;
    createdAt: string;
    visibility: number
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

async function getAudioDuration(file: File): Promise<number> {
    const audioContext = new AudioContext();
    const arrayBuffer = await file.arrayBuffer();
    const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
    return Math.round(audioBuffer.duration);
}

function SessionDetail() {
    const { code } = useParams();
    const { token } = useAuth();
    const [session, setSession] = useState<Session | null>(null);
    const [participants, setParticipants] = useState<Participant[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [fileName, setFileName] = useState("");
    const [genre, setGenre] = useState("");
    const [key, setKey] = useState("");
    const [bpm, setBpm] = useState("");
    const [durationSeconds, setDurationSeconds] = useState(0);
    const [file, setFile] = useState<File | null>(null);
    const [submitError, setSubmitError] = useState("");
    const [submitSuccess, setSubmitSuccess] = useState("");
    const [submitting, setSubmitting] = useState(false);

    async function loadSession() {
        try {
            const [sessionRes, participantsRes] = await Promise.all([
                fetch(`${API_BASE_URL}/api/sessions/${code}`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
                fetch(`${API_BASE_URL}/api/sessions/${code}/participants`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
            ]);

            if (!sessionRes.ok || !participantsRes.ok) {
                throw new Error("Failed to load session");
            }

            setSession(await sessionRes.json());
            setParticipants(await participantsRes.json());

            const submissionsRes = await fetch(`${API_BASE_URL}/api/sessions/${code}/submissions`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            setSubmissions(submissionsRes.ok ? await submissionsRes.json() : []);
        } catch (err) {
            console.error(err);
            setError("Could not load session.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadSession();
    }, [code, token]);

    async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const selectedFile = e.target.files?.[0] ?? null;
        setFile(selectedFile);

        if (selectedFile) {
            const duration = await getAudioDuration(selectedFile);
            setDurationSeconds(duration);
        }
    }

    async function handleSubmitSample(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) {
            setSubmitError("Please select a file.");
            return;
        }

        setSubmitError("");
        setSubmitSuccess("");
        setSubmitting(true);

        try {
            const formData = new FormData();
            formData.append("fileName", fileName);
            formData.append("durationSeconds", durationSeconds.toString());
            formData.append("key", key);
            formData.append("genre", genre);
            formData.append("bpm", bpm);
            formData.append("file", file);

            const response = await fetch(`${API_BASE_URL}/api/sessions/${code}/samples`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to submit sample");
            }

            setSubmitSuccess("Sample submitted successfully!");
            setFileName("");
            setGenre("");
            setKey("");
            setBpm("");
            setDurationSeconds(0);
            setFile(null);

            loadSession();
        } catch (err) {
            console.error(err);
            setSubmitError("Could not submit sample.");
        } finally {
            setSubmitting(false);
        }
    }

    if (loading) return <p>Loading...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;
    if (!session) return <p>Session not found.</p>;

    return (
        <div>
            <h1>{session.name}</h1>
            <p>Code: <strong>{session.code}</strong></p>
            <p>Status: {session.isActive ? "Active" : "Closed"}</p>
            <p>Type: {session.visibility === 1 ? "Collaborative" : "Curated"}</p>
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

            <h2>Submit Sample</h2>
            <form onSubmit={handleSubmitSample}>
                <div>
                    <label htmlFor="fileName">File Name</label>
                    <input id="fileName" type="text" value={fileName} onChange={(e) => setFileName(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="genre">Genre</label>
                    <input id="genre" type="text" value={genre} onChange={(e) => setGenre(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="key">Key</label>
                    <input id="key" type="text" value={key} onChange={(e) => setKey(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="bpm">BPM</label>
                    <input id="bpm" type="number" value={bpm} onChange={(e) => setBpm(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="file">Audio File</label>
                    <input id="file" type="file" accept="audio/*" onChange={handleFileChange} required />
                    {durationSeconds > 0 && <p>Duration detected: {durationSeconds} seconds</p>}
                </div>
                {submitError && <p style={{ color: "red" }}>{submitError}</p>}
                {submitSuccess && <p style={{ color: "green" }}>{submitSuccess}</p>}
                <button type="submit" disabled={submitting}>
                    {submitting ? "Submitting..." : "Submit Sample"}
                </button>
            </form>

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